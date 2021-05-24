using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.MOrganization;
using Microsoft.AspNetCore.Mvc;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using System;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MProduct;
using DMS.Services.MAppUser;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using System.Diagnostics;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using DMS.Services.MProductGrouping;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_by_employee_and_item
{
    public class ReportDirectSalesOrderByEmployeeAndItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ReportDirectSalesOrderByEmployeeAndItemController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.ItemService = ItemService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListAppUser), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO>> FilterListAppUser([FromBody] ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.Id = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.Id;
            AppUserFilter.Username = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = ReportDirectSalesOrderByEmployeeAndItem_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO> ReportDirectSalesOrderByEmployeeAndItem_AppUserDTOs = AppUsers
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_AppUserDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_AppUserDTOs;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO>> FilterListOrganization([FromBody] ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            OrganizationFilter.Id = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportDirectSalesOrderByEmployeeAndItem_OrganizationFilterDTO.Name;
            OrganizationFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO> ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTOs = Organizations
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_OrganizationDTOs;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListItem), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_ItemDTO>> FilterListItem([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO.Name;
            ItemFilter.Search = ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO.Search;
            ItemFilter.StatusId = ReportDirectSalesOrderByEmployeeAndItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_ItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ItemDTOs = Items
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_ItemDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_ItemDTOs;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingDTO> ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingDTOs = ProductGroupings
                .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingDTO(x)).ToList();
            return ReportDirectSalesOrderByEmployeeAndItem_ProductGroupingDTOs;
        }
        #endregion

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            var SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;
            List<long> ItemIds = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.ItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.DirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var DirectSalesOrderIds = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(DirectSalesOrderIds);

            var transactionQuery = from t in DataContext.DirectSalesOrderTransaction
                                   join tt in tempTableQuery.Query on t.DirectSalesOrderId equals tt.Column1
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };
            int count = await transactionQuery.Distinct().CountAsync();
            return count;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>>> List([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>();

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = await ListData(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO, Start, End);
            return ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Total), HttpPost]
        public async Task<ReportDirectSalesOrderByEmployeeAndItem_TotalDTO> Total([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.HasValue == false)
                return new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                     LocalStartDay(CurrentContext) :
                     ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO ReportDirectSalesOrderByEmployeeAndItem_TotalDTO = await TotalData(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO, Start, End);
            return ReportDirectSalesOrderByEmployeeAndItem_TotalDTO;
        }

        [Route(ReportDirectSalesOrderByEmployeeAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrderDate.LessEqual.Value;

            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Skip = 0;
            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Take = int.MaxValue;
            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = await ListData(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO, Start, End);

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO ReportDirectSalesOrderByEmployeeAndItem_TotalDTO = await TotalData(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO, Start, End);
            long stt = 1;
            foreach (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs)
            {
                foreach (var SaleEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    SaleEmployee.STT = stt;
                    stt++;
                }
            }


            string path = "Templates/Report_Direct_Sales_Order_By_Employee_And_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByEmployeeAndItems = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs;
            Data.Total = ReportDirectSalesOrderByEmployeeAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportDirectSalesOrderByEmployeeAndItem.xlsx");
        }

        private async Task<List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>> ListData(
            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;
            List<long> ItemIds = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.ItemId?.In;
            long? ProductGroupingId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.ProductGroupingId?.Equal;

            if (ProductGroupingId.HasValue)
            {
                var ItemDAOs = await ItemService.List(new ItemFilter
                {
                    ProductGroupingId = new IdFilter { Equal = ProductGroupingId.Value },
                    Selects = ItemSelect.Id
                });
                if (ItemIds != null)
                {
                    ItemIds = ItemIds.Union(ItemDAOs.Select(x => x.Id).ToList())
                            .Distinct()
                            .ToList();
                }
                else
                {
                    ItemIds = ItemDAOs.Select(x => x.Id).ToList();
                }
            }

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.DirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var DirectSalesOrderIds = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(DirectSalesOrderIds);

            var transactionQuery = from t in DataContext.DirectSalesOrderTransaction
                                   join tt in tempTableQuery.Query on t.DirectSalesOrderId equals tt.Column1
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };

            var Ids = await transactionQuery
                .Distinct()
                .OrderBy(x => x.OrganizationId)
                .Skip(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Skip)
                .Take(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.Take)
                .ToListAsync();
            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            var transactionQuery2 = from t in DataContext.DirectSalesOrderTransaction
                                    join tt in tempTableQuery.Query on t.DirectSalesOrderId equals tt.Column1
                                    where AppUserIds.Contains(t.SalesEmployeeId) &&
                                    (SaleEmployeeId.HasValue == false || t.SalesEmployeeId == SaleEmployeeId.Value) &&
                                    OrganizationIds.Contains(t.OrganizationId) &&
                                    (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                    select t.DirectSalesOrderId;

            DirectSalesOrderIds = await transactionQuery2.ToListAsync();
            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
                .Where(x => DirectSalesOrderIds.Contains(x.Id))
                .ToListAsync();
            List<DirectSalesOrderContentDAO> DirectSalesOrderContentDAOs = await DataContext.DirectSalesOrderContent
                .Where(x => DirectSalesOrderIds.Contains(x.DirectSalesOrderId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)))
                .Select(x => new DirectSalesOrderContentDAO
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    PrimaryPrice = x.PrimaryPrice,
                    RequestedQuantity = x.RequestedQuantity,
                    SalePrice = x.SalePrice,
                    TaxAmount = x.TaxAmount,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    DirectSalesOrder = x.DirectSalesOrder == null ? null : new DirectSalesOrderDAO
                    {
                        BuyerStoreId = x.DirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotionDAOs = await DataContext.DirectSalesOrderPromotion
                .Where(x => DirectSalesOrderIds.Contains(x.DirectSalesOrderId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)))
                .Select(x => new DirectSalesOrderPromotionDAO
                {
                    Id = x.Id,
                    RequestedQuantity = x.RequestedQuantity,
                    DirectSalesOrderId = x.DirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    DirectSalesOrder = x.DirectSalesOrder == null ? null : new DirectSalesOrderDAO
                    {
                        BuyerStoreId = x.DirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            ItemIds = new List<long>();
            ItemIds.AddRange(DirectSalesOrderContentDAOs.Select(x => x.ItemId));
            ItemIds.AddRange(DirectSalesOrderPromotionDAOs.Select(x => x.ItemId));
            ItemIds = ItemIds.Distinct().ToList();

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name | ItemSelect.SalePrice
            });

            List<UnitOfMeasureDAO> UnitOfMeasureDAOs = await DataContext.UnitOfMeasure.ToListAsync();
            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = new List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>();
            foreach (var Organization in Organizations)
            {
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO>()
                };
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees = Ids
                        .Where(x => x.OrganizationId == Organization.Id)
                        .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                        {
                            SaleEmployeeId = x.SalesEmployeeId
                        }).ToList();
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs.Add(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO);
            }
            // khởi tạo khung dữ liệu
            Parallel.ForEach(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs, ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO =>
            {
                foreach (var SalesEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    var Employee = AppUserDAOs.Where(x => x.Id == SalesEmployee.SaleEmployeeId).FirstOrDefault();
                    if (Employee != null)
                    {
                        SalesEmployee.Username = Employee.Username;
                        SalesEmployee.DisplayName = Employee.DisplayName;
                    }

                    //lấy tất cả đơn hàng được thực hiện bởi nhân viên đang xét
                    var DirectSalesOrders = DirectSalesOrderDAOs.Where(x => x.SaleEmployeeId == SalesEmployee.SaleEmployeeId).ToList();
                    var SalesOrderIds = DirectSalesOrders.Select(x => x.Id).ToList();

                    SalesEmployee.Items = new List<ReportDirectSalesOrderByEmployeeAndItem_ItemDTO>();
                    foreach (DirectSalesOrderContentDAO DirectSalesOrderContentDAO in DirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderContentDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = SalesEmployee.Items.Where(i => i.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                SalesEmployee.Items.Add(ReportDirectSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = DirectSalesOrderContentDAO.DirectSalesOrder.BuyerStoreId;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderContentDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.SaleStock += DirectSalesOrderContentDAO.RequestedQuantity;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.SalePriceAverage += (DirectSalesOrderContentDAO.SalePrice * DirectSalesOrderContentDAO.RequestedQuantity);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.Revenue += (DirectSalesOrderContentDAO.Amount - (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (DirectSalesOrderContentDAO.TaxAmount ?? 0));
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.Discount += ((DirectSalesOrderContentDAO.DiscountAmount ?? 0) + (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                        }
                    }

                    foreach (var item in SalesEmployee.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (DirectSalesOrderPromotionDAO DirectSalesOrderPromotionDAO in DirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderPromotionDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = SalesEmployee.Items.Where(i => i.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByEmployeeAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByEmployeeAndItem_ItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    StoreIds = new HashSet<long>(),
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                SalesEmployee.Items.Add(ReportDirectSalesOrderByEmployeeAndItem_ItemDTO);
                            }
                            var BuyerStoreId = DirectSalesOrderPromotionDAO.DirectSalesOrder.BuyerStoreId;
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.StoreIds.Add(BuyerStoreId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderPromotionDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByEmployeeAndItem_ItemDTO.PromotionStock += DirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            });

            //làm tròn số
            Parallel.ForEach(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs, ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO =>
            {
                foreach (var SaleEmployee in ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees)
                {
                    foreach (var item in SaleEmployee.Items)
                    {
                        item.Discount = Math.Round(item.Discount, 0);
                        item.Revenue = Math.Round(item.Revenue, 0);
                        item.SalePriceAverage = Math.Round(item.SalePriceAverage, 0);
                    }
                }
            });

            return ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs;
        }

        private async Task<ReportDirectSalesOrderByEmployeeAndItem_TotalDTO> TotalData(
            ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO,
            DateTime Start, DateTime End)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long? SaleEmployeeId = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.AppUserId?.Equal;
            List<long> ItemIds = ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.ItemId?.In;

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO ReportDirectSalesOrderByEmployeeAndItem_TotalDTO = new ReportDirectSalesOrderByEmployeeAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.DirectSalesOrder
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
                             (SaleEmployeeId.HasValue == false || i.SaleEmployeeId == SaleEmployeeId.Value) &&
                             OrganizationIds.Contains(i.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id
                             select i.Id;

            var DirectSalesOrderIds = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(DirectSalesOrderIds);

            var transactionQuery = from t in DataContext.DirectSalesOrderTransaction
                                   join tt in tempTableQuery.Query on t.DirectSalesOrderId equals tt.Column1
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       SalesEmployeeId = t.SalesEmployeeId
                                   };

            var Ids = await transactionQuery
                .OrderBy(x => x.OrganizationId)
                .Distinct().ToListAsync();
            AppUserIds = Ids.Select(x => x.SalesEmployeeId).Distinct().ToList();

            List<AppUserDAO> AppUserDAOs = await DataContext.AppUser
                .Where(x => x.DeletedAt == null)
                .Where(au => AppUserIds.Contains(au.Id))
                .OrderBy(su => su.OrganizationId).ThenBy(x => x.DisplayName)
                .ToListAsync();
            OrganizationIds = Ids.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization
                .Where(x => OrganizationIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new OrganizationDAO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO> ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs = new List<ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO>();
            foreach (var Organization in Organizations)
            {
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO = new ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO()
                {
                    OrganizationId = Organization.Id,
                    OrganizationName = Organization.Name,
                    SaleEmployees = new List<ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO>()
                };
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO.SaleEmployees = Ids
                        .Where(x => x.OrganizationId == Organization.Id)
                        .Select(x => new ReportDirectSalesOrderByEmployeeAndItem_SaleEmployeeDTO
                        {
                            SaleEmployeeId = x.SalesEmployeeId
                        }).ToList();
                ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTOs.Add(ReportDirectSalesOrderByEmployeeAndItem_ReportDirectSalesOrderByEmployeeAndItemDTO);
            }

            var DirectSalesOrderContentQuery = DataContext.DirectSalesOrderContent
                .Where(x => AppUserIds.Contains(x.DirectSalesOrder.SaleEmployeeId) &&
                OrganizationIds.Contains(x.DirectSalesOrder.OrganizationId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)) &&
                Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End &&
                x.DirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);
            var DirectSalesOrderPromotionQuery = DataContext.DirectSalesOrderPromotion
                .Where(x => AppUserIds.Contains(x.DirectSalesOrder.SaleEmployeeId) &&
                OrganizationIds.Contains(x.DirectSalesOrder.OrganizationId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)) &&
                Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End &&
                x.DirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalSalesStock = DirectSalesOrderContentQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalPromotionStock = DirectSalesOrderPromotionQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalRevenue = Math.Round(DirectSalesOrderContentQuery.Select(x => x.Amount).Sum()
                - DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.TaxAmount.HasValue).Select(x => x.TaxAmount.Value).Sum(), 0);

            ReportDirectSalesOrderByEmployeeAndItem_TotalDTO.TotalDiscount = Math.Round(DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.DiscountAmount.HasValue).Select(x => x.DiscountAmount.Value).Sum(), 0);

            return ReportDirectSalesOrderByEmployeeAndItem_TotalDTO;
        }

    }
}