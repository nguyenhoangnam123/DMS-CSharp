﻿using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Helpers;
using DMS.Services.MAppUser;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MOrganization;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStore;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreStatus;
using DMS.Services.MStoreType;
using DMS.Services.MSupplier;
using DMS.Services.MTaxType;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MWorkflow;
using GleamTech.DocumentUltimate;
using DMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System;
using DMS.Models;
using Microsoft.EntityFrameworkCore;

namespace DMS.Rpc.indirect_sales_order
{
    public partial class IndirectSalesOrderController : RpcController
    {
        private IEditedPriceStatusService EditedPriceStatusService;
        private IStoreService StoreService;
        private IAppUserService AppUserService;
        private IOrganizationService OrganizationService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IProductGroupingService ProductGroupingService;
        private IProductTypeService ProductTypeService;
        private IProductService ProductService;
        private IRequestStateService RequestStateService;
        private ISupplierService SupplierService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreStatusService StoreStatusService;
        private IStoreTypeService StoreTypeService;
        private ITaxTypeService TaxTypeService;
        private DataContext DataContext;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderController(
            IOrganizationService OrganizationService,
            IEditedPriceStatusService EditedPriceStatusService,
            IStoreService StoreService,
            IAppUserService AppUserService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            IProductGroupingService ProductGroupingService,
            IProductTypeService ProductTypeService,
            IProductService ProductService,
            IRequestStateService RequestStateService,
            ISupplierService SupplierService,
            IStoreGroupingService StoreGroupingService,
            IStoreStatusService StoreStatusService,
            IStoreTypeService StoreTypeService,
            ITaxTypeService TaxTypeService,
            DataContext DataContext,
            ICurrentContext CurrentContext
        )
        {
            this.OrganizationService = OrganizationService;
            this.EditedPriceStatusService = EditedPriceStatusService;
            this.StoreService = StoreService;
            this.AppUserService = AppUserService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.ItemService = ItemService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ProductGroupingService = ProductGroupingService;
            this.ProductTypeService = ProductTypeService;
            this.ProductService = ProductService;
            this.RequestStateService = RequestStateService;
            this.SupplierService = SupplierService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreStatusService = StoreStatusService;
            this.StoreTypeService = StoreTypeService;
            this.TaxTypeService = TaxTypeService;
            this.DataContext = DataContext;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> List([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }


        [Route(IndirectSalesOrderRoute.CountNew), HttpPost]
        public async Task<ActionResult<int>> CountNew([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.CountNew(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.ListNew), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> ListNew([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.ListNew(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }


        [Route(IndirectSalesOrderRoute.CountPending), HttpPost]
        public async Task<ActionResult<int>> CountPending([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.CountPending(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.ListPending), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> ListPending([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.ListPending(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }


        [Route(IndirectSalesOrderRoute.CountCompleted), HttpPost]
        public async Task<ActionResult<int>> CountCompleted([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.CountCompleted(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.ListCompleted), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> ListCompleted([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.ListCompleted(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }

        [Route(IndirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Get([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.Get(IndirectSalesOrder_IndirectSalesOrderDTO.Id);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);

            return IndirectSalesOrder_IndirectSalesOrderDTO;
        }

        [Route(IndirectSalesOrderRoute.GetDetail), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> GetDetail([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.GetDetail(IndirectSalesOrder_IndirectSalesOrderDTO.Id);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            return IndirectSalesOrder_IndirectSalesOrderDTO;
        }

        [Route(IndirectSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Create([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Create(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Update), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Update([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Update(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Send), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Send([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Send(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Approve), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Approve([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Approve(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Reject), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Reject([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Reject(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.BulkApprove), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> BulkApprove([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Id = new IdFilter { In = Ids };
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.Code | IndirectSalesOrderSelect.SaleEmployee | IndirectSalesOrderSelect.BuyerStore |
                IndirectSalesOrderSelect.Organization | IndirectSalesOrderSelect.RequestState | IndirectSalesOrderSelect.Total | IndirectSalesOrderSelect.TotalTaxAmount;
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = int.MaxValue;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);

            IndirectSalesOrders = await IndirectSalesOrderService.BulkApprove(IndirectSalesOrders);
            var IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders?.Select(x => new IndirectSalesOrder_IndirectSalesOrderDTO(x)).ToList();

            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }

        [Route(IndirectSalesOrderRoute.BulkReject), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> BulkReject([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Id = new IdFilter { In = Ids };
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.Id | IndirectSalesOrderSelect.Code | IndirectSalesOrderSelect.SaleEmployee | IndirectSalesOrderSelect.BuyerStore |
                IndirectSalesOrderSelect.Organization | IndirectSalesOrderSelect.RequestState | IndirectSalesOrderSelect.Total | IndirectSalesOrderSelect.TotalTaxAmount;
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = int.MaxValue;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);

            IndirectSalesOrders = await IndirectSalesOrderService.BulkReject(IndirectSalesOrders);
            var IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders?.Select(x => new IndirectSalesOrder_IndirectSalesOrderDTO(x)).ToList();

            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }

        [Route(IndirectSalesOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Delete([FromBody] IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO);
            IndirectSalesOrder = await IndirectSalesOrderService.Delete(IndirectSalesOrder);
            IndirectSalesOrder_IndirectSalesOrderDTO = new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
            if (IndirectSalesOrder.IsValidated)
                return IndirectSalesOrder_IndirectSalesOrderDTO;
            else
                return BadRequest(IndirectSalesOrder_IndirectSalesOrderDTO);
        }

        [Route(IndirectSalesOrderRoute.Print), HttpGet]
        public async Task<ActionResult> Print([FromQuery] long Id)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var IndirectSalesOrder = await IndirectSalesOrderService.Get(Id);
            if (IndirectSalesOrder == null)
                return Content("Đơn hàng không tồn tại");
            IndirectSalesOrder_PrintDTO IndirectSalesOrder_PrintDTO = new IndirectSalesOrder_PrintDTO(IndirectSalesOrder);
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            var STT = 1;
            foreach (var IndirectSalesOrderContent in IndirectSalesOrder_PrintDTO.Contents)
            {
                IndirectSalesOrderContent.STT = STT++;
                IndirectSalesOrderContent.AmountString = IndirectSalesOrderContent.Amount.ToString("N0", culture);
                IndirectSalesOrderContent.PrimaryPriceString = IndirectSalesOrderContent.PrimaryPrice.ToString("N0", culture);
                IndirectSalesOrderContent.QuantityString = IndirectSalesOrderContent.Quantity.ToString("N0", culture);
                IndirectSalesOrderContent.RequestedQuantityString = IndirectSalesOrderContent.RequestedQuantity.ToString("N0", culture);
                IndirectSalesOrderContent.SalePriceString = IndirectSalesOrderContent.SalePrice.ToString("N0", culture);
                IndirectSalesOrderContent.DiscountString = IndirectSalesOrderContent.DiscountPercentage.HasValue ? IndirectSalesOrderContent.DiscountPercentage.Value.ToString("N0", culture) + "%" : "";
                IndirectSalesOrderContent.TaxPercentageString = IndirectSalesOrderContent.TaxPercentage.HasValue ? IndirectSalesOrderContent.TaxPercentage.Value.ToString("N0", culture) + "%" : "";
            }
            foreach (var IndirectSalesOrderPromotion in IndirectSalesOrder_PrintDTO.Promotions)
            {
                IndirectSalesOrderPromotion.STT = STT++;
                IndirectSalesOrderPromotion.QuantityString = IndirectSalesOrderPromotion.Quantity.ToString("N0", culture);
                IndirectSalesOrderPromotion.RequestedQuantityString = IndirectSalesOrderPromotion.RequestedQuantity.ToString("N0", culture);
            }

            IndirectSalesOrder_PrintDTO.SubTotalString = IndirectSalesOrder_PrintDTO.SubTotal.ToString("N0", culture);
            IndirectSalesOrder_PrintDTO.Discount = IndirectSalesOrder_PrintDTO.GeneralDiscountAmount.HasValue ? IndirectSalesOrder_PrintDTO.GeneralDiscountAmount.Value.ToString("N0", culture) : "";
            IndirectSalesOrder_PrintDTO.TotalString = IndirectSalesOrder_PrintDTO.Total.ToString("N0", culture);
            IndirectSalesOrder_PrintDTO.sOrderDate = IndirectSalesOrder_PrintDTO.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            IndirectSalesOrder_PrintDTO.sDeliveryDate = IndirectSalesOrder_PrintDTO.DeliveryDate.HasValue ? IndirectSalesOrder_PrintDTO.DeliveryDate.Value.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy") : string.Empty;
            IndirectSalesOrder_PrintDTO.TotalText = Utils.ConvertAmountTostring((long)IndirectSalesOrder_PrintDTO.Total);

            string path = "Templates/Print_Indirect.docx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            dynamic Data = new ExpandoObject();
            Data.Order = IndirectSalesOrder_PrintDTO;
            MemoryStream MemoryStream = new MemoryStream();
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "docx"))
            {
                document.Process(Data);
            };

            ContentDisposition cd = new ContentDisposition
            {
                FileName = $"Don-hang-gian-tiep-{IndirectSalesOrder.Code}.docx",
                Inline = false,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(output.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document;charset=utf-8");
        }

        [Route(IndirectSalesOrderRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate.LessEqual.Value;

            IndirectSalesOrder_IndirectSalesOrderFilterDTO.Skip = 0;
            IndirectSalesOrder_IndirectSalesOrderFilterDTO.Take = int.MaxValue;
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = (await List(IndirectSalesOrder_IndirectSalesOrderFilterDTO)).Value;

            var OrganizationIds = IndirectSalesOrder_IndirectSalesOrderDTOs.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).Select(x => new Organization
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            List<IndirectSalesOrder_ExportDTO> Exports = Organizations.Select(x => new IndirectSalesOrder_ExportDTO
            {
                OrganizationId = x.Id,
                OrganizationName = x.Name,
            }).ToList();

            long stt = 1;
            decimal SubTotal = 0;
            decimal GeneralDiscountAmount = 0;
            decimal Total = 0;
            foreach (IndirectSalesOrder_ExportDTO IndirectSalesOrder_ExportDTO in Exports)
            {
                IndirectSalesOrder_ExportDTO.Contents = IndirectSalesOrder_IndirectSalesOrderDTOs
                    .Where(x => x.OrganizationId == IndirectSalesOrder_ExportDTO.OrganizationId)
                    .Select(x => new IndirectSalesOrder_ExportContentDTO(x))
                    .ToList();
                foreach (var content in IndirectSalesOrder_ExportDTO.Contents)
                {
                    content.STT = stt++;
                    content.OrderDateString = content.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
                    content.DeliveryDateString = content.DeliveryDate?.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
                    if (content.EditedPriceStatus.Id == EditedPriceStatusEnum.ACTIVE.Id)
                        content.EditPrice = "x";
                }
                SubTotal += IndirectSalesOrder_ExportDTO.Contents.Sum(x => x.SubTotal);
                GeneralDiscountAmount += IndirectSalesOrder_ExportDTO.Contents.Where(x => x.GeneralDiscountAmount.HasValue).Sum(x => x.GeneralDiscountAmount.Value);
                Total += IndirectSalesOrder_ExportDTO.Contents.Sum(x => x.Total);
            }

            string path = "Templates/Indirect_Order_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start == LocalStartDay(CurrentContext) ? "" : Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.Exports = Exports;
            Data.SubTotal = SubTotal;
            Data.GeneralDiscountAmount = GeneralDiscountAmount;
            Data.Total = Total;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ListIndirectSalesOrder.xlsx");
        }

        [Route(IndirectSalesOrderRoute.ExportDetail), HttpPost]
        public async Task<ActionResult> ExportDetail([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DateTime Start = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate?.GreaterEqual == null ?
                    LocalStartDay(CurrentContext) :
                    IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate.GreaterEqual.Value;

            DateTime End = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate?.LessEqual == null ?
                    LocalEndDay(CurrentContext) :
                    IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate.LessEqual.Value;

            IndirectSalesOrder_IndirectSalesOrderFilterDTO.Skip = 0;
            IndirectSalesOrder_IndirectSalesOrderFilterDTO.Take = int.MaxValue;
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = (await List(IndirectSalesOrder_IndirectSalesOrderFilterDTO)).Value;
            var Ids = IndirectSalesOrder_IndirectSalesOrderDTOs.Select(x => x.Id).ToList();
            var RowIds = IndirectSalesOrder_IndirectSalesOrderDTOs.Where(x => x.RequestStateId == RequestStateEnum.APPROVED.Id).Select(x => x.RowId).ToList();
            var queryContent = from c in DataContext.IndirectSalesOrderContent
                               join u in DataContext.UnitOfMeasure on c.UnitOfMeasureId equals u.Id
                               join i in DataContext.Item on c.ItemId equals i.Id
                               join p in DataContext.Product on i.ProductId equals p.Id
                               join ct in DataContext.Category on p.CategoryId equals ct.Id
                               where Ids.Contains(c.IndirectSalesOrderId)
                               select new IndirectSalesOrder_IndirectSalesOrderContentDTO
                               {
                                   Id = c.Id,
                                   IndirectSalesOrderId = c.IndirectSalesOrderId,
                                   ItemId = c.ItemId,
                                   UnitOfMeasureId = c.UnitOfMeasureId,
                                   SalePrice = c.SalePrice,
                                   RequestedQuantity = c.RequestedQuantity,
                                   Quantity = c.Quantity,
                                   Amount = c.Amount,
                                   UnitOfMeasure = new IndirectSalesOrder_UnitOfMeasureDTO
                                   {
                                       Name = u.Name
                                   },
                                   Item = new IndirectSalesOrder_ItemDTO
                                   {
                                       Code = i.Code,
                                       Name = i.Name,
                                       Product = new IndirectSalesOrder_ProductDTO
                                       {
                                           Category = new IndirectSalesOrder_CategoryDTO
                                           {
                                               Name = ct.Name
                                           }
                                       }
                                   }
                               };

            var queryPromotion = from c in DataContext.IndirectSalesOrderPromotion
                                 join u in DataContext.UnitOfMeasure on c.UnitOfMeasureId equals u.Id
                                 join i in DataContext.Item on c.ItemId equals i.Id
                                 join p in DataContext.Product on i.ProductId equals p.Id
                                 join ct in DataContext.Category on p.CategoryId equals ct.Id
                                 where Ids.Contains(c.IndirectSalesOrderId)
                                 select new IndirectSalesOrder_IndirectSalesOrderPromotionDTO
                                 {
                                     Id = c.Id,
                                     IndirectSalesOrderId = c.IndirectSalesOrderId,
                                     ItemId = c.ItemId,
                                     UnitOfMeasureId = c.UnitOfMeasureId,
                                     RequestedQuantity = c.RequestedQuantity,
                                     Quantity = c.Quantity,
                                     UnitOfMeasure = new IndirectSalesOrder_UnitOfMeasureDTO
                                     {
                                         Name = u.Name
                                     },
                                     Item = new IndirectSalesOrder_ItemDTO
                                     {
                                         Code = i.Code,
                                         Name = i.Name,
                                         Product = new IndirectSalesOrder_ProductDTO
                                         {
                                             Category = new IndirectSalesOrder_CategoryDTO
                                             {
                                                 Name = ct.Name
                                             }
                                         }
                                     }
                                 };

            var queryRequestWorkflowStepMapping = from r in DataContext.RequestWorkflowStepMapping
                                                  join a in DataContext.AppUser on r.AppUserId equals a.Id
                                                  where RowIds.Contains(r.RequestId)
                                                  select new IndirectSalesOrder_RequestWorkflowStepMappingDTO
                                                  {
                                                      AppUserId = r.AppUserId,
                                                      UpdatedAt = r.UpdatedAt,
                                                      RequestId = r.RequestId,
                                                      WorkflowStateId = r.WorkflowStateId,
                                                      AppUser = new IndirectSalesOrder_AppUserDTO
                                                      {
                                                          Username = a.Username,
                                                          DisplayName = a.DisplayName,
                                                      }
                                                  };

            List<IndirectSalesOrder_IndirectSalesOrderContentDTO> IndirectSalesOrder_IndirectSalesOrderContentDTOs = await queryContent.ToListAsync();
            List<IndirectSalesOrder_IndirectSalesOrderPromotionDTO> IndirectSalesOrder_IndirectSalesOrderPromotionDTOs = await queryPromotion.ToListAsync();
            List<IndirectSalesOrder_RequestWorkflowStepMappingDTO> IndirectSalesOrder_RequestWorkflowStepMappingDTOs = await queryRequestWorkflowStepMapping.ToListAsync();

            var OrganizationIds = IndirectSalesOrder_IndirectSalesOrderDTOs.Select(x => x.OrganizationId).Distinct().ToList();
            var Organizations = await DataContext.Organization.Where(x => OrganizationIds.Contains(x.Id)).Select(x => new Organization
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            List<IndirectSalesOrder_ExportDetailDTO> Exports = IndirectSalesOrder_IndirectSalesOrderDTOs.Select(x => new IndirectSalesOrder_ExportDetailDTO
            {
                OrganizationId = x.OrganizationId,
                Id = x.Id,
                RequestStateId = x.RequestStateId,
                Code = x.Code,
                Note = x.Note,
                Discount = x.GeneralDiscountAmount.GetValueOrDefault(0),
                OrderDate = x.OrderDate.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy"),
                DeliveryDate = x.DeliveryDate?.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy"),
                BuyerStoreAddress = x.BuyerStore.Address,
                BuyerStoreCode = x.BuyerStore.Code,
                BuyerStoreName = x.BuyerStore.Name,
                BuyerStoreGroupingName = x.BuyerStore.StoreGrouping?.Name,
                BuyerStoreTypeName = x.BuyerStore.StoreType.Name,
                SalesEmployeeName = x.SaleEmployee.DisplayName,
                SalesEmployeeUserName = x.SaleEmployee.Username,
                SellerStoreCode = x.SellerStore.Code,
                SellerStoreName = x.SellerStore.Name,
                RequestStateName = x.RequestState.Name,
                SubTotal = x.SubTotal,
                Total = x.Total,
                RowId = x.RowId,
                Contents = new List<IndirectSalesOrder_ExportDetailContentDTO>()
            }).ToList();
            
            foreach (var Export in Exports)
            {
                var Organization = Organizations.Where(x => x.Id == Export.OrganizationId).FirstOrDefault();
                if (Organization != null)
                    Export.OrganizationName = Organization.Name;

                if(Export.RequestStateId == RequestStateEnum.APPROVED.Id)
                {
                    var RequestWorkflowStepMapping = IndirectSalesOrder_RequestWorkflowStepMappingDTOs
                    .Where(x => x.RequestId == Export.RowId)
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefault();
                    if (RequestWorkflowStepMapping != null)
                    {
                        Export.ApprovedAt = RequestWorkflowStepMapping.UpdatedAt.Value.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
                        Export.MonitorName = RequestWorkflowStepMapping.AppUser.DisplayName;
                        Export.MonitorUserName = RequestWorkflowStepMapping.AppUser.Username;
                        Export.RequestStateName = RequestStateEnum.APPROVED.Name;
                    }
                }

                var Contents = IndirectSalesOrder_IndirectSalesOrderContentDTOs.Where(x => x.IndirectSalesOrderId == Export.Id).ToList();
                var Promotions = IndirectSalesOrder_IndirectSalesOrderPromotionDTOs.Where(x => x.IndirectSalesOrderId == Export.Id).ToList();

                foreach (var Content in Contents)
                {
                    IndirectSalesOrder_ExportDetailContentDTO IndirectSalesOrder_ExportDetailContentDTO = new IndirectSalesOrder_ExportDetailContentDTO
                    {
                        Amount = Content.Amount,
                        CategoryName = Content.Item.Product.Category.Name,
                        ItemCode = Content.Item.Code,
                        ItemName = Content.Item.Name,
                        UnitOfMeasureName = Content.UnitOfMeasure.Name,
                        ItemOrderType = "Sản phẩm bán",
                        Quantity = Content.Quantity,
                        SalePrice = Content.SalePrice,
                    };
                    Export.Contents.Add(IndirectSalesOrder_ExportDetailContentDTO);
                }

                foreach (var Promotion in Promotions)
                {
                    IndirectSalesOrder_ExportDetailContentDTO IndirectSalesOrder_ExportDetailContentDTO = new IndirectSalesOrder_ExportDetailContentDTO
                    {
                        CategoryName = Promotion.Item.Product.Category.Name,
                        ItemCode = Promotion.Item.Code,
                        ItemName = Promotion.Item.Name,
                        UnitOfMeasureName = Promotion.UnitOfMeasure.Name,
                        ItemOrderType = "Sản phẩm khuyến mại",
                        Quantity = Promotion.Quantity,
                        Amount = 0,
                        SalePrice = 0,
                    };
                    Export.Contents.Add(IndirectSalesOrder_ExportDetailContentDTO);
                }
            }

            long stt = 1;
            var Datas = new List<IndirectSalesOrder_ExportDetailContentDTO>();
            foreach (var Export in Exports)
            {
                foreach (var Content in Export.Contents)
                {
                    IndirectSalesOrder_ExportDetailContentDTO IndirectSalesOrder_ExportDetailContentDTO = new IndirectSalesOrder_ExportDetailContentDTO
                    {
                        STT = stt++,
                        OrganizationId = Export.OrganizationId,
                        Id = Export.Id,
                        RequestStateId = Export.RequestStateId,
                        Code = Export.Code,
                        Note = Export.Note,
                        Discount = Export.Discount,
                        OrderDate = Export.OrderDate,
                        DeliveryDate = Export.DeliveryDate,
                        BuyerStoreAddress = Export.BuyerStoreAddress,
                        BuyerStoreCode = Export.BuyerStoreCode,
                        BuyerStoreName = Export.BuyerStoreName,
                        BuyerStoreGroupingName = Export.BuyerStoreGroupingName,
                        BuyerStoreTypeName = Export.BuyerStoreTypeName,
                        SalesEmployeeName = Export.SalesEmployeeName,
                        SalesEmployeeUserName = Export.SalesEmployeeUserName,
                        SellerStoreCode = Export.SellerStoreCode,
                        SellerStoreName = Export.SellerStoreName,
                        RequestStateName = Export.RequestStateName,
                        SubTotal = Export.SubTotal,
                        Total = Export.Total,
                        RowId = Export.RowId,
                        ApprovedAt = Export.ApprovedAt,
                        ERouteCode = Export.ERouteCode,
                        ERouteName = Export.ERouteName,
                        MonitorName = Export.MonitorName,
                        MonitorUserName = Export.MonitorUserName,
                        OrganizationName = Export.OrganizationName,
                        Amount = Content.Amount,
                        CategoryName = Content.CategoryName,
                        ItemCode = Content.ItemCode,
                        ItemName = Content.ItemName,
                        UnitOfMeasureName = Content.UnitOfMeasureName,
                        ItemOrderType = Content.ItemOrderType,
                        Quantity = Content.Quantity,
                        SalePrice = Content.SalePrice,
                    };
                    Datas.Add(IndirectSalesOrder_ExportDetailContentDTO);
                }
            }

            string path = "Templates/Indirect_Order_Detail_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.Start = Start == LocalStartDay(CurrentContext) ? "" : Start.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.End = End.AddHours(CurrentContext.TimeZone).ToString("dd-MM-yyyy");
            Data.Exports = Datas;
            Data.SubTotal = Exports.Sum(x => x.SubTotal);
            Data.Discount = Exports.Sum(x => x.Discount);
            Data.Total = Exports.Sum(x => x.Total);
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "ListIndirectSalesOrder.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter = await IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                IndirectSalesOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private IndirectSalesOrder ConvertDTOToEntity(IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
            IndirectSalesOrder.Id = IndirectSalesOrder_IndirectSalesOrderDTO.Id;
            IndirectSalesOrder.Code = IndirectSalesOrder_IndirectSalesOrderDTO.Code;
            IndirectSalesOrder.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStoreId;
            IndirectSalesOrder.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderDTO.PhoneNumber;
            IndirectSalesOrder.StoreAddress = IndirectSalesOrder_IndirectSalesOrderDTO.StoreAddress;
            IndirectSalesOrder.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.DeliveryAddress;
            IndirectSalesOrder.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStoreId;
            IndirectSalesOrder.SaleEmployeeId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployeeId;
            IndirectSalesOrder.OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.OrganizationId;
            IndirectSalesOrder.OrderDate = IndirectSalesOrder_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.RequestStateId = IndirectSalesOrder_IndirectSalesOrderDTO.RequestStateId;
            IndirectSalesOrder.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = IndirectSalesOrder_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = IndirectSalesOrder_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.Total = IndirectSalesOrder_IndirectSalesOrderDTO.Total;
            IndirectSalesOrder.BuyerStore = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                Telephone = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.WardId,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.OwnerEmail,
                TaxCode = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.TaxCode,
                LegalEntity = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.LegalEntity,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StatusId,
            };
            IndirectSalesOrder.Organization = IndirectSalesOrder_IndirectSalesOrderDTO.Organization == null ? null : new Organization
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Name,
                ParentId = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.ParentId,
                Path = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Path,
                Level = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Level,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.StatusId,
                Phone = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Phone,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Address,
                Email = IndirectSalesOrder_IndirectSalesOrderDTO.Organization.Email,
            };
            IndirectSalesOrder.EditedPriceStatus = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.SaleEmployee = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Username,
                DisplayName = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Phone,
                PositionId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.PositionId,
                Department = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            IndirectSalesOrder.SellerStore = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore == null ? null : new Store
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Name,
                ParentStoreId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ParentStoreId,
                OrganizationId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OrganizationId,
                StoreTypeId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StoreTypeId,
                StoreGroupingId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StoreGroupingId,
                Telephone = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Telephone,
                ProvinceId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ProvinceId,
                DistrictId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DistrictId,
                WardId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.WardId,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Address,
                DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryAddress,
                Latitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Latitude,
                Longitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.Longitude,
                DeliveryLatitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryLatitude,
                DeliveryLongitude = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.DeliveryLongitude,
                OwnerName = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerName,
                OwnerPhone = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerPhone,
                OwnerEmail = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.OwnerEmail,
                TaxCode = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.TaxCode,
                LegalEntity = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.LegalEntity,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StatusId,
            };
            IndirectSalesOrder.IndirectSalesOrderContents = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderContents?
                .Select(x => new IndirectSalesOrderContent
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    PrimaryPrice = x.PrimaryPrice,
                    SalePrice = x.SalePrice,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
                    Factor = x.Factor,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        RetailPrice = x.Item.RetailPrice,
                        SalePrice = x.Item.SalePrice,
                        SaleStock = x.Item.SaleStock,
                        ScanCode = x.Item.ScanCode,
                        StatusId = x.Item.StatusId,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            SupplierCode = x.Item.Product.SupplierCode,
                            Name = x.Item.Product.Name,
                            Description = x.Item.Product.Description,
                            ScanCode = x.Item.Product.ScanCode,
                            ProductTypeId = x.Item.Product.ProductTypeId,
                            SupplierId = x.Item.Product.SupplierId,
                            BrandId = x.Item.Product.BrandId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            RetailPrice = x.Item.Product.RetailPrice,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            StatusId = x.Item.Product.StatusId,
                            ProductType = x.Item.Product.ProductType == null ? null : new ProductType
                            {
                                Id = x.Item.Product.ProductType.Id,
                                Code = x.Item.Product.ProductType.Code,
                                Name = x.Item.Product.ProductType.Name,
                                Description = x.Item.Product.ProductType.Description,
                                StatusId = x.Item.Product.ProductType.StatusId,
                            },
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Name = x.Item.Product.TaxType.Name,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            },
                        }
                    },
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },

                }).ToList();
            IndirectSalesOrder.IndirectSalesOrderPromotions = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderPromotions?
                .Select(x => new IndirectSalesOrderPromotion
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    Quantity = x.Quantity,
                    PrimaryUnitOfMeasureId = x.PrimaryUnitOfMeasureId,
                    RequestedQuantity = x.RequestedQuantity,
                    Note = x.Note,
                    Factor = x.Factor,
                    Item = x.Item == null ? null : new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                        SaleStock = x.Item.SaleStock,
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            Code = x.Item.Product.Code,
                            SupplierCode = x.Item.Product.SupplierCode,
                            Name = x.Item.Product.Name,
                            Description = x.Item.Product.Description,
                            ScanCode = x.Item.Product.ScanCode,
                            ProductTypeId = x.Item.Product.ProductTypeId,
                            SupplierId = x.Item.Product.SupplierId,
                            BrandId = x.Item.Product.BrandId,
                            UnitOfMeasureId = x.Item.Product.UnitOfMeasureId,
                            UnitOfMeasureGroupingId = x.Item.Product.UnitOfMeasureGroupingId,
                            RetailPrice = x.Item.Product.RetailPrice,
                            TaxTypeId = x.Item.Product.TaxTypeId,
                            StatusId = x.Item.Product.StatusId,
                            ProductType = x.Item.Product.ProductType == null ? null : new ProductType
                            {
                                Id = x.Item.Product.ProductType.Id,
                                Code = x.Item.Product.ProductType.Code,
                                Name = x.Item.Product.ProductType.Name,
                                Description = x.Item.Product.ProductType.Description,
                                StatusId = x.Item.Product.ProductType.StatusId,
                            },
                            TaxType = x.Item.Product.TaxType == null ? null : new TaxType
                            {
                                Id = x.Item.Product.TaxType.Id,
                                Code = x.Item.Product.TaxType.Code,
                                StatusId = x.Item.Product.TaxType.StatusId,
                                Name = x.Item.Product.TaxType.Name,
                                Percentage = x.Item.Product.TaxType.Percentage,
                            },
                            UnitOfMeasure = x.Item.Product.UnitOfMeasure == null ? null : new UnitOfMeasure
                            {
                                Id = x.Item.Product.UnitOfMeasure.Id,
                                Code = x.Item.Product.UnitOfMeasure.Code,
                                Name = x.Item.Product.UnitOfMeasure.Name,
                                Description = x.Item.Product.UnitOfMeasure.Description,
                                StatusId = x.Item.Product.UnitOfMeasure.StatusId,
                            },
                            UnitOfMeasureGrouping = x.Item.Product.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
                            {
                                Id = x.Item.Product.UnitOfMeasureGrouping.Id,
                                Code = x.Item.Product.UnitOfMeasureGrouping.Code,
                                Name = x.Item.Product.UnitOfMeasureGrouping.Name,
                                Description = x.Item.Product.UnitOfMeasureGrouping.Description,
                                UnitOfMeasureId = x.Item.Product.UnitOfMeasureGrouping.UnitOfMeasureId,
                            },
                        }
                    },
                    PrimaryUnitOfMeasure = x.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.PrimaryUnitOfMeasure.Id,
                        Code = x.PrimaryUnitOfMeasure.Code,
                        Name = x.PrimaryUnitOfMeasure.Name,
                        Description = x.PrimaryUnitOfMeasure.Description,
                        StatusId = x.PrimaryUnitOfMeasure.StatusId,
                    },
                    UnitOfMeasure = x.UnitOfMeasure == null ? null : new UnitOfMeasure
                    {
                        Id = x.UnitOfMeasure.Id,
                        Code = x.UnitOfMeasure.Code,
                        Name = x.UnitOfMeasure.Name,
                        Description = x.UnitOfMeasure.Description,
                        StatusId = x.UnitOfMeasure.StatusId,
                    },
                }).ToList();
            IndirectSalesOrder.BaseLanguage = CurrentContext.Language;
            return IndirectSalesOrder;
        }

        private IndirectSalesOrderFilter ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Skip = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Skip;
            IndirectSalesOrderFilter.Take = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Take;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderBy;
            IndirectSalesOrderFilter.OrderType = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderType;

            IndirectSalesOrderFilter.Id = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.OrganizationId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrganizationId;
            IndirectSalesOrderFilter.Code = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.AppUserId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.AppUserId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.RequestStateId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.RequestStateId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Total;
            IndirectSalesOrderFilter.StoreStatusId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.StoreStatusId;
            return IndirectSalesOrderFilter;
        }
    }
}

