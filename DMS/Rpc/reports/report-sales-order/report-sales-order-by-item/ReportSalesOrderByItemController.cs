using Common;
using DMS.Models;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using Microsoft.AspNetCore.Mvc;
using System;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Enums;
using Helpers;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MOrganization;
using Microsoft.EntityFrameworkCore;
using DMS.Repositories;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_item
{
    public class ReportSalesOrderByItemController : RpcController
    {
        private DataContext DataContext;
        private IOrganizationService OrganizationService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IItemService ItemService;
        private IProductService ProductService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ReportSalesOrderByItemController(
            DataContext DataContext,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IProductService ProductService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.OrganizationService = OrganizationService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReportSalesOrderByItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<ReportSalesOrderByItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] ReportSalesOrderByItem_ProductGroupingFilterDTO ReportSalesOrderByItem_ProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;
            ProductGroupingFilter.Code = ReportSalesOrderByItem_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ReportSalesOrderByItem_ProductGroupingFilterDTO.Name;

            List<ProductGrouping> ReportSalesOrderByItemGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ReportSalesOrderByItem_ProductGroupingDTO> ReportSalesOrderByItem_ProductGroupingDTOs = ReportSalesOrderByItemGroupings
                .Select(x => new ReportSalesOrderByItem_ProductGroupingDTO(x)).ToList();
            return ReportSalesOrderByItem_ProductGroupingDTOs;
        }

        [Route(ReportSalesOrderByItemRoute.FilterListProductType), HttpPost]
        public async Task<List<ReportSalesOrderByItem_ProductTypeDTO>> FilterListProductType([FromBody] ReportSalesOrderByItem_ProductTypeFilterDTO ReportSalesOrderByItem_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = ReportSalesOrderByItem_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ReportSalesOrderByItem_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ReportSalesOrderByItem_ProductTypeFilterDTO.Name;
            ProductTypeFilter.StatusId = ReportSalesOrderByItem_ProductTypeFilterDTO.StatusId;

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ReportSalesOrderByItem_ProductTypeDTO> ReportSalesOrderByItem_ProductTypeDTOs = ProductTypes
                .Select(x => new ReportSalesOrderByItem_ProductTypeDTO(x)).ToList();
            return ReportSalesOrderByItem_ProductTypeDTOs;
        }

        [Route(ReportSalesOrderByItemRoute.FilterListItem), HttpPost]
        public async Task<List<ReportSalesOrderByItem_ItemDTO>> FilterListItem([FromBody] ReportSalesOrderByItem_ItemFilterDTO ReportSalesOrderByItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportSalesOrderByItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportSalesOrderByItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportSalesOrderByItem_ItemFilterDTO.Name;
            ItemFilter.StatusId = ReportSalesOrderByItem_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportSalesOrderByItem_ItemDTO> ReportSalesOrderByItem_ItemDTOs = Items
                .Select(x => new ReportSalesOrderByItem_ItemDTO(x)).ToList();
            return ReportSalesOrderByItem_ItemDTOs;
        }

        [Route(ReportSalesOrderByItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date  ?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? ItemId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<long> AppUserIds = await DataContext.AppUser.Where(au =>
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
                .Select(x => x.Id)
                .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= Start && x.OrderDate <= End &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
                .ToListAsync();
            
            List<long> IndirectSalesOrderIds = IndirectSalesOrderDAOs.Select(x => x.Id).ToList();
            List<long> itemSales = await DataContext.IndirectSalesOrderContent
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .Select(x=> x.ItemId)
                .ToListAsync();
            List<long> itemPromotion = await DataContext.IndirectSalesOrderPromotion
                .Where(x => IndirectSalesOrderIds.Contains(x.IndirectSalesOrderId))
                .Select(x => x.ItemId)
                .ToListAsync();
            List<long> ItemIds = new List<long>();
            ItemIds.AddRange(itemSales);
            ItemIds.AddRange(itemPromotion);
            ItemIds = ItemIds.Distinct().ToList();

            var query = from i in DataContext.Item
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId
                        where ItemIds.Contains(i.Id) &&
                        (ItemId.HasValue == false || i.Id == ItemId.Value) &&
                        (ProductTypeId.HasValue == false || p.ProductTypeId == ProductTypeId.Value) &&
                        (ProductGroupingId.HasValue == false || ppgm.ProductGroupingId == ProductGroupingId.Value)
                        select i;

            return await query.CountAsync();
        }

        [Route(ReportSalesOrderByItemRoute.List), HttpPost]
        public async Task<List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO>> List([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);

            long? ItemId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<long> AppUserIds = await DataContext.AppUser.Where(au =>
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
                .Select(x => x.Id)
                .ToListAsync();

            List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder
                .Where(x => AppUserIds.Contains(x.SaleEmployeeId) &&
                x.OrderDate >= Start && x.OrderDate <= End &&
                x.RequestStateId == RequestStateEnum.APPROVED.Id)
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
            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = ItemIds },
                ProductGroupingId = new IdFilter { Equal = ProductGroupingId },
                ProductTypeId = new IdFilter { Equal = ProductTypeId },
                Selects = ItemSelect.Id | ItemSelect.Code | ItemSelect.Name | ItemSelect.SalePrice
            };
            if (ItemId.HasValue)
                ItemFilter.Id = new IdFilter { Equal = ItemId.Value };
            List<Item> Items = await ItemService.List(ItemFilter);

            List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO> ReportSalesOrderByItem_ReportSalesOrderByItemDTOs = new List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO>();
            //foreach (IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO in IndirectSalesOrderContentDAOs)
            //{
            //    if (IndirectSalesOrderIds.Contains(IndirectSalesOrderContentDAO.IndirectSalesOrderId))
            //    {
            //        ReportSalesOrderByItem_ReportSalesOrderByItemDTO ReportSalesOrderByItem_ReportSalesOrderByItemDTO = ReportSalesOrderByItem_ReportSalesOrderByItemDTOs
            //            .Where(i => i.ProductId == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
            //        if (ReportSalesOrderByItem_ItemDTO == null)
            //        {
            //            var item = Items.Where(x => x.Id == IndirectSalesOrderContentDAO.ItemId).FirstOrDefault();
            //            var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
            //            ReportSalesOrderByItem_ItemDTO = new ReportSalesOrderByItem_ItemDTO
            //            {
            //                Code = item.Code,
            //                Name = item.Name,
            //                UnitOfMeaureName = UOMName,
            //                IndirecSalesOrderIds = new HashSet<long>(),
            //            };
            //            Store.Items.Add(ReportSalesOrderByItem_ItemDTO);
            //        }
            //        ReportSalesOrderByItem_ItemDTO.IndirecSalesOrderIds.Add(IndirectSalesOrderContentDAO.IndirectSalesOrderId);
            //        ReportSalesOrderByItem_ItemDTO.SaleStock += IndirectSalesOrderContentDAO.RequestedQuantity;
            //        ReportSalesOrderByItem_ItemDTO.SalePriceAverage += IndirectSalesOrderContentDAO.SalePrice * IndirectSalesOrderContentDAO.RequestedQuantity;
            //        ReportSalesOrderByItem_ItemDTO.Revenue += IndirectSalesOrderContentDAO.Amount;
            //        ReportSalesOrderByItem_ItemDTO.Discount += (IndirectSalesOrderContentDAO.DiscountAmount ?? 0 + IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0);
            //    }
            //}

            //foreach (var item in Store.Items)
            //{
            //    item.SalePriceAverage = item.SalePriceAverage / item.SaleStock;
            //}

            //foreach (IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO in IndirectSalesOrderPromotionDAOs)
            //{
            //    if (SalesOrderIds.Contains(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId))
            //    {
            //        ReportSalesOrderByItem_ItemDTO ReportSalesOrderByItem_ItemDTO = Store.Items.Where(i => i.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
            //        if (ReportSalesOrderByItem_ItemDTO == null)
            //        {
            //            var item = Items.Where(x => x.Id == IndirectSalesOrderPromotionDAO.ItemId).FirstOrDefault();
            //            var UOMName = UnitOfMeasureDAOs.Where(x => x.Id == IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId).Select(x => x.Name).FirstOrDefault();
            //            ReportSalesOrderByItem_ItemDTO = new ReportSalesOrderByItem_ItemDTO
            //            {
            //                Code = item.Code,
            //                Name = item.Name,
            //                UnitOfMeaureName = UOMName,
            //                IndirecSalesOrderIds = new HashSet<long>(),
            //            };
            //            Store.Items.Add(ReportSalesOrderByItem_ItemDTO);
            //        }
            //        ReportSalesOrderByItem_ItemDTO.IndirecSalesOrderIds.Add(IndirectSalesOrderPromotionDAO.IndirectSalesOrderId);
            //        ReportSalesOrderByItem_ItemDTO.PromotionStock += IndirectSalesOrderPromotionDAO.RequestedQuantity;
            //    }
            //}
            return ReportSalesOrderByItem_ReportSalesOrderByItemDTOs;
        }
    }
}
