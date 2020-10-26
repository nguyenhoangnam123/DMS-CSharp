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
using DMS.Services.MStoreStatus;
using Thinktecture.EntityFrameworkCore.TempTables;
using Thinktecture;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_by_store_and_item
{
    public class ReportDirectSalesOrderByStoreAndItemController : RpcController
    {
        private DataContext DataContext;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private IStoreStatusService StoreStatusService;
        private ICurrentContext CurrentContext;
        public ReportDirectSalesOrderByStoreAndItemController(
            DataContext DataContext,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            IStoreStatusService StoreStatusService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.ItemService = ItemService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.StoreStatusService = StoreStatusService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportDirectSalesOrderByStoreAndItem_OrganizationDTO>> FilterListOrganization()
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

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);
            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ReportDirectSalesOrderByStoreAndItem_OrganizationDTO> StoreCheckerReport_OrganizationDTOs = Organizations
                .Select(x => new ReportDirectSalesOrderByStoreAndItem_OrganizationDTO(x)).ToList();
            return StoreCheckerReport_OrganizationDTOs;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.FilterListStore), HttpPost]
        public async Task<List<ReportDirectSalesOrderByStoreAndItem_StoreDTO>> FilterListStore([FromBody] ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.Id;
            StoreFilter.Code = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.Code;
            StoreFilter.CodeDraft = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.CodeDraft;
            StoreFilter.Name = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.Name;
            StoreFilter.OrganizationId = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportDirectSalesOrderByStoreAndItem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreFilter.Id == null) StoreFilter.Id = new IdFilter();
            StoreFilter.Id.In = await FilterStore(StoreService, OrganizationService, CurrentContext);
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ReportDirectSalesOrderByStoreAndItem_StoreDTO> ReportDirectSalesOrderByStoreAndItem_StoreDTOs = Stores
                .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreDTO(x)).ToList();
            return ReportDirectSalesOrderByStoreAndItem_StoreDTOs;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.FilterListStoreGrouping), HttpPost]
        public async Task<List<ReportDirectSalesOrderByStoreAndItem_StoreGroupingDTO>> FilterListStoreGrouping([FromBody] ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ReportDirectSalesOrderByStoreAndItem_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreGroupingFilter.Id == null) StoreGroupingFilter.Id = new IdFilter();
            StoreGroupingFilter.Id.In = await FilterStoreGrouping(StoreGroupingService, CurrentContext);
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ReportDirectSalesOrderByStoreAndItem_StoreGroupingDTO> ReportDirectSalesOrderByStoreAndItem_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreGroupingDTO(x)).ToList();
            return ReportDirectSalesOrderByStoreAndItem_StoreGroupingDTOs;
        }
        [Route(ReportDirectSalesOrderByStoreAndItemRoute.FilterListStoreType), HttpPost]
        public async Task<List<ReportDirectSalesOrderByStoreAndItem_StoreTypeDTO>> FilterListStoreType([FromBody] ReportDirectSalesOrderByStoreAndItem_StoreTypeFilterDTO ReportDirectSalesOrderByStoreAndItem_StoreTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ReportDirectSalesOrderByStoreAndItem_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ReportDirectSalesOrderByStoreAndItem_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ReportDirectSalesOrderByStoreAndItem_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            if (StoreTypeFilter.Id == null) StoreTypeFilter.Id = new IdFilter();
            StoreTypeFilter.Id.In = await FilterStoreType(StoreTypeService, CurrentContext);
            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportDirectSalesOrderByStoreAndItem_StoreTypeDTO> ReportDirectSalesOrderByStoreAndItem_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreTypeDTO(x)).ToList();
            return ReportDirectSalesOrderByStoreAndItem_StoreTypeDTOs;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.FilterListStoreStatus), HttpPost]
        public async Task<List<ReportDirectSalesOrderByStoreAndItem_StoreStatusDTO>> FilterListStoreStatus([FromBody] ReportDirectSalesOrderByStoreAndItem_StoreStatusFilterDTO ReportDirectSalesOrderByStoreAndItem_StoreStatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StoreStatusFilter StoreStatusFilter = new StoreStatusFilter();
            StoreStatusFilter.Skip = 0;
            StoreStatusFilter.Take = 20;
            StoreStatusFilter.OrderBy = StoreStatusOrder.Id;
            StoreStatusFilter.OrderType = OrderType.ASC;
            StoreStatusFilter.Selects = StoreStatusSelect.ALL;
            StoreStatusFilter.Id = ReportDirectSalesOrderByStoreAndItem_StoreStatusFilterDTO.Id;
            StoreStatusFilter.Code = ReportDirectSalesOrderByStoreAndItem_StoreStatusFilterDTO.Code;
            StoreStatusFilter.Name = ReportDirectSalesOrderByStoreAndItem_StoreStatusFilterDTO.Name;

            List<StoreStatus> StoreStatuses = await StoreStatusService.List(StoreStatusFilter);
            List<ReportDirectSalesOrderByStoreAndItem_StoreStatusDTO> ReportDirectSalesOrderByStoreAndItem_StoreStatusDTOs = StoreStatuses
                .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreStatusDTO(x)).ToList();
            return ReportDirectSalesOrderByStoreAndItem_StoreStatusDTOs;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? StoreId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

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
            var query = from i in DataContext.DirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                        where i.OrderDate >= Start && i.OrderDate <= End &&
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
                        select s;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO>>> List([FromBody] ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return new List<ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO>();

            DateTime Start = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            long? StoreId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
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

            var query = from i in DataContext.DirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                        where i.OrderDate >= Start && i.OrderDate <= End &&
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

            List<Store> Stores = await query.Distinct().OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
                .Skip(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Skip)
                .Take(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Take)
                .ToListAsync();

            OrganizationIds = Stores.Select(x => x.OrganizationId).Distinct().ToList();
            OrganizationDAOs = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).ToListAsync();

            List<ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO> ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs = OrganizationDAOs.Select(on => new ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO
            {
                OrganizationId = on.Id,
                OrganizationName = on.Name,
            }).ToList();
            foreach (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs)
            {
                ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.Stores = Stores
                    .Where(x => x.OrganizationId == ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.OrganizationId)
                    .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreDTO
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
            List<DirectSalesOrderDAO> DirectSalesOrderDAOs = await DataContext.DirectSalesOrder
                .Where(x => StoreIds.Contains(x.BuyerStoreId) && Start <= x.OrderDate && x.OrderDate <= End &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .Select(x => new DirectSalesOrderDAO
                {
                    Id = x.Id,
                    BuyerStoreId = x.BuyerStoreId
                }).ToListAsync();
            List<long> DirectSalesOrderIds = DirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<DirectSalesOrderContentDAO> DirectSalesOrderContentDAOs = await DataContext.DirectSalesOrderContent
                .Where(x => StoreIds.Contains(x.DirectSalesOrder.BuyerStoreId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End)
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
                .Where(x => StoreIds.Contains(x.DirectSalesOrder.BuyerStoreId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End)
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
            List<long> ItemIds = new List<long>();
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
            // khởi tạo khung dữ liệu
            foreach (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.Stores)
                {
                    var SalesOrderIds = DirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Select(x => x.Id).ToList();
                    Store.StoreStatus = StoreStatusEnum.StoreStatusEnumList.Where(x => x.Id == Store.StoreStatusId).Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreStatusDTO
                    {
                        Name = x.Name
                    }).FirstOrDefault();
                    Store.Items = new List<ReportDirectSalesOrderByStoreAndItem_ItemDTO>();
                    foreach (DirectSalesOrderContentDAO DirectSalesOrderContentDAO in DirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderContentDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO ReportDirectSalesOrderByStoreAndItem_ItemDTO = Store.Items.Where(i => i.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByStoreAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByStoreAndItem_ItemDTO = new ReportDirectSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportDirectSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderContentDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.SaleStock += DirectSalesOrderContentDAO.RequestedQuantity;
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.SalePriceAverage += (DirectSalesOrderContentDAO.SalePrice * DirectSalesOrderContentDAO.RequestedQuantity);
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.Revenue += (DirectSalesOrderContentDAO.Amount - (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (DirectSalesOrderContentDAO.TaxAmount ?? 0));
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.Discount += ((DirectSalesOrderContentDAO.DiscountAmount ?? 0) + (DirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0));
                        }
                    }

                    foreach (var item in Store.Items)
                    {
                        item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
                    }

                    foreach (DirectSalesOrderPromotionDAO DirectSalesOrderPromotionDAO in DirectSalesOrderPromotionDAOs)
                    {
                        if (SalesOrderIds.Contains(DirectSalesOrderPromotionDAO.DirectSalesOrderId))
                        {
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO ReportDirectSalesOrderByStoreAndItem_ItemDTO = Store.Items.Where(i => i.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                            if (ReportDirectSalesOrderByStoreAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == DirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
                                if (item == null)
                                    continue;
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == DirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportDirectSalesOrderByStoreAndItem_ItemDTO = new ReportDirectSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    DirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportDirectSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.DirectSalesOrderIds.Add(DirectSalesOrderPromotionDAO.DirectSalesOrderId);
                            ReportDirectSalesOrderByStoreAndItem_ItemDTO.PromotionStock += DirectSalesOrderPromotionDAO.RequestedQuantity;
                        }
                    }
                }
            }

            //làm tròn số
            foreach (var ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.Stores)
                {
                    foreach (var item in Store.Items)
                    {
                        item.Revenue = Math.Round(item.Revenue, 0);
                        item.Discount = Math.Round(item.Discount, 0);
                        item.SalePriceAverage = Math.Round(item.SalePriceAverage, 0);
                    }
                }
            }

            ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.Any()).ToList();
            return ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.Any()).ToList();
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.Total), HttpPost]
        public async Task<ReportDirectSalesOrderByStoreAndItem_TotalDTO> Total([FromBody] ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return new ReportDirectSalesOrderByStoreAndItem_TotalDTO();

            DateTime Start = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportDirectSalesOrderByStoreAndItem_TotalDTO();

            long? StoreId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;
            long? StoreStatusId = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.StoreStatusId?.Equal;

            ReportDirectSalesOrderByStoreAndItem_TotalDTO ReportDirectSalesOrderByStoreAndItem_TotalDTO = new ReportDirectSalesOrderByStoreAndItem_TotalDTO();
            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
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

            var query = from i in DataContext.DirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join tt in tempTableQuery.Query on i.BuyerStoreId equals tt.Column1
                        where i.OrderDate >= Start && i.OrderDate <= End &&
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
                        select new Store
                        {
                            Id = s.Id,
                            Code = s.Code,
                            CodeDraft = s.CodeDraft,
                            Name = s.Name,
                            Address = s.Address,
                            OrganizationId = s.OrganizationId,
                        };

            List<Store> Stores = await query.Distinct().OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
                .Skip(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Skip)
                .Take(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Take)
                .ToListAsync();

            OrganizationIds = Stores.Select(x => x.OrganizationId).Distinct().ToList();
            OrganizationDAOs = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).ToListAsync();

            List<ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO> ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs = OrganizationDAOs.Select(on => new ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO
            {
                OrganizationId = on.Id,
                OrganizationName = on.Name,
            }).ToList();
            foreach (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs)
            {
                ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.Stores = Stores
                    .Where(x => x.OrganizationId == ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.OrganizationId)
                    .Select(x => new ReportDirectSalesOrderByStoreAndItem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        CodeDraft = x.CodeDraft,
                        Name = x.Name,
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();
            var DirectSalesOrderContentQuery = DataContext.DirectSalesOrderContent
                .Where(x => StoreIds.Contains(x.DirectSalesOrder.BuyerStoreId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End);
            var DirectSalesOrderPromotionQuery = DataContext.DirectSalesOrderPromotion
                .Where(x => StoreIds.Contains(x.DirectSalesOrder.BuyerStoreId) && Start <= x.DirectSalesOrder.OrderDate && x.DirectSalesOrder.OrderDate <= End);

            ReportDirectSalesOrderByStoreAndItem_TotalDTO.TotalSalesStock = DirectSalesOrderContentQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByStoreAndItem_TotalDTO.TotalPromotionStock = DirectSalesOrderPromotionQuery.Select(x => x.RequestedQuantity).Sum();

            ReportDirectSalesOrderByStoreAndItem_TotalDTO.TotalRevenue = Math.Round(DirectSalesOrderContentQuery.Select(x => x.Amount).Sum()
                - DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.TaxAmount.HasValue).Select(x => x.TaxAmount.Value).Sum(), 0);

            ReportDirectSalesOrderByStoreAndItem_TotalDTO.TotalDiscount = Math.Round(DirectSalesOrderContentQuery.Where(x => x.GeneralDiscountAmount.HasValue).Select(x => x.GeneralDiscountAmount.Value).Sum()
                + DirectSalesOrderContentQuery.Where(x => x.DiscountAmount.HasValue).Select(x => x.DiscountAmount.Value).Sum(), 0);
            return ReportDirectSalesOrderByStoreAndItem_TotalDTO;
        }

        [Route(ReportDirectSalesOrderByStoreAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Skip = 0;
            ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO.Take = int.MaxValue;
            List<ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO> ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs = (await List(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO)).Value;

            ReportDirectSalesOrderByStoreAndItem_TotalDTO ReportDirectSalesOrderByStoreAndItem_TotalDTO = await Total(ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemFilterDTO);
            long stt = 1;
            foreach (ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTO.Stores)
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
            Data.ReportSalesOrderByStoreAndItems = ReportDirectSalesOrderByStoreAndItem_ReportDirectSalesOrderByStoreAndItemDTOs;
            Data.Total = ReportDirectSalesOrderByStoreAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportDirectSalesOrderByStoreAndItem.xlsx");
        }
    }
}
