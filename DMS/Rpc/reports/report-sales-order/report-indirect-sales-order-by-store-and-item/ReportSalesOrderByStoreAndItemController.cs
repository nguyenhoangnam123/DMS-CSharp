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
using System.IO;
using System.Dynamic;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;
using DMS.Services.MStoreStatus;
using DMS.Services.MAppUser;
using DMS.Services.MProductGrouping;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_by_store_and_item
{
    public class ReportSalesOrderByStoreAndItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IStoreStatusService StoreStatusService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderByStoreAndItemController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IStoreStatusService StoreStatusService,
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
            this.StoreStatusService = StoreStatusService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_OrganizationDTO>> FilterListOrganization()
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
            OrganizationFilter.IsDisplay = true;

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportSalesOrderByStoreAndItem_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportSalesOrderByStoreAndItem_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListStore), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_StoreDTO>> FilterListStore([FromBody] ReportSalesOrderByStoreAndItem_StoreFilterDTO ReportSalesOrderByStoreAndItem_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportSalesOrderByStoreAndItem_StoreFilterDTO.Id;
            StoreFilter.Code = ReportSalesOrderByStoreAndItem_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ReportSalesOrderByStoreAndItem_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ReportSalesOrderByStoreAndItem_StoreFilterDTO.Name;
            StoreFilter.OrganizationId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportSalesOrderByStoreAndItem_StoreDTO> ReportSalesOrderByStoreAndItem_StoreDTOs = Stores
                .Select(x => new ReportSalesOrderByStoreAndItem_StoreDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_StoreDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreGroupingFilter.Id == null) StoreGroupingFilter.Id = new IdFilter();
            StoreGroupingFilter.Id.In = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportSalesOrderByStoreAndItem_StoreGroupingDTO> ReportSalesOrderByStoreAndItem_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportSalesOrderByStoreAndItem_StoreGroupingDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_StoreGroupingDTOs;
        }
        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_StoreTypeDTO>> FilterListStoreType([FromBody] ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreTypeFilter.Id == null) StoreTypeFilter.Id = new IdFilter();
            StoreTypeFilter.Id.In = await FilterStoreType(StoreTypeService, CurrentContext);
            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportSalesOrderByStoreAndItem_StoreTypeDTO> ReportSalesOrderByStoreAndItem_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportSalesOrderByStoreAndItem_StoreTypeDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_StoreTypeDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportSalesOrderByStoreAndItem_StoreStatusFilterDTO ReportSalesOrderByStoreAndItem_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportSalesOrderByStoreAndItem_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportSalesOrderByStoreAndItem_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportSalesOrderByStoreAndItem_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportSalesOrderByStoreAndItem_StoreStatusDTO> ReportSalesOrderByStoreAndItem_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportSalesOrderByStoreAndItem_StoreStatusDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_StoreStatusDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListItem), HttpPost]

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] ReportSalesOrderByStoreAndItem_ProductGroupingFilterDTO ReportSalesOrderByStoreAndItem_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Code = ReportSalesOrderByStoreAndItem_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ReportSalesOrderByStoreAndItem_ProductGroupingFilterDTO.Name;

            if (ProductGroupingFilter.Id == null) ProductGroupingFilter.Id = new IdFilter();
            ProductGroupingFilter.Id.In = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            List<ProductGrouping> ReportSalesOrderByStoreAndItemGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ReportSalesOrderByStoreAndItem_ProductGroupingDTO> ReportSalesOrderByStoreAndItem_ProductGroupingDTOs = ReportSalesOrderByStoreAndItemGroupings
                .Select(x => new ReportSalesOrderByStoreAndItem_ProductGroupingDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_ProductGroupingDTOs;
        }

        public async Task<List<ReportSalesOrderByStoreAndItem_ItemDTO>> FilterListItem([FromBody] ReportSalesOrderByStoreAndItem_ItemFilterDTO ReportSalesOrderByStoreAndItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportSalesOrderByStoreAndItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportSalesOrderByStoreAndItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportSalesOrderByStoreAndItem_ItemFilterDTO.Name;
            ItemFilter.StatusId = ReportSalesOrderByStoreAndItem_ItemFilterDTO.StatusId;
            ItemFilter.Search = ReportSalesOrderByStoreAndItem_ItemFilterDTO.Search;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportSalesOrderByStoreAndItem_ItemDTO> ReportSalesOrderByStoreAndItem_ItemDTOs = Items
                .Select(x => new ReportSalesOrderByStoreAndItem_ItemDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_ItemDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;
            List<long> ItemIds = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.ItemId?.In;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.IndirectSalesOrder
                             join s in DataContext.Store on i.BuyerStoreId equals s.Id
                             join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
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
                             (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                             OrganizationIds.Contains(s.OrganizationId) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                             s.DeletedAt == null
                             select i.Id;

            var Ids = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery2 = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var transactionQuery = from t in DataContext.IndirectSalesOrderTransaction
                                   join tt in tempTableQuery2.Query on t.IndirectSalesOrderId equals tt.Column1
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new
                                   {
                                       OrganizationId = t.OrganizationId,
                                       StoreId = t.BuyerStoreId
                                   };

            int count = await transactionQuery.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO>>> List([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return new List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO>();

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = await ListData(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO, Start, End);
            return ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Total), HttpPost]
        public async Task<ReportSalesOrderByStoreAndItem_TotalDTO> Total([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return new ReportSalesOrderByStoreAndItem_TotalDTO();

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportSalesOrderByStoreAndItem_TotalDTO();

            ReportSalesOrderByStoreAndItem_TotalDTO ReportSalesOrderByStoreAndItem_TotalDTO = await TotalData(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO, Start, End);
            return ReportSalesOrderByStoreAndItem_TotalDTO;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Skip = 0;
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Take = int.MaxValue;
            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = await ListData(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO, Start, End);

            ReportSalesOrderByStoreAndItem_TotalDTO ReportSalesOrderByStoreAndItem_TotalDTO = await TotalData(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO, Start, End);
            long stt = 1;
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores)
                {
                    Store.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Sales_Order_By_Store_And_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByStoreAndItems = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs;
            Data.Total = ReportSalesOrderByStoreAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportSalesOrderByStoreAndItem.xlsx");
        }

        private async Task<List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO>> ListData(
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;
            List<long> ItemIds = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.ItemId?.In;
            long? ProductGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.ProductGroupingId?.Equal;

            if (ProductGroupingId.HasValue)
            {
                var ItemDAOs = await ItemService.List(new ItemFilter
                {
                    ProductGroupingId = new IdFilter { Equal = ProductGroupingId.Value },
                    Selects = ItemSelect.Id
                });
                ItemIds = ItemIds.Union(ItemDAOs.Select(x => x.Id).ToList())
                    .Distinct()
                    .ToList();
            }

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.IndirectSalesOrder
                             join s in DataContext.Store on i.BuyerStoreId equals s.Id
                             join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
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
                             (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                             (OrganizationIds.Contains(s.OrganizationId)) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                             s.DeletedAt == null
                             select i.Id;

            var Ids = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery2 = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var transactionQuery = from t in DataContext.IndirectSalesOrderTransaction
                                   join tt in tempTableQuery2.Query on t.IndirectSalesOrderId equals tt.Column1
                                   join s in DataContext.Store on t.BuyerStoreId equals s.Id
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new Store
                                   {
                                       Id = s.Id,
                                       Code = s.Code,
                                       CodeDraft = s.CodeDraft,
                                       Name = s.Name,
                                       Address = s.Address,
                                       OrganizationId = s.OrganizationId,
                                       StoreStatusId = s.StoreStatusId
                                   };

            List<Store> Stores = await transactionQuery.Distinct().OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
                .Skip(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Skip)
                .Take(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Take)
                .ToListAsync();

            OrganizationIds = Stores.Select(x => x.OrganizationId).Distinct().ToList();
            OrganizationDAOs = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).ToListAsync();

            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = OrganizationDAOs.Select(on => new ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO
            {
                OrganizationId = on.Id,
                OrganizationName = on.Name,
            }).ToList();
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores = Stores
                    .Where(x => x.OrganizationId == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.OrganizationId)
                    .Select(x => new ReportSalesOrderByStoreAndItem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        CodeDraft = x.CodeDraft,
                        Name = x.Name,
                        StoreStatusId = x.StoreStatusId,
                        Address = x.Address,
                        OrganizationId = x.OrganizationId,
                        StoreGroupingId = x.StoreGroupingId,
                        StoreTypeId = x.StoreTypeId,
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();

            var transactionQuery2 = from t in DataContext.IndirectSalesOrderTransaction
                                    join tt in tempTableQuery2.Query on t.IndirectSalesOrderId equals tt.Column1
                                    where StoreIds.Contains(t.BuyerStoreId) &&
                                    (StoreId.HasValue == false || t.BuyerStoreId == StoreId.Value) &&
                                    OrganizationIds.Contains(t.OrganizationId) &&
                                    (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                    select t.IndirectSalesOrderId;

            var IndirectSalesOrderIds = await transactionQuery2.ToListAsync();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => IndirectSalesOrderIds.Contains(x.Id))
                .Select(x => new IndirectSalesOrderDAO
                {
                    Id = x.Id,
                    BuyerStoreId = x.BuyerStoreId
                }).ToListAsync();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)))
                .Select(x => new IndirectSalesOrderContentDAO
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    PrimaryPrice = x.PrimaryPrice,
                    RequestedQuantity = x.RequestedQuantity,
                    SalePrice = x.SalePrice,
                    TaxAmount = x.TaxAmount,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    IndirectSalesOrder = x.IndirectSalesOrder == null ? null : new IndirectSalesOrderDAO
                    {
                        BuyerStoreId = x.IndirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = await DataContext.IndirectSalesOrderPromotion
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)))
                .Select(x => new IndirectSalesOrderPromotionDAO
                {
                    Id = x.Id,
                    RequestedQuantity = x.RequestedQuantity,
                    IndirectSalesOrderId = x.IndirectSalesOrderId,
                    ItemId = x.ItemId,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    IndirectSalesOrder = x.IndirectSalesOrder == null ? null : new IndirectSalesOrderDAO
                    {
                        BuyerStoreId = x.IndirectSalesOrder.BuyerStoreId,
                    }
                })
                .ToListAsync();
            ItemIds = new List<long>();
            ItemIds.AddRange(IndirectSalesOrderContentDAOs.Select(x => x.ItemId));
            ItemIds.AddRange(IndirectSalesOrderPromotionDAOs.Select(x => x.ItemId));
            ItemIds = ItemIds.Distinct().ToList();

            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ItemIds },
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name | ItemSelect.SalePrice
            });

            List<UnitOfMeasureDAO> UnitOfMeasureDAOs = await DataContext.UnitOfMeasure.ToListAsync();
            // khởi tạo khung dữ liệu
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores)
                {
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Select(x => x.Id).ToList();
                    Store.StoreStatus = StoreStatusEnum.StoreStatusEnumList.Where(x => x.Id == Store.StoreStatusId).Select(x => new ReportSalesOrderByStoreAndItem_StoreStatusDTO
                    {
                        Name = x.Name
                    }).FirstOrDefault();
                    Store.Items = new List<ReportSalesOrderByStoreAndItem_ItemDTO>();
                    foreach (IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO in IndirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderContentDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByStoreAndItem_ItemDTO ReportSalesOrderByStoreAndItem_ItemDTO = Store.Items.Where(i => i.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportSalesOrderByStoreAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportSalesOrderByStoreAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
                            ReportSalesOrderByStoreAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByStoreAndItem_ItemDTO.SalePriceAverage += (IndirectSalesOrderContentDAO.PrimaryPrice * IndirectSalesOrderContentDAO.RequestedQuantity);
                            ReportSalesOrderByStoreAndItem_ItemDTO.Revenue += (IndirectSalesOrderContentDAO.Amount - (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                            ReportSalesOrderByStoreAndItem_ItemDTO.Discount += ((IndirectSalesOrderContentDAO.DiscountAmount ?? 0) + (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                        }
                    }

                    foreach (var item in Store.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO in IndirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByStoreAndItem_ItemDTO ReportSalesOrderByStoreAndItem_ItemDTO = Store.Items.Where(i => i.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                            if (ReportSalesOrderByStoreAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportSalesOrderByStoreAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId);
                            ReportSalesOrderByStoreAndItem_ItemDTO.PromotionStock += IndirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            }

            //làm tròn số
            foreach (var ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores)
                {
                    foreach (var item in Store.Items)
                    {
                        item.Revenue = Math.Round(item.Revenue, 0);
                        item.Discount = Math.Round(item.Discount, 0);
                        item.SalePriceAverage = Math.Round(item.SalePriceAverage, 0);
                    }
                }
            }

            return ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs;
        }

        private async Task<ReportSalesOrderByStoreAndItem_TotalDTO> TotalData(
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;
            List<long> ItemIds = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.ItemId?.In;

            ReportSalesOrderByStoreAndItem_TotalDTO ReportSalesOrderByStoreAndItem_TotalDTO = new ReportSalesOrderByStoreAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            var orderQuery = from i in DataContext.IndirectSalesOrder
                             join s in DataContext.Store on i.BuyerStoreId equals s.Id
                             join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                             where i.OrderDate >= Start && i.OrderDate <= End &&
                             AppUserIds.Contains(i.SaleEmployeeId) &&
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
                             (StoreStatusId.HasValue == false || StoreStatusId.Value == StoreStatusEnum.ALL.Id || s.StoreStatusId == StoreStatusId.Value) &&
                             (OrganizationIds.Contains(s.OrganizationId)) &&
                             i.RequestStateId == RequestStateEnum.APPROVED.Id &&
                             s.DeletedAt == null
                             select i.Id;

            var Ids = await orderQuery.ToListAsync();
            ITempTableQuery<TempTable<long>> tempTableQuery2 = await DataContext
                       .BulkInsertValuesIntoTempTableAsync<long>(Ids);

            var transactionQuery = from t in DataContext.IndirectSalesOrderTransaction
                                   join tt in tempTableQuery2.Query on t.IndirectSalesOrderId equals tt.Column1
                                   join s in DataContext.Store on t.BuyerStoreId equals s.Id
                                   where (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(t.ItemId))
                                   select new Store
                                   {
                                       Id = s.Id,
                                       Code = s.Code,
                                       CodeDraft = s.CodeDraft,
                                       Name = s.Name,
                                       Address = s.Address,
                                       OrganizationId = s.OrganizationId,
                                       StoreStatusId = s.StoreStatusId
                                   };

            List<Store> Stores = await transactionQuery.Distinct()
                .OrderBy(x => x.OrganizationId)
                .ThenBy(x => x.Name)
                .ToListAsync();

            OrganizationIds = Stores.Select(x => x.OrganizationId).Distinct().ToList();
            OrganizationDAOs = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).ToListAsync();

            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = OrganizationDAOs.Select(on => new ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO
            {
                OrganizationId = on.Id,
                OrganizationName = on.Name,
            }).ToList();
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores = Stores
                    .Where(x => x.OrganizationId == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.OrganizationId)
                    .Select(x => new ReportSalesOrderByStoreAndItem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        CodeDraft = x.CodeDraft,
                        Name = x.Name,
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();
            var IndirectSalesOrderContentQuery = DataContext.IndirectSalesOrderContent
                .Where(x => StoreIds.Contains(x.IndirectSalesOrder.BuyerStoreId) &&
                AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) &&
                OrganizationIds.Contains(x.IndirectSalesOrder.OrganizationId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)) &&
                Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End &&
                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);
            var IndirectSalesOrderPromotionQuery = DataContext.IndirectSalesOrderPromotion
                .Where(x => StoreIds.Contains(x.IndirectSalesOrder.BuyerStoreId) &&
                AppUserIds.Contains(x.IndirectSalesOrder.SaleEmployeeId) &&
                OrganizationIds.Contains(x.IndirectSalesOrder.OrganizationId) &&
                (ItemIds == null || ItemIds.Count == 0 || ItemIds.Contains(x.ItemId)) &&
                Start <= x.IndirectSalesOrder.OrderDate && x.IndirectSalesOrder.OrderDate <= End &&
                x.IndirectSalesOrder.RequestStateId == RequestStateEnum.APPROVED.Id);

            ReportSalesOrderByStoreAndItem_TotalDTO.TotalSalesStock = IndirectSalesOrderContentQuery.Select(x => x.RequestedQuantity).Sum();

            ReportSalesOrderByStoreAndItem_TotalDTO.TotalPromotionStock = IndirectSalesOrderPromotionQuery.Select(x => x.RequestedQuantity).Sum();

            ReportSalesOrderByStoreAndItem_TotalDTO.TotalRevenue = Math.Round(IndirectSalesOrderContentQuery.Select(x => x.Amount).Sum()
                - IndirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum(), 0);

            ReportSalesOrderByStoreAndItem_TotalDTO.TotalDiscount = Math.Round(IndirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + IndirectSalesOrderContentQuery.Where(x => x.DiscountAmount.HasValue).Select(x => x.DiscountAmount.Value).Sum(), 0);
            return ReportSalesOrderByStoreAndItem_TotalDTO;
        }
    }
}
