using Common;
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
using Helpers;
using Microsoft.EntityFrameworkCore;
using DMS.Services.MProduct;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_store_and_item
{
    public class ReportSalesOrderByStoreAndItemController : RpcController
    {
        private DataContext DataContext;
        private IItemService ItemService;
        private IOrganizationService OrganizationService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderByStoreAndItemController(
            DataContext DataContext,
            IItemService ItemService,
            IOrganizationService OrganizationService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.ItemService = ItemService;
            this.OrganizationService = OrganizationService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_OrganizationDTO>> FilterListOrganization([FromBody] ReportSalesOrderByStoreAndItem_OrganizationFilterDTO ReportSalesOrderByStoreAndItem_OrganizationFilterDTO)
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
            OrganizationFilter.Id = ReportSalesOrderByStoreAndItem_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ReportSalesOrderByStoreAndItem_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ReportSalesOrderByStoreAndItem_OrganizationFilterDTO.Name;

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
            StoreFilter.Name = ReportSalesOrderByStoreAndItem_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.StoreGroupingId;
            StoreFilter.StatusId = ReportSalesOrderByStoreAndItem_StoreFilterDTO.StatusId;

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
            StoreGroupingFilter.StatusId = ReportSalesOrderByStoreAndItem_StoreGroupingFilterDTO.StatusId;
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
            StoreTypeFilter.StatusId = ReportSalesOrderByStoreAndItem_StoreTypeFilterDTO.StatusId;

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ReportSalesOrderByStoreAndItem_StoreTypeDTO> ReportSalesOrderByStoreAndItem_StoreTypeDTOs = StoreTypes
                .Select(x => new ReportSalesOrderByStoreAndItem_StoreTypeDTO(x)).ToList();
            return ReportSalesOrderByStoreAndItem_StoreTypeDTOs;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.SellerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (StoreId.HasValue == false || i.SellerStoreId == StoreId.Value) &&
                        (StoreTypeId.HasValue == false || s.StoreTypeId == StoreTypeId.Value) &&
                        (StoreGroupingId.HasValue == false || s.StoreGroupingId == StoreGroupingId.Value) &&
                        OrganizationIds.Contains(s.OrganizationId)
                        select s;

            int count = await query.Distinct().CountAsync();
            return count;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.List), HttpPost]
        public async Task<List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO>> List([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && (OrganizationIds.Count == 0 || OrganizationIds.Contains(o.Id))).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            List<Store> Stores = await StoreService.List(new StoreFilter
            {
                OrganizationId = new IdFilter { In = OrganizationIds },
                StoreTypeId = new IdFilter { Equal = StoreTypeId },
                StoreGroupingId = new IdFilter { Equal = StoreGroupingId },
                Skip = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Skip,
                Take = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Take,
                Selects = StoreSelect.Id | StoreSelect.Code | StoreSelect.Name | StoreSelect.Organization | StoreSelect.StoreGrouping | StoreSelect.StoreType
            });

            List<string> OrganizationNames = Stores.Select(s => s.Organization.Name).Distinct().ToList();
            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = OrganizationNames.Select(on => new ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO
            {
                OrganizationName = on,
            }).ToList();
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores = Stores
                    .Where(x => x.Organization.Name == ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.OrganizationName)
                    .Select(x => new ReportSalesOrderByStoreAndItem_StoreDTO
                    {
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Address = x.Address,
                        OrganizationId = x.OrganizationId,
                        StoreGroupingId = x.StoreGroupingId,
                        StoreTypeId = x.StoreTypeId,
                    })
                    .ToList();
            }
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.Any()).ToList();

            List<long> StoreIds = Stores.Select(s => s.Id).ToList();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => StoreIds.Contains(x.SellerStoreId) && Start <= x.OrderDate && x.OrderDate <= End)
                .ToListAsync();
            List<long> IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .ToListAsync();
            List<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotionDAOs = await DataContext.IndirectSalesOrderPromotion
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .ToListAsync();
            List<long> ItemIds = new List<long>();
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
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.SellerStoreId == Store.Id).Select(x => x.Id).ToList();
                    Store.Items = new List<ReportSalesOrderByStoreAndItem_ItemDTO>();
                    foreach (IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO in IndirectSalesOrderContentDAOs)
                    {
                        if (SalesOrderIds.Contains(IndirectSalesOrderContentDAO.IndirectSalesOrderId))
                        {
                            ReportSalesOrderByStoreAndItem_ItemDTO ReportSalesOrderByStoreAndItem_ItemDTO = Store.Items.Where(i => i.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                            if (ReportSalesOrderByStoreAndItem_ItemDTO == null)
                            {
                                var item = Items.Where(x => x.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Code = item.Code,
                                    Name = item.Name,
                                    UnitOfMeasureName = UOMName,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportSalesOrderByStoreAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
                            ReportSalesOrderByStoreAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByStoreAndItem_ItemDTO.SalePriceAverage += IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByStoreAndItem_ItemDTO.Revenue += IndirectSalesOrderContentDAO.Amount;
                            ReportSalesOrderByStoreAndItem_ItemDTO.Discount += (IndirectSalesOrderContentDAO.DiscountAmount ?? 0 + IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0);
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
                                var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
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
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.All(x => x.Items.Any())).ToList();
            return ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs;
        }
    }
}
