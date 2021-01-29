using DMS.Common;
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
using DMS.Helpers;
using DMS.Services.MDirectSalesOrder;
using DMS.Services.MOrganization;
using Microsoft.EntityFrameworkCore;
using DMS.Repositories;
using System.IO;
using System.Dynamic;
using NGS.Templater;
using DMS.Rpc.direct_sales_order;
using DMS.Services.MAppUser;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_by_item
{
    public class ReportDirectSalesOrderByItemController : RpcController
    {
        private DataContext DataContext;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IDirectSalesOrderService DirectSalesOrderService;
        private IItemService ItemService;
        private IProductService ProductService;
        private IProductTypeService ProductTypeService;
        private IProductGroupingService ProductGroupingService;
        private ICurrentContext CurrentContext;
        public ReportDirectSalesOrderByItemController(
            DataContext DataContext,
            IAppUserService AppUserService,
            IOrganizationService OrganizationService,
            IItemService ItemService,
            IDirectSalesOrderService DirectSalesOrderService,
            IProductService ProductService,
            IProductTypeService ProductTypeService,
            IProductGroupingService ProductGroupingService,
            ICurrentContext CurrentContext)
        {
            this.DataContext = DataContext;
            this.AppUserService = AppUserService;
            this.OrganizationService = OrganizationService;
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.ItemService = ItemService;
            this.ProductService = ProductService;
            this.ProductTypeService = ProductTypeService;
            this.ProductGroupingService = ProductGroupingService;
            this.CurrentContext = CurrentContext;
        }

        #region Filter List
        [Route(ReportDirectSalesOrderByItemRoute.FilterListOrganization), HttpPost]
        public async Task<List<ReportDirectSalesOrderByItem_OrganizationDTO>> FilterListOrganization()
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
            List<ReportDirectSalesOrderByItem_OrganizationDTO> ReportDirectSalesOrderByItem_OrganizationDTO = Organizations
                .Select(x => new ReportDirectSalesOrderByItem_OrganizationDTO(x)).ToList();
            return ReportDirectSalesOrderByItem_OrganizationDTO;
        }

        [Route(ReportDirectSalesOrderByItemRoute.FilterListProductGrouping), HttpPost]
        public async Task<List<ReportDirectSalesOrderByItem_ProductGroupingDTO>> FilterListProductGrouping([FromBody] ReportDirectSalesOrderByItem_ProductGroupingFilterDTO ReportDirectSalesOrderByItem_ProductGroupingFilterDTO)
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
            ProductGroupingFilter.Code = ReportDirectSalesOrderByItem_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = ReportDirectSalesOrderByItem_ProductGroupingFilterDTO.Name;

            if (ProductGroupingFilter.Id == null) ProductGroupingFilter.Id = new IdFilter();
            ProductGroupingFilter.Id.In = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            List<ProductGrouping> ReportDirectSalesOrderByItemGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<ReportDirectSalesOrderByItem_ProductGroupingDTO> ReportDirectSalesOrderByItem_ProductGroupingDTOs = ReportDirectSalesOrderByItemGroupings
                .Select(x => new ReportDirectSalesOrderByItem_ProductGroupingDTO(x)).ToList();
            return ReportDirectSalesOrderByItem_ProductGroupingDTOs;
        }

        [Route(ReportDirectSalesOrderByItemRoute.FilterListProductType), HttpPost]
        public async Task<List<ReportDirectSalesOrderByItem_ProductTypeDTO>> FilterListProductType([FromBody] ReportDirectSalesOrderByItem_ProductTypeFilterDTO ReportDirectSalesOrderByItem_ProductTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = ReportDirectSalesOrderByItem_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = ReportDirectSalesOrderByItem_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = ReportDirectSalesOrderByItem_ProductTypeFilterDTO.Name;
            ProductTypeFilter.StatusId = ReportDirectSalesOrderByItem_ProductTypeFilterDTO.StatusId;

            if (ProductTypeFilter.Id == null) ProductTypeFilter.Id = new IdFilter();
            ProductTypeFilter.Id.In = await FilterProductType(ProductTypeService, CurrentContext);
            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<ReportDirectSalesOrderByItem_ProductTypeDTO> ReportDirectSalesOrderByItem_ProductTypeDTOs = ProductTypes
                .Select(x => new ReportDirectSalesOrderByItem_ProductTypeDTO(x)).ToList();
            return ReportDirectSalesOrderByItem_ProductTypeDTOs;
        }

        [Route(ReportDirectSalesOrderByItemRoute.FilterListItem), HttpPost]
        public async Task<List<ReportDirectSalesOrderByItem_ItemDTO>> FilterListItem([FromBody] ReportDirectSalesOrderByItem_ItemFilterDTO ReportDirectSalesOrderByItem_ItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = ReportDirectSalesOrderByItem_ItemFilterDTO.Id;
            ItemFilter.Code = ReportDirectSalesOrderByItem_ItemFilterDTO.Code;
            ItemFilter.Name = ReportDirectSalesOrderByItem_ItemFilterDTO.Name;
            ItemFilter.StatusId = ReportDirectSalesOrderByItem_ItemFilterDTO.StatusId;
            ItemFilter.Search = ReportDirectSalesOrderByItem_ItemFilterDTO.Search;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<ReportDirectSalesOrderByItem_ItemDTO> ReportDirectSalesOrderByItem_ItemDTOs = Items
                .Select(x => new ReportDirectSalesOrderByItem_ItemDTO(x)).ToList();
            return ReportDirectSalesOrderByItem_ItemDTOs;
        }
        #endregion

        [Route(ReportDirectSalesOrderByItemRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.HasValue == false)
                return 0;

            DateTime Start = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return 0;

            long? ItemId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            if (ProductTypeId.HasValue)
            {
                var listId = new List<long> { ProductTypeId.Value };
                ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            }
            List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            if (ProductGroupingId.HasValue)
            {
                var listId = new List<long> { ProductGroupingId.Value };
                ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            }
            var query = from t in DataContext.DirectSalesOrderTransaction
                        join od in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SaleEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        (ProductTypeIds.Contains(p.ProductTypeId)) &&
                        (
                            sppgm == null ||
                            (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        ) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.APPROVED.Id
                        group t by new {t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            return await query.CountAsync();
        }

        [Route(ReportDirectSalesOrderByItemRoute.List), HttpPost]
        public async Task<ActionResult<List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO>>> List([FromBody] ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.HasValue == false)
                return new List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO>();

            DateTime Start = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            if (End.Subtract(Start).Days > 31)
                return BadRequest(new { message = "Chỉ được phép xem tối đa trong vòng 31 ngày" });

            List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO> ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs = await ListData(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO, Start, End);
            return ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs;
        }

        [Route(ReportDirectSalesOrderByItemRoute.Total), HttpPost]
        public async Task<ReportDirectSalesOrderByItem_TotalDTO> Total([FromBody] ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.HasValue == false)
                return new ReportDirectSalesOrderByItem_TotalDTO();

            DateTime Start = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                   LocalStartDay(CurrentContext) :
                   ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            long? ItemId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            if (End.Subtract(Start).Days > 31)
                return new ReportDirectSalesOrderByItem_TotalDTO();

            ReportDirectSalesOrderByItem_TotalDTO ReportDirectSalesOrderByItem_TotalDTO = await TotalData(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO, Start, End);
            return ReportDirectSalesOrderByItem_TotalDTO;
        }

        [Route(ReportDirectSalesOrderByItemRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.GreaterEqual.Value;

            DateTime End = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Date.LessEqual.Value;

            ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Skip = 0;
            ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Take = int.MaxValue;
            List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO> ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs = await ListData(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO, Start, End);

            ReportDirectSalesOrderByItem_TotalDTO ReportDirectSalesOrderByItem_TotalDTO = await TotalData(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO, Start, End);
            long stt = 1;
            foreach (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO in ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs)
            {
                foreach (var Item in ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.ItemDetails)
                {
                    Item.STT = stt;
                    stt++;
                }
            }

            string path = "Templates/Report_Direct_Sales_Order_By_Item.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.ReportSalesOrderByItems = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs;
            Data.Total = ReportDirectSalesOrderByItem_TotalDTO;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ReportDirectSalesOrderByItem.xlsx");
        }

        private async Task<List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO>> ListData(
            ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ItemId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            if (ProductTypeId.HasValue)
            {
                var listId = new List<long> { ProductTypeId.Value };
                ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            }
            List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            if (ProductGroupingId.HasValue)
            {
                var listId = new List<long> { ProductGroupingId.Value };
                ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            }

            var query = from t in DataContext.DirectSalesOrderTransaction
                        join od in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SaleEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        (ProductTypeIds.Contains(p.ProductTypeId)) &&
                        (
                            sppgm == null ||
                            (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        ) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.APPROVED.Id
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query
                .OrderBy(x => x.OrganizationId).ThenBy(x => x.ItemId)
                .Skip(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Skip)
                .Take(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.Take)
                .ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();
            var OrganizationNames = await DataContext.Organization.Where(x => OrgIds.Contains(x.Id)).OrderBy(x => x.Id).Select(x => x.Name).ToListAsync();
            List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO> ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs = new List<ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO>();
            foreach (var OrganizationName in OrganizationNames)
            {
                ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO = new ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO();
                ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.OrganizationName = OrganizationName;
                ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.ItemDetails = new List<ReportDirectSalesOrderByItem_ItemDetailDTO>();
                ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs.Add(ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO);
            }

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.DirectSalesOrderTransaction
                                   join od in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals od.Id
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join ind in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   AppUserIds.Contains(od.SaleEmployeeId) &&
                                   od.OrderDate >= Start && od.OrderDate <= End &&
                                   ind.RequestStateId == RequestStateEnum.APPROVED.Id
                                   select new DirectSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       DirectSalesOrderId = t.DirectSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       DirectSalesOrder = new DirectSalesOrderDAO
                                       {
                                           BuyerStoreId = ind.BuyerStoreId
                                       },
                                       Item = new ItemDAO
                                       {
                                           Id = i.Id,
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
            var DirectSalesOrderTransactions = await queryTransaction.ToListAsync();

            foreach (var ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO in ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs)
            {
                var Transactions = DirectSalesOrderTransactions.Where(x => x.Organization.Name == ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.OrganizationName);
                foreach (var Transaction in Transactions)
                {
                    var ItemDetail = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.ItemDetails.Where(x => x.ItemId == Transaction.ItemId).FirstOrDefault();
                    if (ItemDetail == null)
                    {
                        ItemDetail = new ReportDirectSalesOrderByItem_ItemDetailDTO();
                        ItemDetail.ItemId = Transaction.Item.Id;
                        ItemDetail.ItemCode = Transaction.Item.Code;
                        ItemDetail.ItemName = Transaction.Item.Name;
                        ItemDetail.UnitOfMeasureName = Transaction.UnitOfMeasure.Name;
                        ItemDetail.DirectSalesOrderIds = new HashSet<long>();
                        ItemDetail.BuyerStoreIds = new HashSet<long>();
                        ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.ItemDetails.Add(ItemDetail);
                    }
                    if (Transaction.TypeId == TransactionTypeEnum.SALES_CONTENT.Id)
                    {
                        ItemDetail.SaleStock += Transaction.Quantity;
                    }
                    if (Transaction.TypeId == TransactionTypeEnum.PROMOTION.Id)
                    {
                        ItemDetail.PromotionStock += Transaction.Quantity;
                    }
                    ItemDetail.Discount += Transaction.Discount ?? 0;
                    ItemDetail.Revenue += Transaction.Revenue ?? 0;
                    ItemDetail.DirectSalesOrderIds.Add(Transaction.DirectSalesOrderId);
                    ItemDetail.BuyerStoreIds.Add(Transaction.DirectSalesOrder.BuyerStoreId);
                }
            }
            //làm tròn số
            foreach (var ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO in ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs)
            {
                foreach (var item in ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTO.ItemDetails)
                {
                    item.Discount = Math.Round(item.Discount, 0);
                    item.Revenue = Math.Round(item.Revenue, 0);
                }
            }

            return ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemDTOs;
        }

        private async Task<ReportDirectSalesOrderByItem_TotalDTO> TotalData(
            ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO,
            DateTime Start, DateTime End)
        {
            long? ItemId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ItemId?.Equal;
            long? ProductTypeId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductTypeId?.Equal;
            long? ProductGroupingId = ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.ProductGroupingId?.Equal;

            ReportDirectSalesOrderByItem_TotalDTO ReportDirectSalesOrderByItem_TotalDTO = new ReportDirectSalesOrderByItem_TotalDTO();

            List<long> OrganizationIds = await FilterOrganization(OrganizationService, CurrentContext);
            List<OrganizationDAO> OrganizationDAOs = await DataContext.Organization.Where(o => o.DeletedAt == null && OrganizationIds.Contains(o.Id)).ToListAsync();
            OrganizationDAO OrganizationDAO = null;
            if (ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId?.Equal != null)
            {
                OrganizationDAO = await DataContext.Organization.Where(o => o.Id == ReportDirectSalesOrderByItem_ReportDirectSalesOrderByItemFilterDTO.OrganizationId.Equal.Value).FirstOrDefaultAsync();
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

            List<long> ProductTypeIds = await FilterProductType(ProductTypeService, CurrentContext);
            if (ProductTypeId.HasValue)
            {
                var listId = new List<long> { ProductTypeId.Value };
                ProductTypeIds = ProductTypeIds.Intersect(listId).ToList();
            }
            List<long> ProductGroupingIds = await FilterProductGrouping(ProductGroupingService, CurrentContext);
            if (ProductGroupingId.HasValue)
            {
                var listId = new List<long> { ProductGroupingId.Value };
                ProductGroupingIds = ProductGroupingIds.Intersect(listId).ToList();
            }

            var query = from t in DataContext.DirectSalesOrderTransaction
                        join od in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals od.Id
                        join i in DataContext.Item on t.ItemId equals i.Id
                        join p in DataContext.Product on i.ProductId equals p.Id
                        join ppgm in DataContext.ProductProductGroupingMapping on p.Id equals ppgm.ProductId into PPGM
                        from sppgm in PPGM.DefaultIfEmpty()
                        where OrganizationIds.Contains(t.OrganizationId) &&
                        AppUserIds.Contains(od.SaleEmployeeId) &&
                        (ItemId.HasValue == false || t.ItemId == ItemId) &&
                        (ProductTypeIds.Contains(p.ProductTypeId)) &&
                        (
                            sppgm == null ||
                            (ProductGroupingIds.Any() == false || ProductGroupingIds.Contains(sppgm.ProductGroupingId))
                        ) &&
                        od.OrderDate >= Start && od.OrderDate <= End &&
                        od.RequestStateId == RequestStateEnum.APPROVED.Id
                        group t by new { t.OrganizationId, t.ItemId } into x
                        select new
                        {
                            OrganizationId = x.Key.OrganizationId,
                            ItemId = x.Key.ItemId,
                        };

            var keys = await query.ToListAsync();

            var OrgIds = keys.Select(x => x.OrganizationId).Distinct().ToList();

            var ItemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var queryTransaction = from t in DataContext.DirectSalesOrderTransaction
                                   join od in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals od.Id
                                   join i in DataContext.Item on t.ItemId equals i.Id
                                   join ind in DataContext.DirectSalesOrder on t.DirectSalesOrderId equals ind.Id
                                   join u in DataContext.UnitOfMeasure on t.UnitOfMeasureId equals u.Id
                                   join o in DataContext.Organization on t.OrganizationId equals o.Id
                                   where OrgIds.Contains(t.OrganizationId) &&
                                   ItemIds.Contains(t.ItemId) &&
                                   od.OrderDate >= Start && od.OrderDate <= End &&
                                   ind.RequestStateId == RequestStateEnum.APPROVED.Id
                                   select new DirectSalesOrderTransactionDAO
                                   {
                                       Id = t.Id,
                                       ItemId = t.ItemId,
                                       Discount = t.Discount,
                                       DirectSalesOrderId = t.DirectSalesOrderId,
                                       OrganizationId = t.OrganizationId,
                                       Quantity = t.Quantity,
                                       Revenue = t.Revenue,
                                       TypeId = t.TypeId,
                                       UnitOfMeasureId = t.UnitOfMeasureId,
                                       DirectSalesOrder = new DirectSalesOrderDAO
                                       {
                                           BuyerStoreId = ind.BuyerStoreId
                                       },
                                       Item = new ItemDAO
                                       {
                                           Id = i.Id,
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
            var DirectSalesOrderTransactions = await queryTransaction.ToListAsync();

            ReportDirectSalesOrderByItem_TotalDTO.TotalDiscount = DirectSalesOrderTransactions
                .Where(x => x.Discount.HasValue)
                .Select(x => x.Discount.Value)
                .DefaultIfEmpty(0)
                .Sum();
            ReportDirectSalesOrderByItem_TotalDTO.TotalRevenue = DirectSalesOrderTransactions
                .Where(x => x.Revenue.HasValue)
                .Select(x => x.Revenue.Value)
                .DefaultIfEmpty(0)
                .Sum();
            ReportDirectSalesOrderByItem_TotalDTO.TotalPromotionStock = DirectSalesOrderTransactions
                .Where(x => x.TypeId == TransactionTypeEnum.PROMOTION.Id)
                .Select(x => x.Quantity)
                .Sum();
            ReportDirectSalesOrderByItem_TotalDTO.TotalSalesStock = DirectSalesOrderTransactions
                .Where(x => x.TypeId == TransactionTypeEnum.SALES_CONTENT.Id)
                .Select(x => x.Quantity)
                .Sum();

            return ReportDirectSalesOrderByItem_TotalDTO;
        }

    }
}
