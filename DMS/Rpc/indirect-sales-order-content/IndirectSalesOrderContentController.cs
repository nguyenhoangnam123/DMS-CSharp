using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MIndirectSalesOrderContent;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MUnitOfMeasure;
using DMS.Enums;

namespace DMS.Rpc.indirect_sales_order_content
{
    public class IndirectSalesOrderContentRoute : Root
    {
        public const string Master = Module + "/indirect-sales-order-content/indirect-sales-order-content-master";
        public const string Detail = Module + "/indirect-sales-order-content/indirect-sales-order-content-detail";
        private const string Default = Rpc + Module + "/indirect-sales-order-content";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListIndirectSalesOrder = Default + "/filter-list-indirect-sales-order";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";

        public const string SingleListIndirectSalesOrder = Default + "/single-list-indirect-sales-order";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectSalesOrderContentFilter.Id), FieldType.ID },
            { nameof(IndirectSalesOrderContentFilter.IndirectSalesOrderId), FieldType.ID },
            { nameof(IndirectSalesOrderContentFilter.ItemId), FieldType.ID },
            { nameof(IndirectSalesOrderContentFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(IndirectSalesOrderContentFilter.Quantity), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.PrimaryUnitOfMeasureId), FieldType.ID },
            { nameof(IndirectSalesOrderContentFilter.RequestedQuantity), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.SalePrice), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.DiscountPercentage), FieldType.DECIMAL },
            { nameof(IndirectSalesOrderContentFilter.DiscountAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.GeneralDiscountPercentage), FieldType.DECIMAL },
            { nameof(IndirectSalesOrderContentFilter.GeneralDiscountAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.Amount), FieldType.LONG },
            { nameof(IndirectSalesOrderContentFilter.TaxPercentage), FieldType.DECIMAL },
            { nameof(IndirectSalesOrderContentFilter.TaxAmount), FieldType.LONG },
        };
    }

    public class IndirectSalesOrderContentController : RpcController
    {
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IIndirectSalesOrderContentService IndirectSalesOrderContentService;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderContentController(
            IIndirectSalesOrderService IndirectSalesOrderService,
            IUnitOfMeasureService UnitOfMeasureService,
            IIndirectSalesOrderContentService IndirectSalesOrderContentService,
            ICurrentContext CurrentContext
        )
        {
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.IndirectSalesOrderContentService = IndirectSalesOrderContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO);
            IndirectSalesOrderContentFilter = IndirectSalesOrderContentService.ToFilter(IndirectSalesOrderContentFilter);
            int count = await IndirectSalesOrderContentService.Count(IndirectSalesOrderContentFilter);
            return count;
        }

        [Route(IndirectSalesOrderContentRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrderContent_IndirectSalesOrderContentDTO>>> List([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO);
            IndirectSalesOrderContentFilter = IndirectSalesOrderContentService.ToFilter(IndirectSalesOrderContentFilter);
            List<IndirectSalesOrderContent> IndirectSalesOrderContents = await IndirectSalesOrderContentService.List(IndirectSalesOrderContentFilter);
            List<IndirectSalesOrderContent_IndirectSalesOrderContentDTO> IndirectSalesOrderContent_IndirectSalesOrderContentDTOs = IndirectSalesOrderContents
                .Select(c => new IndirectSalesOrderContent_IndirectSalesOrderContentDTO(c)).ToList();
            return IndirectSalesOrderContent_IndirectSalesOrderContentDTOs;
        }

        [Route(IndirectSalesOrderContentRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderContent_IndirectSalesOrderContentDTO>> Get([FromBody]IndirectSalesOrderContent_IndirectSalesOrderContentDTO IndirectSalesOrderContent_IndirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id))
                return Forbid();

            IndirectSalesOrderContent IndirectSalesOrderContent = await IndirectSalesOrderContentService.Get(IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id);
            return new IndirectSalesOrderContent_IndirectSalesOrderContentDTO(IndirectSalesOrderContent);
        }

        [Route(IndirectSalesOrderContentRoute.Create), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderContent_IndirectSalesOrderContentDTO>> Create([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentDTO IndirectSalesOrderContent_IndirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id))
                return Forbid();

            IndirectSalesOrderContent IndirectSalesOrderContent = ConvertDTOToEntity(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
            IndirectSalesOrderContent = await IndirectSalesOrderContentService.Create(IndirectSalesOrderContent);
            IndirectSalesOrderContent_IndirectSalesOrderContentDTO = new IndirectSalesOrderContent_IndirectSalesOrderContentDTO(IndirectSalesOrderContent);
            if (IndirectSalesOrderContent.IsValidated)
                return IndirectSalesOrderContent_IndirectSalesOrderContentDTO;
            else
                return BadRequest(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
        }

        [Route(IndirectSalesOrderContentRoute.Update), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderContent_IndirectSalesOrderContentDTO>> Update([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentDTO IndirectSalesOrderContent_IndirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id))
                return Forbid();

            IndirectSalesOrderContent IndirectSalesOrderContent = ConvertDTOToEntity(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
            IndirectSalesOrderContent = await IndirectSalesOrderContentService.Update(IndirectSalesOrderContent);
            IndirectSalesOrderContent_IndirectSalesOrderContentDTO = new IndirectSalesOrderContent_IndirectSalesOrderContentDTO(IndirectSalesOrderContent);
            if (IndirectSalesOrderContent.IsValidated)
                return IndirectSalesOrderContent_IndirectSalesOrderContentDTO;
            else
                return BadRequest(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
        }

        [Route(IndirectSalesOrderContentRoute.Delete), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderContent_IndirectSalesOrderContentDTO>> Delete([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentDTO IndirectSalesOrderContent_IndirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id))
                return Forbid();

            IndirectSalesOrderContent IndirectSalesOrderContent = ConvertDTOToEntity(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
            IndirectSalesOrderContent = await IndirectSalesOrderContentService.Delete(IndirectSalesOrderContent);
            IndirectSalesOrderContent_IndirectSalesOrderContentDTO = new IndirectSalesOrderContent_IndirectSalesOrderContentDTO(IndirectSalesOrderContent);
            if (IndirectSalesOrderContent.IsValidated)
                return IndirectSalesOrderContent_IndirectSalesOrderContentDTO;
            else
                return BadRequest(IndirectSalesOrderContent_IndirectSalesOrderContentDTO);
        }
        
        [Route(IndirectSalesOrderContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
            IndirectSalesOrderContentFilter = IndirectSalesOrderContentService.ToFilter(IndirectSalesOrderContentFilter);
            IndirectSalesOrderContentFilter.Id = new IdFilter { In = Ids };
            IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.Id;
            IndirectSalesOrderContentFilter.Skip = 0;
            IndirectSalesOrderContentFilter.Take = int.MaxValue;

            List<IndirectSalesOrderContent> IndirectSalesOrderContents = await IndirectSalesOrderContentService.List(IndirectSalesOrderContentFilter);
            IndirectSalesOrderContents = await IndirectSalesOrderContentService.BulkDelete(IndirectSalesOrderContents);
            return true;
        }
        
        [Route(IndirectSalesOrderContentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectSalesOrderSelect.ALL
            };
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            UnitOfMeasureFilter PrimaryUnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL
            };
            List<UnitOfMeasure> PrimaryUnitOfMeasures = await UnitOfMeasureService.List(PrimaryUnitOfMeasureFilter);
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL
            };
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrderContent> IndirectSalesOrderContents = new List<IndirectSalesOrderContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(IndirectSalesOrderContents);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int IndirectSalesOrderIdColumn = 1 + StartColumn;
                int ItemIdColumn = 2 + StartColumn;
                int UnitOfMeasureIdColumn = 3 + StartColumn;
                int QuantityColumn = 4 + StartColumn;
                int PrimaryUnitOfMeasureIdColumn = 5 + StartColumn;
                int RequestedQuantityColumn = 6 + StartColumn;
                int SalePriceColumn = 7 + StartColumn;
                int DiscountPercentageColumn = 8 + StartColumn;
                int DiscountAmountColumn = 9 + StartColumn;
                int GeneralDiscountPercentageColumn = 10 + StartColumn;
                int GeneralDiscountAmountColumn = 11 + StartColumn;
                int AmountColumn = 12 + StartColumn;
                int TaxPercentageColumn = 13 + StartColumn;
                int TaxAmountColumn = 14 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string IndirectSalesOrderIdValue = worksheet.Cells[i + StartRow, IndirectSalesOrderIdColumn].Value?.ToString();
                    string ItemIdValue = worksheet.Cells[i + StartRow, ItemIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string QuantityValue = worksheet.Cells[i + StartRow, QuantityColumn].Value?.ToString();
                    string PrimaryUnitOfMeasureIdValue = worksheet.Cells[i + StartRow, PrimaryUnitOfMeasureIdColumn].Value?.ToString();
                    string RequestedQuantityValue = worksheet.Cells[i + StartRow, RequestedQuantityColumn].Value?.ToString();
                    string SalePriceValue = worksheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                    string DiscountPercentageValue = worksheet.Cells[i + StartRow, DiscountPercentageColumn].Value?.ToString();
                    string DiscountAmountValue = worksheet.Cells[i + StartRow, DiscountAmountColumn].Value?.ToString();
                    string GeneralDiscountPercentageValue = worksheet.Cells[i + StartRow, GeneralDiscountPercentageColumn].Value?.ToString();
                    string GeneralDiscountAmountValue = worksheet.Cells[i + StartRow, GeneralDiscountAmountColumn].Value?.ToString();
                    string AmountValue = worksheet.Cells[i + StartRow, AmountColumn].Value?.ToString();
                    string TaxPercentageValue = worksheet.Cells[i + StartRow, TaxPercentageColumn].Value?.ToString();
                    string TaxAmountValue = worksheet.Cells[i + StartRow, TaxAmountColumn].Value?.ToString();
                    
                    IndirectSalesOrderContent IndirectSalesOrderContent = new IndirectSalesOrderContent();
                    IndirectSalesOrderContent.Quantity = long.TryParse(QuantityValue, out long Quantity) ? Quantity : 0;
                    IndirectSalesOrderContent.RequestedQuantity = long.TryParse(RequestedQuantityValue, out long RequestedQuantity) ? RequestedQuantity : 0;
                    IndirectSalesOrderContent.SalePrice = long.TryParse(SalePriceValue, out long SalePrice) ? SalePrice : 0;
                    IndirectSalesOrderContent.DiscountPercentage = decimal.TryParse(DiscountPercentageValue, out decimal DiscountPercentage) ? DiscountPercentage : 0;
                    IndirectSalesOrderContent.DiscountAmount = long.TryParse(DiscountAmountValue, out long DiscountAmount) ? DiscountAmount : 0;
                    IndirectSalesOrderContent.GeneralDiscountPercentage = decimal.TryParse(GeneralDiscountPercentageValue, out decimal GeneralDiscountPercentage) ? GeneralDiscountPercentage : 0;
                    IndirectSalesOrderContent.GeneralDiscountAmount = long.TryParse(GeneralDiscountAmountValue, out long GeneralDiscountAmount) ? GeneralDiscountAmount : 0;
                    IndirectSalesOrderContent.Amount = long.TryParse(AmountValue, out long Amount) ? Amount : 0;
                    IndirectSalesOrderContent.TaxPercentage = decimal.TryParse(TaxPercentageValue, out decimal TaxPercentage) ? TaxPercentage : 0;
                    IndirectSalesOrderContent.TaxAmount = long.TryParse(TaxAmountValue, out long TaxAmount) ? TaxAmount : 0;
                    IndirectSalesOrder IndirectSalesOrder = IndirectSalesOrders.Where(x => x.Id.ToString() == IndirectSalesOrderIdValue).FirstOrDefault();
                    IndirectSalesOrderContent.IndirectSalesOrderId = IndirectSalesOrder == null ? 0 : IndirectSalesOrder.Id;
                    IndirectSalesOrderContent.IndirectSalesOrder = IndirectSalesOrder;
                    UnitOfMeasure PrimaryUnitOfMeasure = PrimaryUnitOfMeasures.Where(x => x.Id.ToString() == PrimaryUnitOfMeasureIdValue).FirstOrDefault();
                    IndirectSalesOrderContent.PrimaryUnitOfMeasureId = PrimaryUnitOfMeasure == null ? 0 : PrimaryUnitOfMeasure.Id;
                    IndirectSalesOrderContent.PrimaryUnitOfMeasure = PrimaryUnitOfMeasure;
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures.Where(x => x.Id.ToString() == UnitOfMeasureIdValue).FirstOrDefault();
                    IndirectSalesOrderContent.UnitOfMeasureId = UnitOfMeasure == null ? 0 : UnitOfMeasure.Id;
                    IndirectSalesOrderContent.UnitOfMeasure = UnitOfMeasure;
                    
                    IndirectSalesOrderContents.Add(IndirectSalesOrderContent);
                }
            }
            IndirectSalesOrderContents = await IndirectSalesOrderContentService.Import(IndirectSalesOrderContents);
            if (IndirectSalesOrderContents.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < IndirectSalesOrderContents.Count; i++)
                {
                    IndirectSalesOrderContent IndirectSalesOrderContent = IndirectSalesOrderContents[i];
                    if (!IndirectSalesOrderContent.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.Id)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.Id)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.IndirectSalesOrderId)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.IndirectSalesOrderId)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.ItemId)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.ItemId)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.UnitOfMeasureId)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.UnitOfMeasureId)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.Quantity)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.Quantity)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.PrimaryUnitOfMeasureId)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.PrimaryUnitOfMeasureId)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.RequestedQuantity)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.RequestedQuantity)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.SalePrice)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.SalePrice)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.DiscountPercentage)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.DiscountPercentage)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.DiscountAmount)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.DiscountAmount)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.GeneralDiscountPercentage)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.GeneralDiscountPercentage)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.GeneralDiscountAmount)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.GeneralDiscountAmount)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.Amount)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.Amount)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.TaxPercentage)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.TaxPercentage)];
                        if (IndirectSalesOrderContent.Errors.ContainsKey(nameof(IndirectSalesOrderContent.TaxAmount)))
                            Error += IndirectSalesOrderContent.Errors[nameof(IndirectSalesOrderContent.TaxAmount)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(IndirectSalesOrderContentRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrderContent
                var IndirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO);
                IndirectSalesOrderContentFilter.Skip = 0;
                IndirectSalesOrderContentFilter.Take = int.MaxValue;
                IndirectSalesOrderContentFilter = IndirectSalesOrderContentService.ToFilter(IndirectSalesOrderContentFilter);
                List<IndirectSalesOrderContent> IndirectSalesOrderContents = await IndirectSalesOrderContentService.List(IndirectSalesOrderContentFilter);

                var IndirectSalesOrderContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "IndirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "SalePrice",
                        "DiscountPercentage",
                        "DiscountAmount",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "Amount",
                        "TaxPercentage",
                        "TaxAmount",
                    }
                };
                List<object[]> IndirectSalesOrderContentData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrderContents.Count; i++)
                {
                    var IndirectSalesOrderContent = IndirectSalesOrderContents[i];
                    IndirectSalesOrderContentData.Add(new Object[]
                    {
                        IndirectSalesOrderContent.Id,
                        IndirectSalesOrderContent.IndirectSalesOrderId,
                        IndirectSalesOrderContent.ItemId,
                        IndirectSalesOrderContent.UnitOfMeasureId,
                        IndirectSalesOrderContent.Quantity,
                        IndirectSalesOrderContent.PrimaryUnitOfMeasureId,
                        IndirectSalesOrderContent.RequestedQuantity,
                        IndirectSalesOrderContent.SalePrice,
                        IndirectSalesOrderContent.DiscountPercentage,
                        IndirectSalesOrderContent.DiscountAmount,
                        IndirectSalesOrderContent.GeneralDiscountPercentage,
                        IndirectSalesOrderContent.GeneralDiscountAmount,
                        IndirectSalesOrderContent.Amount,
                        IndirectSalesOrderContent.TaxPercentage,
                        IndirectSalesOrderContent.TaxAmount,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrderContent", IndirectSalesOrderContentHeaders, IndirectSalesOrderContentData);
                #endregion
                
                #region IndirectSalesOrder
                var IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
                IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
                IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
                IndirectSalesOrderFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderFilter.Skip = 0;
                IndirectSalesOrderFilter.Take = int.MaxValue;
                List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);

                var IndirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "BuyerStoreId",
                        "PhoneNumber",
                        "StoreAddress",
                        "DeliveryAddress",
                        "SellerStoreId",
                        "SaleEmployeeId",
                        "OrderDate",
                        "DeliveryDate",
                        "IndirectSalesOrderStatusId",
                        "EditedPriceStatusId",
                        "Note",
                        "SubTotal",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TotalTaxAmount",
                        "Total",
                    }
                };
                List<object[]> IndirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrders.Count; i++)
                {
                    var IndirectSalesOrder = IndirectSalesOrders[i];
                    IndirectSalesOrderData.Add(new Object[]
                    {
                        IndirectSalesOrder.Id,
                        IndirectSalesOrder.Code,
                        IndirectSalesOrder.BuyerStoreId,
                        IndirectSalesOrder.PhoneNumber,
                        IndirectSalesOrder.StoreAddress,
                        IndirectSalesOrder.DeliveryAddress,
                        IndirectSalesOrder.SellerStoreId,
                        IndirectSalesOrder.SaleEmployeeId,
                        IndirectSalesOrder.OrderDate,
                        IndirectSalesOrder.DeliveryDate,
                        IndirectSalesOrder.IndirectSalesOrderStatusId,
                        IndirectSalesOrder.EditedPriceStatusId,
                        IndirectSalesOrder.Note,
                        IndirectSalesOrder.SubTotal,
                        IndirectSalesOrder.GeneralDiscountPercentage,
                        IndirectSalesOrder.GeneralDiscountAmount,
                        IndirectSalesOrder.TotalTaxAmount,
                        IndirectSalesOrder.Total,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrder", IndirectSalesOrderHeaders, IndirectSalesOrderData);
                #endregion
                #region UnitOfMeasure
                var UnitOfMeasureFilter = new UnitOfMeasureFilter();
                UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
                UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
                UnitOfMeasureFilter.OrderType = OrderType.ASC;
                UnitOfMeasureFilter.Skip = 0;
                UnitOfMeasureFilter.Take = int.MaxValue;
                List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);

                var UnitOfMeasureHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Description",
                        "StatusId",
                    }
                };
                List<object[]> UnitOfMeasureData = new List<object[]>();
                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    var UnitOfMeasure = UnitOfMeasures[i];
                    UnitOfMeasureData.Add(new Object[]
                    {
                        UnitOfMeasure.Id,
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name,
                        UnitOfMeasure.Description,
                        UnitOfMeasure.StatusId,
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UnitOfMeasureHeaders, UnitOfMeasureData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrderContent.xlsx");
        }

        [Route(IndirectSalesOrderContentRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrderContent
                var IndirectSalesOrderContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "IndirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "SalePrice",
                        "DiscountPercentage",
                        "DiscountAmount",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "Amount",
                        "TaxPercentage",
                        "TaxAmount",
                    }
                };
                List<object[]> IndirectSalesOrderContentData = new List<object[]>();
                excel.GenerateWorksheet("IndirectSalesOrderContent", IndirectSalesOrderContentHeaders, IndirectSalesOrderContentData);
                #endregion
                
                #region IndirectSalesOrder
                var IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
                IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
                IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
                IndirectSalesOrderFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderFilter.Skip = 0;
                IndirectSalesOrderFilter.Take = int.MaxValue;
                List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);

                var IndirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "BuyerStoreId",
                        "PhoneNumber",
                        "StoreAddress",
                        "DeliveryAddress",
                        "SellerStoreId",
                        "SaleEmployeeId",
                        "OrderDate",
                        "DeliveryDate",
                        "IndirectSalesOrderStatusId",
                        "EditedPriceStatusId",
                        "Note",
                        "SubTotal",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TotalTaxAmount",
                        "Total",
                    }
                };
                List<object[]> IndirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrders.Count; i++)
                {
                    var IndirectSalesOrder = IndirectSalesOrders[i];
                    IndirectSalesOrderData.Add(new Object[]
                    {
                        IndirectSalesOrder.Id,
                        IndirectSalesOrder.Code,
                        IndirectSalesOrder.BuyerStoreId,
                        IndirectSalesOrder.PhoneNumber,
                        IndirectSalesOrder.StoreAddress,
                        IndirectSalesOrder.DeliveryAddress,
                        IndirectSalesOrder.SellerStoreId,
                        IndirectSalesOrder.SaleEmployeeId,
                        IndirectSalesOrder.OrderDate,
                        IndirectSalesOrder.DeliveryDate,
                        IndirectSalesOrder.IndirectSalesOrderStatusId,
                        IndirectSalesOrder.EditedPriceStatusId,
                        IndirectSalesOrder.Note,
                        IndirectSalesOrder.SubTotal,
                        IndirectSalesOrder.GeneralDiscountPercentage,
                        IndirectSalesOrder.GeneralDiscountAmount,
                        IndirectSalesOrder.TotalTaxAmount,
                        IndirectSalesOrder.Total,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrder", IndirectSalesOrderHeaders, IndirectSalesOrderData);
                #endregion
                #region UnitOfMeasure
                var UnitOfMeasureFilter = new UnitOfMeasureFilter();
                UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
                UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
                UnitOfMeasureFilter.OrderType = OrderType.ASC;
                UnitOfMeasureFilter.Skip = 0;
                UnitOfMeasureFilter.Take = int.MaxValue;
                List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);

                var UnitOfMeasureHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "Description",
                        "StatusId",
                    }
                };
                List<object[]> UnitOfMeasureData = new List<object[]>();
                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    var UnitOfMeasure = UnitOfMeasures[i];
                    UnitOfMeasureData.Add(new Object[]
                    {
                        UnitOfMeasure.Id,
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name,
                        UnitOfMeasure.Description,
                        UnitOfMeasure.StatusId,
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UnitOfMeasureHeaders, UnitOfMeasureData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrderContent.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
            IndirectSalesOrderContentFilter = IndirectSalesOrderContentService.ToFilter(IndirectSalesOrderContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                IndirectSalesOrderContentFilter.Id = new IdFilter { Equal = Id };
                int count = await IndirectSalesOrderContentService.Count(IndirectSalesOrderContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private IndirectSalesOrderContent ConvertDTOToEntity(IndirectSalesOrderContent_IndirectSalesOrderContentDTO IndirectSalesOrderContent_IndirectSalesOrderContentDTO)
        {
            IndirectSalesOrderContent IndirectSalesOrderContent = new IndirectSalesOrderContent();
            IndirectSalesOrderContent.Id = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Id;
            IndirectSalesOrderContent.IndirectSalesOrderId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrderId;
            IndirectSalesOrderContent.ItemId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.ItemId;
            IndirectSalesOrderContent.UnitOfMeasureId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasureId;
            IndirectSalesOrderContent.Quantity = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Quantity;
            IndirectSalesOrderContent.PrimaryUnitOfMeasureId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderContent.RequestedQuantity = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.RequestedQuantity;
            IndirectSalesOrderContent.SalePrice = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.SalePrice;
            IndirectSalesOrderContent.DiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.DiscountPercentage;
            IndirectSalesOrderContent.DiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.DiscountAmount;
            IndirectSalesOrderContent.GeneralDiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.GeneralDiscountPercentage;
            IndirectSalesOrderContent.GeneralDiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.GeneralDiscountAmount;
            IndirectSalesOrderContent.Amount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.Amount;
            IndirectSalesOrderContent.TaxPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.TaxPercentage;
            IndirectSalesOrderContent.TaxAmount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.TaxAmount;
            IndirectSalesOrderContent.IndirectSalesOrder = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder == null ? null : new IndirectSalesOrder
            {
                Id = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.Id,
                Code = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.Code,
                BuyerStoreId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.BuyerStoreId,
                PhoneNumber = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.PhoneNumber,
                StoreAddress = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.StoreAddress,
                DeliveryAddress = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.DeliveryAddress,
                SellerStoreId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.SellerStoreId,
                SaleEmployeeId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.SaleEmployeeId,
                OrderDate = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.OrderDate,
                DeliveryDate = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.DeliveryDate,
                IndirectSalesOrderStatusId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.IndirectSalesOrderStatusId,
                EditedPriceStatusId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.EditedPriceStatusId,
                Note = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.Note,
                SubTotal = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.SubTotal,
                GeneralDiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.GeneralDiscountPercentage,
                GeneralDiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.GeneralDiscountAmount,
                TotalTaxAmount = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.TotalTaxAmount,
                Total = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.IndirectSalesOrder.Total,
            };
            IndirectSalesOrderContent.PrimaryUnitOfMeasure = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Id,
                Code = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Code,
                Name = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Name,
                Description = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Description,
                StatusId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.PrimaryUnitOfMeasure.StatusId,
            };
            IndirectSalesOrderContent.UnitOfMeasure = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure.Id,
                Code = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure.Code,
                Name = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure.Name,
                Description = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure.Description,
                StatusId = IndirectSalesOrderContent_IndirectSalesOrderContentDTO.UnitOfMeasure.StatusId,
            };
            IndirectSalesOrderContent.BaseLanguage = CurrentContext.Language;
            return IndirectSalesOrderContent;
        }

        private IndirectSalesOrderContentFilter ConvertFilterDTOToFilterEntity(IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
            IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.ALL;
            IndirectSalesOrderContentFilter.Skip = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.Skip;
            IndirectSalesOrderContentFilter.Take = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.Take;
            IndirectSalesOrderContentFilter.OrderBy = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.OrderBy;
            IndirectSalesOrderContentFilter.OrderType = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.OrderType;

            IndirectSalesOrderContentFilter.Id = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.Id;
            IndirectSalesOrderContentFilter.IndirectSalesOrderId = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderContentFilter.ItemId = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.ItemId;
            IndirectSalesOrderContentFilter.UnitOfMeasureId = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderContentFilter.Quantity = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.Quantity;
            IndirectSalesOrderContentFilter.PrimaryUnitOfMeasureId = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderContentFilter.RequestedQuantity = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.RequestedQuantity;
            IndirectSalesOrderContentFilter.SalePrice = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.SalePrice;
            IndirectSalesOrderContentFilter.DiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.DiscountPercentage;
            IndirectSalesOrderContentFilter.DiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.DiscountAmount;
            IndirectSalesOrderContentFilter.GeneralDiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderContentFilter.GeneralDiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderContentFilter.Amount = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.Amount;
            IndirectSalesOrderContentFilter.TaxPercentage = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.TaxPercentage;
            IndirectSalesOrderContentFilter.TaxAmount = IndirectSalesOrderContent_IndirectSalesOrderContentFilterDTO.TaxAmount;
            return IndirectSalesOrderContentFilter;
        }

        [Route(IndirectSalesOrderContentRoute.FilterListIndirectSalesOrder), HttpPost]
        public async Task<List<IndirectSalesOrderContent_IndirectSalesOrderDTO>> FilterListIndirectSalesOrder([FromBody] IndirectSalesOrderContent_IndirectSalesOrderFilterDTO IndirectSalesOrderContent_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = 20;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
            IndirectSalesOrderFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Id = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.IndirectSalesOrderStatusId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Total;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrderContent_IndirectSalesOrderDTO> IndirectSalesOrderContent_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new IndirectSalesOrderContent_IndirectSalesOrderDTO(x)).ToList();
            return IndirectSalesOrderContent_IndirectSalesOrderDTOs;
        }
        [Route(IndirectSalesOrderContentRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrderContent_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] IndirectSalesOrderContent_UnitOfMeasureFilterDTO IndirectSalesOrderContent_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrderContent_UnitOfMeasureDTO> IndirectSalesOrderContent_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrderContent_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrderContent_UnitOfMeasureDTOs;
        }

        [Route(IndirectSalesOrderContentRoute.SingleListIndirectSalesOrder), HttpPost]
        public async Task<List<IndirectSalesOrderContent_IndirectSalesOrderDTO>> SingleListIndirectSalesOrder([FromBody] IndirectSalesOrderContent_IndirectSalesOrderFilterDTO IndirectSalesOrderContent_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = 20;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
            IndirectSalesOrderFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Id = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.IndirectSalesOrderStatusId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrderContent_IndirectSalesOrderFilterDTO.Total;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrderContent_IndirectSalesOrderDTO> IndirectSalesOrderContent_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new IndirectSalesOrderContent_IndirectSalesOrderDTO(x)).ToList();
            return IndirectSalesOrderContent_IndirectSalesOrderDTOs;
        }
        [Route(IndirectSalesOrderContentRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrderContent_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] IndirectSalesOrderContent_UnitOfMeasureFilterDTO IndirectSalesOrderContent_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrderContent_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrderContent_UnitOfMeasureDTO> IndirectSalesOrderContent_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrderContent_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrderContent_UnitOfMeasureDTOs;
        }

    }
}

