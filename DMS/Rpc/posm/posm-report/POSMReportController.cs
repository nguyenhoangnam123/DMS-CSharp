using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using System.Dynamic;
using DMS.Entities;
using DMS.Services.MShowingOrder;
using DMS.Services.MAppUser;
using DMS.Services.MOrganization;
using DMS.Services.MShowingWarehouse;
using DMS.Services.MStatus;
using DMS.Services.MShowingItem;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Enums;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;

namespace DMS.Rpc.posm.posm_report
{
    public partial class POSMReportController : RpcController
    {
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IShowingItemService ShowingItemService;
        private IShowingOrderService ShowingOrderService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private DataContext DataContext;
        private ICurrentContext CurrentContext;
        public POSMReportController(
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IShowingItemService ShowingItemService,
            IShowingOrderService ShowingOrderService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            DataContext DataContext,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.ShowingItemService = ShowingItemService;
            this.ShowingOrderService = ShowingOrderService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.DataContext = DataContext;
            this.CurrentContext = CurrentContext;
        }

        [Route(POSMReportRoute.Count), HttpPost]
        public async Task<ActionResult<long>> Count([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = POSMReport_ShowingOrderFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_ShowingOrderFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.LessEqual.Value;

            long? StoreId = POSMReport_ShowingOrderFilterDTO.StoreId?.Equal;
            long? StoreTypeId = POSMReport_ShowingOrderFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = POSMReport_ShowingOrderFilterDTO.StoreGroupingId?.Equal;
            List<long> ShowingItemIds = POSMReport_ShowingOrderFilterDTO.ShowingItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (POSMReport_ShowingOrderFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == POSMReport_ShowingOrderFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from so in DataContext.ShowingOrder
                        join s in DataContext.Store on so.StoreId equals s.Id
                        join tt in tempTableQuery.Query on so.StoreId equals tt.Column1
                        where Start <= so.Date && so.Date <= End &&
                        AppUserIds.Contains(so.AppUserId) &&
                        (StoreTypeIds.Contains(s.StoreTypeId)) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(s.OrganizationId) &&
                        so.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null
                        select so.StoreId;
            return await query.Distinct().CountAsync();
        }

        [Route(POSMReportRoute.List), HttpPost]
        public async Task<ActionResult<List<POSMReport_POSMReportDTO>>> List([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = POSMReport_ShowingOrderFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_ShowingOrderFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.LessEqual.Value;

            long? StoreId = POSMReport_ShowingOrderFilterDTO.StoreId?.Equal;
            long? StoreTypeId = POSMReport_ShowingOrderFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = POSMReport_ShowingOrderFilterDTO.StoreGroupingId?.Equal;
            List<long> ShowingItemIds = POSMReport_ShowingOrderFilterDTO.ShowingItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (POSMReport_ShowingOrderFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == POSMReport_ShowingOrderFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            AppUserFilter AppUserFilter = new AppUserFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                Id = new IdFilter { },
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.DisplayName | AppUserSelect.Organization
            };
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);
            var AppUsers = await AppUserService.List(AppUserFilter);
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();

            List<long> StoreIds = await FilterStore(StoreService, OrganizationService, CurrentContext);
            if (StoreId.HasValue)
            {
                var listId = new List<long> { StoreId.Value };
                StoreIds = StoreIds.Intersect(listId).ToList();
            }
            List<long> StoreTypeIds = await FilterStoreType(StoreTypeService, CurrentContext);
            if (StoreTypeId.HasValue)
            {
                var listId = new List<long> { StoreTypeId.Value };
                StoreTypeIds = StoreTypeIds.Intersect(listId).ToList();
            }
            List<long> StoreGroupingIds = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            if (StoreGroupingId.HasValue)
            {
                var listId = new List<long> { StoreGroupingId.Value };
                StoreGroupingIds = StoreGroupingIds.Intersect(listId).ToList();
            }

            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);

            var query = from so in DataContext.ShowingOrder
                        join s in DataContext.Store on so.StoreId equals s.Id
                        join tt in tempTableQuery.Query on so.StoreId equals tt.Column1
                        where Start <= so.Date && so.Date <= End &&
                        AppUserIds.Contains(so.AppUserId) &&
                        (StoreTypeIds.Contains(s.StoreTypeId)) &&
                        (
                            (
                                StoreGroupingId.HasValue == false &&
                                (s.StoreGroupingId.HasValue == false || StoreGroupingIds.Contains(s.StoreGroupingId.Value))
                            ) ||
                            (
                                StoreGroupingId.HasValue &&
                                StoreGroupingId.Value == s.StoreGroupingId.Value
                            )
                        ) &&
                        OrganizationIds.Contains(s.OrganizationId) &&
                        so.StatusId == StatusEnum.ACTIVE.Id &&
                        s.DeletedAt == null
                        select new ShowingOrderDAO
                        {
                            Id = so.Id,
                            AppUserId = so.AppUserId,
                            Code = so.Code,
                            Date = so.Date,
                            OrganizationId = so.OrganizationId,
                            StoreId = so.StoreId,
                            Total = so.Total,
                            Store = new StoreDAO
                            {
                                Id = s.Id,
                                OrganizationId = s.OrganizationId,
                                Name = s.Name,
                            }
                        };

            StoreIds = await query.OrderBy(x => x.Store.OrganizationId).ThenBy(x => x.Store.Name)
                .Select(x => x.StoreId)
                .Distinct()
                .Skip(POSMReport_ShowingOrderFilterDTO.Skip)
                .Take(POSMReport_ShowingOrderFilterDTO.Take)
                .ToListAsync();
            var ShowingOrderDAOs = await query.Where(x => StoreIds.Contains(x.StoreId)).ToListAsync();
            var ShowingOrderIds = ShowingOrderDAOs.Select(x => x.Id).ToList();

            var queryContent = from c in DataContext.ShowingOrderContent
                               join i in DataContext.ShowingItem on c.ShowingItemId equals i.Id
                               join u in DataContext.UnitOfMeasure on c.UnitOfMeasureId equals u.Id
                               where ShowingOrderIds.Contains(c.ShowingOrderId)
                               select new ShowingOrderContentDAO
                               {
                                   Id = c.Id,
                                   Amount = c.Amount,
                                   Quantity = c.Quantity,
                                   SalePrice = c.SalePrice,
                                   ShowingItemId = c.ShowingItemId,
                                   ShowingOrderId = c.ShowingOrderId,
                                   UnitOfMeasureId = c.UnitOfMeasureId,
                                   ShowingItem = new ShowingItemDAO
                                   {
                                       Code = i.Code,
                                       Name = i.Name,
                                   },
                                   UnitOfMeasure = new UnitOfMeasureDAO
                                   {
                                       Name = u.Name
                                   }
                               };
            var ShowingOrderContentDAOs = await queryContent.ToListAsync();

            var Stores = await DataContext.Store.Where(x => StoreIds.Contains(x.Id)).ToListAsync();
            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = new List<POSMReport_POSMReportDTO>();
            foreach (var Store in Stores)
            {
                POSMReport_POSMReportDTO POSMReport_POSMReportDTO = new POSMReport_POSMReportDTO()
                {
                    StoreId = Store.Id,
                    StoreCode = Store.Code,
                    StoreName = Store.Name,
                    StoreAddress = Store.Address,
                    Contents = new List<POSMReport_POSMReportContentDTO>()
                };
                POSMReport_POSMReportDTOs.Add(POSMReport_POSMReportDTO);
                var subOrder = ShowingOrderDAOs.Where(x => x.StoreId == Store.Id).ToList();
                POSMReport_POSMReportDTO.Total = subOrder.Select(x => x.Total).DefaultIfEmpty(0).Sum();

                var subIds = subOrder.Select(x => x.Id).ToList();
                var subContent = ShowingOrderContentDAOs.Where(x => subIds.Contains(x.ShowingOrderId)).ToList();
                foreach (var Content in subContent)
                {
                    POSMReport_POSMReportContentDTO POSMReport_POSMReportContentDTO = POSMReport_POSMReportDTO.Contents
                        .Where(x => x.ShowingItemId == Content.ShowingItemId)
                        .Where(x => x.UnitOfMeasure == Content.UnitOfMeasure.Name)
                        .Where(x => x.SalePrice == Content.SalePrice)
                        .FirstOrDefault();
                    if (POSMReport_POSMReportContentDTO == null)
                    {
                        POSMReport_POSMReportContentDTO = new POSMReport_POSMReportContentDTO
                        {
                            Amount = Content.Amount,
                            Quantity = Content.Quantity,
                            SalePrice = Content.SalePrice,
                            ShowingItemId = Content.ShowingItemId,
                            ShowingItemCode = Content.ShowingItem.Code,
                            ShowingItemName = Content.ShowingItem.Name,
                            UnitOfMeasure = Content.UnitOfMeasure.Name
                        };
                        POSMReport_POSMReportDTO.Contents.Add(POSMReport_POSMReportContentDTO);
                    }
                    else
                    {
                        POSMReport_POSMReportContentDTO.Amount += Content.Amount;
                        POSMReport_POSMReportContentDTO.Quantity += Content.Quantity;
                    }
                }
            }

            return POSMReport_POSMReportDTOs;
        }

        //[Route(POSMReportRoute.Export), HttpPost]
        //public async Task<ActionResult> Export([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);

        //    MemoryStream memoryStream = new MemoryStream();
        //    using (ExcelPackage excel = new ExcelPackage(memoryStream))
        //    {
        //        #region ShowingOrder
        //        var ShowingOrderFilter = ConvertFilterDTOToFilterEntity(POSMReport_ShowingOrderFilterDTO);
        //        ShowingOrderFilter.Skip = 0;
        //        ShowingOrderFilter.Take = int.MaxValue;
        //        ShowingOrderFilter = await ShowingOrderService.ToFilter(ShowingOrderFilter);
        //        List<ShowingOrder> ShowingOrders = await ShowingOrderService.List(ShowingOrderFilter);

        //        var ShowingOrderHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Id",
        //                "Code",
        //                "AppUserId",
        //                "OrganizationId",
        //                "Date",
        //                "ShowingWarehouseId",
        //                "StatusId",
        //                "Total",
        //                "RowId",
        //            }
        //        };
        //        List<object[]> ShowingOrderData = new List<object[]>();
        //        for (int i = 0; i < ShowingOrders.Count; i++)
        //        {
        //            var ShowingOrder = ShowingOrders[i];
        //            ShowingOrderData.Add(new Object[]
        //            {
        //                ShowingOrder.Id,
        //                ShowingOrder.Code,
        //                ShowingOrder.AppUserId,
        //                ShowingOrder.OrganizationId,
        //                ShowingOrder.Date,
        //                ShowingOrder.ShowingWarehouseId,
        //                ShowingOrder.StatusId,
        //                ShowingOrder.Total,
        //                ShowingOrder.RowId,
        //            });
        //        }
        //        excel.GenerateWorksheet("ShowingOrder", ShowingOrderHeaders, ShowingOrderData);
        //        #endregion

        //        #region AppUser
        //        var AppUserFilter = new AppUserFilter();
        //        AppUserFilter.Selects = AppUserSelect.ALL;
        //        AppUserFilter.OrderBy = AppUserOrder.Id;
        //        AppUserFilter.OrderType = OrderType.ASC;
        //        AppUserFilter.Skip = 0;
        //        AppUserFilter.Take = int.MaxValue;
        //        List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

        //        var AppUserHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Id",
        //                "Username",
        //                "DisplayName",
        //                "Address",
        //                "Email",
        //                "Phone",
        //                "SexId",
        //                "Birthday",
        //                "Avatar",
        //                "PositionId",
        //                "Department",
        //                "OrganizationId",
        //                "ProvinceId",
        //                "Longitude",
        //                "Latitude",
        //                "StatusId",
        //                "GPSUpdatedAt",
        //                "RowId",
        //            }
        //        };
        //        List<object[]> AppUserData = new List<object[]>();
        //        for (int i = 0; i < AppUsers.Count; i++)
        //        {
        //            var AppUser = AppUsers[i];
        //            AppUserData.Add(new Object[]
        //            {
        //                AppUser.Id,
        //                AppUser.Username,
        //                AppUser.DisplayName,
        //                AppUser.Address,
        //                AppUser.Email,
        //                AppUser.Phone,
        //                AppUser.SexId,
        //                AppUser.Birthday,
        //                AppUser.Avatar,
        //                AppUser.PositionId,
        //                AppUser.Department,
        //                AppUser.OrganizationId,
        //                AppUser.ProvinceId,
        //                AppUser.Longitude,
        //                AppUser.Latitude,
        //                AppUser.StatusId,
        //                AppUser.GPSUpdatedAt,
        //                AppUser.RowId,
        //            });
        //        }
        //        excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
        //        #endregion
        //        #region Organization
        //        var OrganizationFilter = new OrganizationFilter();
        //        OrganizationFilter.Selects = OrganizationSelect.ALL;
        //        OrganizationFilter.OrderBy = OrganizationOrder.Id;
        //        OrganizationFilter.OrderType = OrderType.ASC;
        //        OrganizationFilter.Skip = 0;
        //        OrganizationFilter.Take = int.MaxValue;
        //        List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

        //        var OrganizationHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Id",
        //                "Code",
        //                "Name",
        //                "ParentId",
        //                "Path",
        //                "Level",
        //                "StatusId",
        //                "Phone",
        //                "Email",
        //                "Address",
        //                "RowId",
        //            }
        //        };
        //        List<object[]> OrganizationData = new List<object[]>();
        //        for (int i = 0; i < Organizations.Count; i++)
        //        {
        //            var Organization = Organizations[i];
        //            OrganizationData.Add(new Object[]
        //            {
        //                Organization.Id,
        //                Organization.Code,
        //                Organization.Name,
        //                Organization.ParentId,
        //                Organization.Path,
        //                Organization.Level,
        //                Organization.StatusId,
        //                Organization.Phone,
        //                Organization.Email,
        //                Organization.Address,
        //                Organization.RowId,
        //            });
        //        }
        //        excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
        //        #endregion

        //        #region Status
        //        var StatusFilter = new StatusFilter();
        //        StatusFilter.Selects = StatusSelect.ALL;
        //        StatusFilter.OrderBy = StatusOrder.Id;
        //        StatusFilter.OrderType = OrderType.ASC;
        //        StatusFilter.Skip = 0;
        //        StatusFilter.Take = int.MaxValue;
        //        List<Status> Statuses = await StatusService.List(StatusFilter);

        //        var StatusHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Id",
        //                "Code",
        //                "Name",
        //            }
        //        };
        //        List<object[]> StatusData = new List<object[]>();
        //        for (int i = 0; i < Statuses.Count; i++)
        //        {
        //            var Status = Statuses[i];
        //            StatusData.Add(new Object[]
        //            {
        //                Status.Id,
        //                Status.Code,
        //                Status.Name,
        //            });
        //        }
        //        excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
        //        #endregion
        //        #region ShowingItem
        //        var ShowingItemFilter = new ShowingItemFilter();
        //        ShowingItemFilter.Selects = ShowingItemSelect.ALL;
        //        ShowingItemFilter.OrderBy = ShowingItemOrder.Id;
        //        ShowingItemFilter.OrderType = OrderType.ASC;
        //        ShowingItemFilter.Skip = 0;
        //        ShowingItemFilter.Take = int.MaxValue;
        //        List<ShowingItem> ShowingItems = await ShowingItemService.List(ShowingItemFilter);

        //        var ShowingItemHeaders = new List<string[]>()
        //        {
        //            new string[] {
        //                "Id",
        //                "Code",
        //                "Name",
        //                "CategoryId",
        //                "UnitOfMeasureId",
        //                "SalePrice",
        //                "Desception",
        //                "StatusId",
        //                "Used",
        //                "RowId",
        //            }
        //        };
        //        List<object[]> ShowingItemData = new List<object[]>();
        //        for (int i = 0; i < ShowingItems.Count; i++)
        //        {
        //            var ShowingItem = ShowingItems[i];
        //            ShowingItemData.Add(new Object[]
        //            {
        //                ShowingItem.Id,
        //                ShowingItem.Code,
        //                ShowingItem.Name,
        //                ShowingItem.CategoryId,
        //                ShowingItem.UnitOfMeasureId,
        //                ShowingItem.SalePrice,
        //                ShowingItem.Desception,
        //                ShowingItem.StatusId,
        //                ShowingItem.Used,
        //                ShowingItem.RowId,
        //            });
        //        }
        //        excel.GenerateWorksheet("ShowingItem", ShowingItemHeaders, ShowingItemData);
        //        #endregion

        //        excel.Save();
        //    }
        //    return File(memoryStream.ToArray(), "application/octet-stream", "ShowingOrder.xlsx");
        //}
    }
}

