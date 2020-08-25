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
using System.IO;
using System.Dynamic;
using NGS.Templater;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

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

        [Route(ReportSalesOrderByStoreAndItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

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

            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (StoreIds.Contains(i.BuyerStoreId)) &&
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
                        OrganizationIds.Contains(s.OrganizationId)
                        select s;

            int count = await query.Distinct().CountAsync();
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
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest("Chỉ được phép xem tối đa trong vòng 31 ngày");

            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
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
            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (StoreIds.Contains(i.BuyerStoreId)) &&
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
                        (OrganizationIds.Contains(s.OrganizationId))
                        select new Store
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Address = s.Address,
                            OrganizationId = s.OrganizationId,
                            Organization = new Organization
                            {
                                Id = o.Id,
                                Code = o.Code,
                                Name = o.Name,
                            }

                        };

            List<Store> Stores = await query.Distinct().ToListAsync();

            Stores = Stores.OrderBy(x => x.Name)
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.Name)
                .Skip(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Skip)
                .Take(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Take)
                .ToList();
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

            StoreIds = Stores.Select(s => s.Id).ToList();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => StoreIds.Contains(x.BuyerStoreId) && Start <= x.OrderDate && x.OrderDate <= End)
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
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Select(x => x.Id).ToList();
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
                            ReportSalesOrderByStoreAndItem_ItemDTO.SalePriceAverage += (IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity);
                            ReportSalesOrderByStoreAndItem_ItemDTO.Revenue += (IndirectSalesOrderContentDAO.Amount - (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (IndirectSalesOrderContentDAO.TaxAmount ?? 0));
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

            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.Any()).ToList();
            return ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.Where(x => x.Stores.Any()).ToList();
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Total), HttpPost]
        public async Task<ReportSalesOrderByStoreAndItem_TotalDTO> Total([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.HasValue == false)
                return new ReportSalesOrderByStoreAndItem_TotalDTO();

            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
                   StaticParams.DateTimeNow :
                   ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return new ReportSalesOrderByStoreAndItem_TotalDTO();

            long? StoreId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId?.Equal;
            long? StoreTypeId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId?.Equal;
            long? StoreGroupingId = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId?.Equal;

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

            var query = from i in DataContext.IndirectSalesOrder
                        join s in DataContext.Store on i.BuyerStoreId equals s.Id
                        join o in DataContext.Organization on s.OrganizationId equals o.Id
                        where i.OrderDate >= Start && i.OrderDate <= End &&
                        (StoreIds.Contains(i.BuyerStoreId)) &&
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
                        (OrganizationIds.Contains(s.OrganizationId))
                        select new Store
                        {
                            Id = s.Id,
                            Code = s.Code,
                            Name = s.Name,
                            Address = s.Address,
                            OrganizationId = s.OrganizationId,
                            Organization = new Organization
                            {
                                Id = o.Id,
                                Code = o.Code,
                                Name = o.Name,
                            }
                        };

            List<Store> Stores = await query.Distinct().ToListAsync();

            Stores = Stores.OrderBy(x => x.Name).ToList();

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
                    })
                    .ToList();
            }

            StoreIds = Stores.Select(s => s.Id).ToList();
            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => StoreIds.Contains(x.BuyerStoreId) && Start <= x.OrderDate && x.OrderDate <= End)
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

            // khởi tạo khung dữ liệu
            foreach (ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs)
            {
                foreach (var Store in ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO.Stores)
                {
                    var SalesOrderIds = IndirectSalesOrderDAOs.Where(x => x.BuyerStoreId == Store.Id).Select(x => x.Id).ToList();
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
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
                                    IndirectSalesOrderIds = new HashSet<long>(),
                                };
                                Store.Items.Add(ReportSalesOrderByStoreAndItem_ItemDTO);
                            }
                            ReportSalesOrderByStoreAndItem_ItemDTO.IndirectSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
                            ReportSalesOrderByStoreAndItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
                            ReportSalesOrderByStoreAndItem_ItemDTO.SalePriceAverage += (IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity);
                            ReportSalesOrderByStoreAndItem_ItemDTO.Revenue += (IndirectSalesOrderContentDAO.Amount - (IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0) + (IndirectSalesOrderContentDAO.TaxAmount ?? 0));
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
                                ReportSalesOrderByStoreAndItem_ItemDTO = new ReportSalesOrderByStoreAndItem_ItemDTO
                                {
                                    Id = item.Id,
                                    Code = item.Code,
                                    Name = item.Name,
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

            ReportSalesOrderByStoreAndItem_TotalDTO.TotalDiscount = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Items).Sum(x => x.Discount);
            ReportSalesOrderByStoreAndItem_TotalDTO.TotalRevenue = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Items).Sum(x => x.Revenue);
            ReportSalesOrderByStoreAndItem_TotalDTO.TotalPromotionStock = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Items).Sum(x => x.PromotionStock);
            ReportSalesOrderByStoreAndItem_TotalDTO.TotalSalesStock = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs.SelectMany(x => x.Stores)
                .SelectMany(x => x.Items).Sum(x => x.SaleStock);
            return ReportSalesOrderByStoreAndItem_TotalDTO;
        }

        [Route(ReportSalesOrderByStoreAndItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.GreaterEqual == null ?
               StaticParams.DateTimeNow.Date :
               ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.GreaterEqual.Value.Date;

            DateTime End = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrderDate.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return BadRequest("Chỉ được phép xem tối đa trong vòng 31 ngày");

            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Skip = 0;
            ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.Take = int.MaxValue;
            List<ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTO> ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs = (await List(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO)).Value;

            ReportSalesOrderByStoreAndItem_TotalDTO ReportSalesOrderByStoreAndItem_TotalDTO = await Total(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO);
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
            Data.Start = Start.ToString("dd-MM-yyyy");
            Data.End = End.ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByStoreAndItems = ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemDTOs;
            Data.Total = ReportSalesOrderByStoreAndItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportSalesOrderByStoreAndItem.xlsx");
        }
    }
}
