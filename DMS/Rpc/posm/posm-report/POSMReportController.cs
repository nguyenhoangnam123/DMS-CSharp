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

            if (POSMReport_ShowingOrderFilterDTO.HasValue == false)
                return 0;

            DateTime Start = POSMReport_ShowingOrderFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_ShowingOrderFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_ShowingOrderFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

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
                        join soc in DataContext.ShowingOrderContent on so.Id equals soc.ShowingOrderId
                        join s in DataContext.Store on so.StoreId equals s.Id
                        join tt in tempTableQuery.Query on so.StoreId equals tt.Column1
                        where Start <= so.Date && so.Date <= End &&
                        (ShowingItemIds == null || ShowingItemIds.Count == 0 || ShowingItemIds.Contains(soc.ShowingItemId)) &&
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
                        select new
                        {
                            StoreId = s.Id,
                            OrganizationId = so.OrganizationId
                        };
            return await query.Distinct().CountAsync();
        }

        [Route(POSMReportRoute.List), HttpPost]
        public async Task<ActionResult<List<POSMReport_POSMReportDTO>>> List([FromBody] POSMReport_POSMReportFilterDTO POSMReport_POSMReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (POSMReport_POSMReportFilterDTO.HasValue == false)
                return new List<POSMReport_POSMReportDTO>();

            DateTime Start = POSMReport_POSMReportFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_POSMReportFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = (await ListData(POSMReport_POSMReportFilterDTO, Start, End)).Value;

            return POSMReport_POSMReportDTOs;
        }

        [Route(POSMReportRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] POSMReport_POSMReportFilterDTO POSMReport_POSMReportFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = POSMReport_POSMReportFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.GreaterEqual.Value;

            DateTime End = POSMReport_POSMReportFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    POSMReport_POSMReportFilterDTO.Date.LessEqual.Value;

            POSMReport_POSMReportFilterDTO.Skip = 0;
            POSMReport_POSMReportFilterDTO.Take = int.MaxValue;
            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = (await ListData(POSMReport_POSMReportFilterDTO, Start, End)).Value;

            long stt = 1;
            foreach (POSMReport_POSMReportDTO POSMReport_POSMReportDTO in POSMReport_POSMReportDTOs)
            {
                foreach (var Store in POSMReport_POSMReportDTO.Stores)
                {
                    Store.STT = stt;
                    stt++;
                }
            }

            var Total = POSMReport_POSMReportDTOs.SelectMany(x => x.Stores).Select(x => x.Total).DefaultIfEmpty(0).Sum();
            var SumQuantity = POSMReport_POSMReportDTOs.SelectMany(x => x.Stores).SelectMany(x => x.Contents).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();

            var OrgRoot = await DataContext.Organization
                .Where(x => x.DeletedAt == null && 
                x.StatusId == StatusEnum.ACTIVE.Id && 
                x.ParentId.HasValue == false)
                .FirstOrDefaultAsync();

            string path = "Templates/POSM_Report.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.Data = POSMReport_POSMReportDTOs;
            Data.RootName = OrgRoot == null ? "" : OrgRoot.Name.ToUpper();
            Data.Total = Total;
            Data.SumQuantity = SumQuantity;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "POSMReport.xlsx");
        }

        private async Task<ActionResult<List<POSMReport_POSMReportDTO>>> ListData([FromBody] POSMReport_POSMReportFilterDTO POSMReport_ShowingOrderFilterDTO, DateTime Start, DateTime End)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

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

            var Ids = await query.Distinct().OrderBy(x => x.OrganizationId).ThenBy(x => x.Store.Name)
                .Select(x => new
                {
                    StoreId = x.StoreId,
                    OrganizationId = x.OrganizationId,
                })
                .Skip(POSMReport_ShowingOrderFilterDTO.Skip)
                .Take(POSMReport_ShowingOrderFilterDTO.Take)
                .ToListAsync();

            StoreIds = Ids.Select(x => x.StoreId).Distinct().ToList();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            var ShowingOrderDAOs = await query.Where(x => StoreIds.Contains(x.StoreId)).ToListAsync();
            var ShowingOrderIds = ShowingOrderDAOs.Select(x => x.Id).ToList();

            var queryContent = from c in DataContext.ShowingOrderContent
                               join i in DataContext.ShowingItem on c.ShowingItemId equals i.Id
                               join u in DataContext.UnitOfMeasure on c.UnitOfMeasureId equals u.Id
                               where ShowingOrderIds.Contains(c.ShowingOrderId) &&
                               (ShowingItemIds == null || ShowingItemIds.Count == 0 || ShowingItemIds.Contains(c.ShowingItemId))
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

            var Stores = await DataContext.Store.Where(x => StoreIds.Contains(x.Id))
                .Select(x => new StoreDAO
                {
                    Id = x.Id,
                    Code = x.Code,
                    CodeDraft = x.CodeDraft,
                    Name = x.Name,
                    Address = x.Address,
                    StoreStatus = x.StoreStatus == null ? null : new StoreStatusDAO
                    {
                        Name = x.StoreStatus.Name
                    }
                }).ToListAsync();
            List<POSMReport_POSMReportDTO> POSMReport_POSMReportDTOs = new List<POSMReport_POSMReportDTO>();
            foreach (var Organization in Organizations)
            {
                POSMReport_POSMReportDTO POSMReport_POSMReportDTO = new POSMReport_POSMReportDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    Stores = new List<POSMReport_POSMStoreDTO>()
                };
                POSMReport_POSMReportDTO.Stores = Ids
                        .Where(x => x.OrganizationId == Organization.Id)
                        .Select(x => new POSMReport_POSMStoreDTO
                        {
                            Id = x.StoreId,
                            OrganizationId = Organization.Id
                        }).ToList();
                POSMReport_POSMReportDTOs.Add(POSMReport_POSMReportDTO);
            }

            foreach (POSMReport_POSMReportDTO POSMReport_POSMReportDTO in POSMReport_POSMReportDTOs)
            {
                foreach (POSMReport_POSMStoreDTO POSMReport_POSMStoreDTO in POSMReport_POSMReportDTO.Stores)
                {
                    var Store = Stores.Where(x => x.Id == POSMReport_POSMStoreDTO.Id).FirstOrDefault();
                    if (Store != null)
                    {
                        POSMReport_POSMStoreDTO.Code = Store.Code;
                        POSMReport_POSMStoreDTO.CodeDraft = Store.CodeDraft;
                        POSMReport_POSMStoreDTO.Name = Store.Name;
                        POSMReport_POSMStoreDTO.Address = Store.Address;
                        POSMReport_POSMStoreDTO.StoreStatusName = Store.StoreStatus.Name;
                    }

                    var subOrder = ShowingOrderDAOs.Where(x => x.StoreId == POSMReport_POSMStoreDTO.Id &&
                        x.OrganizationId == POSMReport_POSMStoreDTO.OrganizationId)
                        .ToList();
                    POSMReport_POSMStoreDTO.Total = subOrder.Select(x => x.Total).DefaultIfEmpty(0).Sum();

                    var subIds = subOrder.Select(x => x.Id).ToList();
                    var subContent = ShowingOrderContentDAOs.Where(x => subIds.Contains(x.ShowingOrderId)).ToList();
                    foreach (var Content in subContent)
                    {
                        POSMReport_POSMReportContentDTO POSMReport_POSMReportContentDTO = POSMReport_POSMStoreDTO.Contents
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
                            POSMReport_POSMStoreDTO.Contents.Add(POSMReport_POSMReportContentDTO);
                        }
                        else
                        {
                            POSMReport_POSMReportContentDTO.Amount += Content.Amount;
                            POSMReport_POSMReportContentDTO.Quantity += Content.Quantity;
                        }
                    }
                }
            }

            return POSMReport_POSMReportDTOs;
        }
    }
}

