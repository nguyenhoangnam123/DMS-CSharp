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
using System.IO;
using System.Dynamic;
using NGS.Templater;

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

        #region Filter List
        [Route(ReportSalesOrderByItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportSalesOrderByItem_OrganizationDTO>> FilterListOrganization()
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
            List<ReportSalesOrderByItem_OrganizationDTO> ReportSalesOrderByItem_OrganizationDTO = Organizations
                .Select(x => new ReportSalesOrderByItem_OrganizationDTO(x)).ToList();
            return ReportSalesOrderByItem_OrganizationDTO;
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

            if (ProductGroupingFilter.Id == null) ProductGroupingFilter.Id = new IdFilter();
            ProductGroupingFilter.Id.In = await FilterProductGrouping(ProductGroupingService, CurrentContext);
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

            if (ProductTypeFilter.Id == null) ProductTypeFilter.Id = new IdFilter();
            ProductTypeFilter.Id.In = await FilterProductType(ProductTypeService, CurrentContext);
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
        #endregion

        [Route(ReportSalesOrderByItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return 0;

            long? ItemId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from t in DataContext.IndirectSalesOrderTransaction
                        where OrganizationIds.Contains(t.OrganizationId)
                        group t by new {t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            return await query.CountAsync();
        }

        [Route(ReportSalesOrderByItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO>>> List([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.HasValue == false)
                return new List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO>();

            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return BadRequest("Chỉ được phép xem tối đa trong vòng 31 ngày");

            long? ItemId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();

            var query = from t in DataContext.IndirectSalesOrderTransaction
                        where OrganizationIds.Contains(t.OrganizationId)
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.ItemId)
                .Skip(ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Skip)
                .Take(ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Take)
                .ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();
            var OrganizationNames = await DataContext.Organization.Where(x => OrgIds.Contains(x.Id)).OrderBy(x => x.Id).Select(x => x.Name).ToListAsync();
            List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO> ReportSalesOrderByItem_ReportSalesOrderByItemDTOs = new List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO>();
            foreach (var OrganizationName in OrganizationNames)
            {
                ReportSalesOrderByItem_ReportSalesOrderByItemDTO ReportSalesOrderByItem_ReportSalesOrderByItemDTO = new ReportSalesOrderByItem_ReportSalesOrderByItemDTO();
                ReportSalesOrderByItem_ReportSalesOrderByItemDTO.OrganizationName = OrganizationName;
                ReportSalesOrderByItem_ReportSalesOrderByItemDTO.ItemDetails = new List<ReportSalesOrderByItem_ItemDetailDTO>();
                ReportSalesOrderByItem_ReportSalesOrderByItemDTOs.Add(ReportSalesOrderByItem_ReportSalesOrderByItemDTO);
            }

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.IndirectSalesOrderTransaction
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join p in DataContext.Product on i.ProductId equals p.Id
                                   join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId
                                   join ind in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   (ProductTypeId.HasValue == false || p.ProductTypeId == ProductTypeId) &&
                                   (ProductGroupingId.HasValue == false || ppgm.ProductGroupingId == ProductGroupingId)
                                   select new IndirectSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       IndirectSalesOrderId = t.IndirectSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       IndirectSalesOrder = new IndirectSalesOrderDAO
                                       {
                                           BuyerStoreId = ind.BuyerStoreId
                                       },
                                       Item = new ItemDAO
                                       {
                                           Code = i.Code,
                                           Name = i.Name,
                                       },
                                       Organization = new OrganizationDAO
                                       {
                                           Name = o.Name
                                       },
                                       UnitOfMeasure = new UnitOfMeasureDAO
                                       {
                                           Name = u.Name
                                       }
                                   };
            var IndirectSalesOrderTransactions = await queryTransaction.ToListAsync();

            foreach (var ReportSalesOrderByItem_ReportSalesOrderByItemDTO in ReportSalesOrderByItem_ReportSalesOrderByItemDTOs)
            {
                var Transactions = IndirectSalesOrderTransactions.Where(x => x.Organization.Name == ReportSalesOrderByItem_ReportSalesOrderByItemDTO.OrganizationName);
                foreach (var Transaction in Transactions)
                {
                    var ItemDetail = ReportSalesOrderByItem_ReportSalesOrderByItemDTO.ItemDetails.Where(x => x.ItemId == Transaction.ItemId).FirstOrDefault();
                    if(ItemDetail == null)
                    {
                        ItemDetail = new ReportSalesOrderByItem_ItemDetailDTO();
                        ItemDetail.ItemCode = Transaction.Item.Code;
                        ItemDetail.ItemName = Transaction.Item.Name;
                        ItemDetail.UnitOfMeasureName = Transaction.UnitOfMeasure.Name;
                        ItemDetail.IndirectSalesOrderIds = new HashSet<long>();
                        ItemDetail.BuyerStoreIds = new HashSet<long>();
                        ReportSalesOrderByItem_ReportSalesOrderByItemDTO.ItemDetails.Add(ItemDetail);
                    }
                    if(Transaction.TypeId == IndirectSalesOrderTransactionTypeEnum.SALES_CONTENT.Id)
                    {
                        ItemDetail.SaleStock += Transaction.Quantity;
                    }
                    if (Transaction.TypeId == IndirectSalesOrderTransactionTypeEnum.PROMOTION.Id)
                    {
                        ItemDetail.SaleStock += Transaction.Quantity;
                    }
                    ItemDetail.Discount += Transaction.Discount ?? 0;
                    ItemDetail.Revenue += Transaction.Revenue ?? 0;
                    ItemDetail.IndirectSalesOrderIds.Add(Transaction.IndirectSalesOrderId);
                    ItemDetail.BuyerStoreIds.Add(Transaction.IndirectSalesOrder.BuyerStoreId);
                }
            }
            //làm tròn số
            foreach (var ReportSalesOrderByItem_ReportSalesOrderByItemDTO in ReportSalesOrderByItem_ReportSalesOrderByItemDTOs)
            {
                foreach (var item in ReportSalesOrderByItem_ReportSalesOrderByItemDTO.ItemDetails)
                {
                    item.Discount = Math.Round(item.Discount, 0);
                    item.Revenue = Math.Round(item.Revenue, 0);
                }
            }

            return ReportSalesOrderByItem_ReportSalesOrderByItemDTOs;
        }

        [Route(ReportSalesOrderByItemRoute.Total), HttpPost]
        public async Task<ReportSalesOrderByItem_TotalDTO> Total([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.HasValue == false)
                return new ReportSalesOrderByItem_TotalDTO();

            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                   StaticParams.DateTimeNow :
                   ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            long? ItemId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            Start = new DateTime(Start.Year, Start.Month, Start.Day);
            End = (new DateTime(End.Year, End.Month, End.Day)).AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return new ReportSalesOrderByItem_TotalDTO();

            ReportSalesOrderByItem_TotalDTO ReportSalesOrderByItem_TotalDTO = new ReportSalesOrderByItem_TotalDTO();

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
                OrganizationDAOs = OrganizationDAOs.Where(o => o.Path.StartsWith(OrganizationDAO.Path)).ToList();
            }
            OrganizationIds = OrganizationDAOs.Select(o => o.Id).ToList();
            List<long> AppUserIds = await DataContext.AppUser.Where(au =>
               au.OrganizationId.HasValue && OrganizationIds.Contains(au.OrganizationId.Value))
                .Select(x => x.Id)
                .ToListAsync();

            var query = from t in DataContext.IndirectSalesOrderTransaction
                        where OrganizationIds.Contains(t.OrganizationId)
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query.ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.IndirectSalesOrderTransaction
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join p in DataContext.Product on i.ProductId equals p.Id
                                   join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId
                                   join ind in DataContext.IndirectSalesOrder on t.IndirectSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   (ProductTypeId.HasValue == false || p.ProductTypeId == ProductTypeId) &&
                                   (ProductGroupingId.HasValue == false || ppgm.ProductGroupingId == ProductGroupingId)
                                   select new IndirectSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       IndirectSalesOrderId = t.IndirectSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       IndirectSalesOrder = new IndirectSalesOrderDAO
                                       {
                                           BuyerStoreId = ind.BuyerStoreId
                                       },
                                       Item = new ItemDAO
                                       {
                                           Code = i.Code,
                                           Name = i.Name,
                                       },
                                       Organization = new OrganizationDAO
                                       {
                                           Name = o.Name
                                       },
                                       UnitOfMeasure = new UnitOfMeasureDAO
                                       {
                                           Name = u.Name
                                       }
                                   };
            var IndirectSalesOrderTransactions = await queryTransaction.ToListAsync();

            ReportSalesOrderByItem_TotalDTO.TotalDiscount = IndirectSalesOrderTransactions
                .Where(x => x.Discount.HasValue)
                .Select(x => x.Discount.Value)
                .DefaultIfEmpty(0)
                .Sum();
            ReportSalesOrderByItem_TotalDTO.TotalRevenue = IndirectSalesOrderTransactions
                .Where(x => x.Revenue.HasValue)
                .Select(x => x.Revenue.Value)
                .DefaultIfEmpty(0)
                .Sum();
            ReportSalesOrderByItem_TotalDTO.TotalPromotionStock = IndirectSalesOrderTransactions
                .Where(x => x.TypeId == IndirectSalesOrderTransactionTypeEnum.PROMOTION.Id)
                .Select(x => x.Quantity)
                .Sum();
            ReportSalesOrderByItem_TotalDTO.TotalSalesStock = IndirectSalesOrderTransactions
                .Where(x => x.TypeId == IndirectSalesOrderTransactionTypeEnum.SALES_CONTENT.Id)
                .Select(x => x.Quantity)
                .Sum();

            return ReportSalesOrderByItem_TotalDTO;
        }

        [Route(ReportSalesOrderByItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DateTime Start = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
               StaticParams.DateTimeNow.Date :
               ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.GreaterEqual.Value.Date;

            DateTime End = ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    StaticParams.DateTimeNow.Date.AddDays(1).AddSeconds(-1) :
                    ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Date.LessEqual.Value.Date.AddDays(1).AddSeconds(-1);
            if (End.Subtract(Start).Days > 31)
                return BadRequest("Chỉ được phép xem tối đa trong vòng 31 ngày");

            ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Skip = 0;
            ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO.Take = int.MaxValue;
            List<ReportSalesOrderByItem_ReportSalesOrderByItemDTO> ReportSalesOrderByItem_ReportSalesOrderByItemDTOs = (await List(ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO)).Value;

            ReportSalesOrderByItem_TotalDTO ReportSalesOrderByItem_TotalDTO = await Total(ReportSalesOrderByItem_ReportSalesOrderByItemFilterDTO);
            long stt = 1;
            foreach (ReportSalesOrderByItem_ReportSalesOrderByItemDTO ReportSalesOrderByItem_ReportSalesOrderByItemDTO in ReportSalesOrderByItem_ReportSalesOrderByItemDTOs)
            {
                foreach (var Item in ReportSalesOrderByItem_ReportSalesOrderByItemDTO.ItemDetails)
                {
                    Item.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Sales_Order_By_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.ToString("dd-MM-yyyy");
            Data.End = End.ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByItems = ReportSalesOrderByItem_ReportSalesOrderByItemDTOs;
            Data.Total = ReportSalesOrderByItem_TotalDTO;
            using (var document = Configuration.Factory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportSalesOrderByItem.xlsx");
        }
    }
}
