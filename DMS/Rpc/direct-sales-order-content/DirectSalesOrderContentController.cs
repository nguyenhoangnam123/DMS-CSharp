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
using DMS.Services.MDirectSalesOrderContent;
using DMS.Services.MDirectSalesOrder;
using DMS.Services.MItem;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.direct_sales_order_content
{
    public class DirectSalesOrderContentRoute : Root
    {
        public const string Master = Module + "/direct-sales-order-content/direct-sales-order-content-master";
        public const string Detail = Module + "/direct-sales-order-content/direct-sales-order-content-detail";
        private const string Default = Rpc + Module + "/direct-sales-order-content";
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
        
        
        public const string FilterListDirectSalesOrder = Default + "/filter-list-direct-sales-order";
        
        public const string FilterListItem = Default + "/filter-list-item";
        
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        

        
        public const string SingleListDirectSalesOrder = Default + "/single-list-direct-sales-order";
        
        public const string SingleListItem = Default + "/single-list-item";
        
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(DirectSalesOrderContentFilter.Id), FieldType.ID },
            { nameof(DirectSalesOrderContentFilter.DirectSalesOrderId), FieldType.ID },
            { nameof(DirectSalesOrderContentFilter.ItemId), FieldType.ID },
            { nameof(DirectSalesOrderContentFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(DirectSalesOrderContentFilter.Quantity), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.PrimaryUnitOfMeasureId), FieldType.ID },
            { nameof(DirectSalesOrderContentFilter.RequestedQuantity), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.Price), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.DiscountPercentage), FieldType.DECIMAL },
            { nameof(DirectSalesOrderContentFilter.DiscountAmount), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.GeneralDiscountPercentage), FieldType.DECIMAL },
            { nameof(DirectSalesOrderContentFilter.GeneralDiscountAmount), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.TaxPercentage), FieldType.DECIMAL },
            { nameof(DirectSalesOrderContentFilter.TaxAmount), FieldType.LONG },
            { nameof(DirectSalesOrderContentFilter.Amount), FieldType.LONG },
        };
    }

    public class DirectSalesOrderContentController : RpcController
    {
        private IDirectSalesOrderService DirectSalesOrderService;
        private IItemService ItemService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IDirectSalesOrderContentService DirectSalesOrderContentService;
        private ICurrentContext CurrentContext;
        public DirectSalesOrderContentController(
            IDirectSalesOrderService DirectSalesOrderService,
            IItemService ItemService,
            IUnitOfMeasureService UnitOfMeasureService,
            IDirectSalesOrderContentService DirectSalesOrderContentService,
            ICurrentContext CurrentContext
        )
        {
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.ItemService = ItemService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.DirectSalesOrderContentService = DirectSalesOrderContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectSalesOrderContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectSalesOrderContent_DirectSalesOrderContentFilterDTO DirectSalesOrderContent_DirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderContent_DirectSalesOrderContentFilterDTO);
            DirectSalesOrderContentFilter = DirectSalesOrderContentService.ToFilter(DirectSalesOrderContentFilter);
            int count = await DirectSalesOrderContentService.Count(DirectSalesOrderContentFilter);
            return count;
        }

        [Route(DirectSalesOrderContentRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectSalesOrderContent_DirectSalesOrderContentDTO>>> List([FromBody] DirectSalesOrderContent_DirectSalesOrderContentFilterDTO DirectSalesOrderContent_DirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderContent_DirectSalesOrderContentFilterDTO);
            DirectSalesOrderContentFilter = DirectSalesOrderContentService.ToFilter(DirectSalesOrderContentFilter);
            List<DirectSalesOrderContent> DirectSalesOrderContents = await DirectSalesOrderContentService.List(DirectSalesOrderContentFilter);
            List<DirectSalesOrderContent_DirectSalesOrderContentDTO> DirectSalesOrderContent_DirectSalesOrderContentDTOs = DirectSalesOrderContents
                .Select(c => new DirectSalesOrderContent_DirectSalesOrderContentDTO(c)).ToList();
            return DirectSalesOrderContent_DirectSalesOrderContentDTOs;
        }

        [Route(DirectSalesOrderContentRoute.Get), HttpPost]
        public async Task<ActionResult<DirectSalesOrderContent_DirectSalesOrderContentDTO>> Get([FromBody]DirectSalesOrderContent_DirectSalesOrderContentDTO DirectSalesOrderContent_DirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrderContent_DirectSalesOrderContentDTO.Id))
                return Forbid();

            DirectSalesOrderContent DirectSalesOrderContent = await DirectSalesOrderContentService.Get(DirectSalesOrderContent_DirectSalesOrderContentDTO.Id);
            return new DirectSalesOrderContent_DirectSalesOrderContentDTO(DirectSalesOrderContent);
        }

        [Route(DirectSalesOrderContentRoute.Create), HttpPost]
        public async Task<ActionResult<DirectSalesOrderContent_DirectSalesOrderContentDTO>> Create([FromBody] DirectSalesOrderContent_DirectSalesOrderContentDTO DirectSalesOrderContent_DirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrderContent_DirectSalesOrderContentDTO.Id))
                return Forbid();

            DirectSalesOrderContent DirectSalesOrderContent = ConvertDTOToEntity(DirectSalesOrderContent_DirectSalesOrderContentDTO);
            DirectSalesOrderContent = await DirectSalesOrderContentService.Create(DirectSalesOrderContent);
            DirectSalesOrderContent_DirectSalesOrderContentDTO = new DirectSalesOrderContent_DirectSalesOrderContentDTO(DirectSalesOrderContent);
            if (DirectSalesOrderContent.IsValidated)
                return DirectSalesOrderContent_DirectSalesOrderContentDTO;
            else
                return BadRequest(DirectSalesOrderContent_DirectSalesOrderContentDTO);
        }

        [Route(DirectSalesOrderContentRoute.Update), HttpPost]
        public async Task<ActionResult<DirectSalesOrderContent_DirectSalesOrderContentDTO>> Update([FromBody] DirectSalesOrderContent_DirectSalesOrderContentDTO DirectSalesOrderContent_DirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrderContent_DirectSalesOrderContentDTO.Id))
                return Forbid();

            DirectSalesOrderContent DirectSalesOrderContent = ConvertDTOToEntity(DirectSalesOrderContent_DirectSalesOrderContentDTO);
            DirectSalesOrderContent = await DirectSalesOrderContentService.Update(DirectSalesOrderContent);
            DirectSalesOrderContent_DirectSalesOrderContentDTO = new DirectSalesOrderContent_DirectSalesOrderContentDTO(DirectSalesOrderContent);
            if (DirectSalesOrderContent.IsValidated)
                return DirectSalesOrderContent_DirectSalesOrderContentDTO;
            else
                return BadRequest(DirectSalesOrderContent_DirectSalesOrderContentDTO);
        }

        [Route(DirectSalesOrderContentRoute.Delete), HttpPost]
        public async Task<ActionResult<DirectSalesOrderContent_DirectSalesOrderContentDTO>> Delete([FromBody] DirectSalesOrderContent_DirectSalesOrderContentDTO DirectSalesOrderContent_DirectSalesOrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrderContent_DirectSalesOrderContentDTO.Id))
                return Forbid();

            DirectSalesOrderContent DirectSalesOrderContent = ConvertDTOToEntity(DirectSalesOrderContent_DirectSalesOrderContentDTO);
            DirectSalesOrderContent = await DirectSalesOrderContentService.Delete(DirectSalesOrderContent);
            DirectSalesOrderContent_DirectSalesOrderContentDTO = new DirectSalesOrderContent_DirectSalesOrderContentDTO(DirectSalesOrderContent);
            if (DirectSalesOrderContent.IsValidated)
                return DirectSalesOrderContent_DirectSalesOrderContentDTO;
            else
                return BadRequest(DirectSalesOrderContent_DirectSalesOrderContentDTO);
        }
        
        [Route(DirectSalesOrderContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter();
            DirectSalesOrderContentFilter = DirectSalesOrderContentService.ToFilter(DirectSalesOrderContentFilter);
            DirectSalesOrderContentFilter.Id = new IdFilter { In = Ids };
            DirectSalesOrderContentFilter.Selects = DirectSalesOrderContentSelect.Id;
            DirectSalesOrderContentFilter.Skip = 0;
            DirectSalesOrderContentFilter.Take = int.MaxValue;

            List<DirectSalesOrderContent> DirectSalesOrderContents = await DirectSalesOrderContentService.List(DirectSalesOrderContentFilter);
            DirectSalesOrderContents = await DirectSalesOrderContentService.BulkDelete(DirectSalesOrderContents);
            return true;
        }
        
        [Route(DirectSalesOrderContentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DirectSalesOrderSelect.ALL
            };
            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            ItemFilter ItemFilter = new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL
            };
            List<Item> Items = await ItemService.List(ItemFilter);
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
            List<DirectSalesOrderContent> DirectSalesOrderContents = new List<DirectSalesOrderContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(DirectSalesOrderContents);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int DirectSalesOrderIdColumn = 1 + StartColumn;
                int ItemIdColumn = 2 + StartColumn;
                int UnitOfMeasureIdColumn = 3 + StartColumn;
                int QuantityColumn = 4 + StartColumn;
                int PrimaryUnitOfMeasureIdColumn = 5 + StartColumn;
                int RequestedQuantityColumn = 6 + StartColumn;
                int PriceColumn = 7 + StartColumn;
                int DiscountPercentageColumn = 8 + StartColumn;
                int DiscountAmountColumn = 9 + StartColumn;
                int GeneralDiscountPercentageColumn = 10 + StartColumn;
                int GeneralDiscountAmountColumn = 11 + StartColumn;
                int TaxPercentageColumn = 12 + StartColumn;
                int TaxAmountColumn = 13 + StartColumn;
                int AmountColumn = 14 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string DirectSalesOrderIdValue = worksheet.Cells[i + StartRow, DirectSalesOrderIdColumn].Value?.ToString();
                    string ItemIdValue = worksheet.Cells[i + StartRow, ItemIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string QuantityValue = worksheet.Cells[i + StartRow, QuantityColumn].Value?.ToString();
                    string PrimaryUnitOfMeasureIdValue = worksheet.Cells[i + StartRow, PrimaryUnitOfMeasureIdColumn].Value?.ToString();
                    string RequestedQuantityValue = worksheet.Cells[i + StartRow, RequestedQuantityColumn].Value?.ToString();
                    string PriceValue = worksheet.Cells[i + StartRow, PriceColumn].Value?.ToString();
                    string DiscountPercentageValue = worksheet.Cells[i + StartRow, DiscountPercentageColumn].Value?.ToString();
                    string DiscountAmountValue = worksheet.Cells[i + StartRow, DiscountAmountColumn].Value?.ToString();
                    string GeneralDiscountPercentageValue = worksheet.Cells[i + StartRow, GeneralDiscountPercentageColumn].Value?.ToString();
                    string GeneralDiscountAmountValue = worksheet.Cells[i + StartRow, GeneralDiscountAmountColumn].Value?.ToString();
                    string TaxPercentageValue = worksheet.Cells[i + StartRow, TaxPercentageColumn].Value?.ToString();
                    string TaxAmountValue = worksheet.Cells[i + StartRow, TaxAmountColumn].Value?.ToString();
                    string AmountValue = worksheet.Cells[i + StartRow, AmountColumn].Value?.ToString();
                    
                    DirectSalesOrderContent DirectSalesOrderContent = new DirectSalesOrderContent();
                    DirectSalesOrderContent.Quantity = long.TryParse(QuantityValue, out long Quantity) ? Quantity : 0;
                    DirectSalesOrderContent.RequestedQuantity = long.TryParse(RequestedQuantityValue, out long RequestedQuantity) ? RequestedQuantity : 0;
                    DirectSalesOrderContent.Price = long.TryParse(PriceValue, out long Price) ? Price : 0;
                    DirectSalesOrderContent.DiscountPercentage = decimal.TryParse(DiscountPercentageValue, out decimal DiscountPercentage) ? DiscountPercentage : 0;
                    DirectSalesOrderContent.DiscountAmount = long.TryParse(DiscountAmountValue, out long DiscountAmount) ? DiscountAmount : 0;
                    DirectSalesOrderContent.GeneralDiscountPercentage = decimal.TryParse(GeneralDiscountPercentageValue, out decimal GeneralDiscountPercentage) ? GeneralDiscountPercentage : 0;
                    DirectSalesOrderContent.GeneralDiscountAmount = long.TryParse(GeneralDiscountAmountValue, out long GeneralDiscountAmount) ? GeneralDiscountAmount : 0;
                    DirectSalesOrderContent.TaxPercentage = decimal.TryParse(TaxPercentageValue, out decimal TaxPercentage) ? TaxPercentage : 0;
                    DirectSalesOrderContent.TaxAmount = long.TryParse(TaxAmountValue, out long TaxAmount) ? TaxAmount : 0;
                    DirectSalesOrderContent.Amount = long.TryParse(AmountValue, out long Amount) ? Amount : 0;
                    DirectSalesOrder DirectSalesOrder = DirectSalesOrders.Where(x => x.Id.ToString() == DirectSalesOrderIdValue).FirstOrDefault();
                    DirectSalesOrderContent.DirectSalesOrderId = DirectSalesOrder == null ? 0 : DirectSalesOrder.Id;
                    DirectSalesOrderContent.DirectSalesOrder = DirectSalesOrder;
                    Item Item = Items.Where(x => x.Id.ToString() == ItemIdValue).FirstOrDefault();
                    DirectSalesOrderContent.ItemId = Item == null ? 0 : Item.Id;
                    DirectSalesOrderContent.Item = Item;
                    UnitOfMeasure PrimaryUnitOfMeasure = PrimaryUnitOfMeasures.Where(x => x.Id.ToString() == PrimaryUnitOfMeasureIdValue).FirstOrDefault();
                    DirectSalesOrderContent.PrimaryUnitOfMeasureId = PrimaryUnitOfMeasure == null ? 0 : PrimaryUnitOfMeasure.Id;
                    DirectSalesOrderContent.PrimaryUnitOfMeasure = PrimaryUnitOfMeasure;
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures.Where(x => x.Id.ToString() == UnitOfMeasureIdValue).FirstOrDefault();
                    DirectSalesOrderContent.UnitOfMeasureId = UnitOfMeasure == null ? 0 : UnitOfMeasure.Id;
                    DirectSalesOrderContent.UnitOfMeasure = UnitOfMeasure;
                    
                    DirectSalesOrderContents.Add(DirectSalesOrderContent);
                }
            }
            DirectSalesOrderContents = await DirectSalesOrderContentService.Import(DirectSalesOrderContents);
            if (DirectSalesOrderContents.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < DirectSalesOrderContents.Count; i++)
                {
                    DirectSalesOrderContent DirectSalesOrderContent = DirectSalesOrderContents[i];
                    if (!DirectSalesOrderContent.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.Id)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.Id)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.DirectSalesOrderId)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.DirectSalesOrderId)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.ItemId)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.ItemId)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.UnitOfMeasureId)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.UnitOfMeasureId)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.Quantity)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.Quantity)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.PrimaryUnitOfMeasureId)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.PrimaryUnitOfMeasureId)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.RequestedQuantity)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.RequestedQuantity)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.Price)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.Price)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.DiscountPercentage)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.DiscountPercentage)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.DiscountAmount)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.DiscountAmount)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.GeneralDiscountPercentage)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.GeneralDiscountPercentage)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.GeneralDiscountAmount)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.GeneralDiscountAmount)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.TaxPercentage)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.TaxPercentage)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.TaxAmount)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.TaxAmount)];
                        if (DirectSalesOrderContent.Errors.ContainsKey(nameof(DirectSalesOrderContent.Amount)))
                            Error += DirectSalesOrderContent.Errors[nameof(DirectSalesOrderContent.Amount)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(DirectSalesOrderContentRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] DirectSalesOrderContent_DirectSalesOrderContentFilterDTO DirectSalesOrderContent_DirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrderContent
                var DirectSalesOrderContentFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderContent_DirectSalesOrderContentFilterDTO);
                DirectSalesOrderContentFilter.Skip = 0;
                DirectSalesOrderContentFilter.Take = int.MaxValue;
                DirectSalesOrderContentFilter = DirectSalesOrderContentService.ToFilter(DirectSalesOrderContentFilter);
                List<DirectSalesOrderContent> DirectSalesOrderContents = await DirectSalesOrderContentService.List(DirectSalesOrderContentFilter);

                var DirectSalesOrderContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "DirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Price",
                        "DiscountPercentage",
                        "DiscountAmount",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TaxPercentage",
                        "TaxAmount",
                        "Amount",
                    }
                };
                List<object[]> DirectSalesOrderContentData = new List<object[]>();
                for (int i = 0; i < DirectSalesOrderContents.Count; i++)
                {
                    var DirectSalesOrderContent = DirectSalesOrderContents[i];
                    DirectSalesOrderContentData.Add(new Object[]
                    {
                        DirectSalesOrderContent.Id,
                        DirectSalesOrderContent.DirectSalesOrderId,
                        DirectSalesOrderContent.ItemId,
                        DirectSalesOrderContent.UnitOfMeasureId,
                        DirectSalesOrderContent.Quantity,
                        DirectSalesOrderContent.PrimaryUnitOfMeasureId,
                        DirectSalesOrderContent.RequestedQuantity,
                        DirectSalesOrderContent.Price,
                        DirectSalesOrderContent.DiscountPercentage,
                        DirectSalesOrderContent.DiscountAmount,
                        DirectSalesOrderContent.GeneralDiscountPercentage,
                        DirectSalesOrderContent.GeneralDiscountAmount,
                        DirectSalesOrderContent.TaxPercentage,
                        DirectSalesOrderContent.TaxAmount,
                        DirectSalesOrderContent.Amount,
                    });
                }
                excel.GenerateWorksheet("DirectSalesOrderContent", DirectSalesOrderContentHeaders, DirectSalesOrderContentData);
                #endregion
                
                #region DirectSalesOrder
                var DirectSalesOrderFilter = new DirectSalesOrderFilter();
                DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
                DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
                DirectSalesOrderFilter.OrderType = OrderType.ASC;
                DirectSalesOrderFilter.Skip = 0;
                DirectSalesOrderFilter.Take = int.MaxValue;
                List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);

                var DirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "BuyerStoreId",
                        "StorePhone",
                        "StoreAddress",
                        "StoreDeliveryAddress",
                        "TaxCode",
                        "SaleEmployeeId",
                        "OrderDate",
                        "DeliveryDate",
                        "EditedPriceStatusId",
                        "Note",
                        "SubTotal",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TotalTaxAmount",
                        "Total",
                        "RequestStateId",
                    }
                };
                List<object[]> DirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < DirectSalesOrders.Count; i++)
                {
                    var DirectSalesOrder = DirectSalesOrders[i];
                    DirectSalesOrderData.Add(new Object[]
                    {
                        DirectSalesOrder.Id,
                        DirectSalesOrder.Code,
                        DirectSalesOrder.BuyerStoreId,
                        DirectSalesOrder.StorePhone,
                        DirectSalesOrder.StoreAddress,
                        DirectSalesOrder.StoreDeliveryAddress,
                        DirectSalesOrder.TaxCode,
                        DirectSalesOrder.SaleEmployeeId,
                        DirectSalesOrder.OrderDate,
                        DirectSalesOrder.DeliveryDate,
                        DirectSalesOrder.EditedPriceStatusId,
                        DirectSalesOrder.Note,
                        DirectSalesOrder.SubTotal,
                        DirectSalesOrder.GeneralDiscountPercentage,
                        DirectSalesOrder.GeneralDiscountAmount,
                        DirectSalesOrder.TotalTaxAmount,
                        DirectSalesOrder.Total,
                        DirectSalesOrder.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("DirectSalesOrder", DirectSalesOrderHeaders, DirectSalesOrderData);
                #endregion
                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrderContent.xlsx");
        }

        [Route(DirectSalesOrderContentRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] DirectSalesOrderContent_DirectSalesOrderContentFilterDTO DirectSalesOrderContent_DirectSalesOrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrderContent
                var DirectSalesOrderContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "DirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Price",
                        "DiscountPercentage",
                        "DiscountAmount",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TaxPercentage",
                        "TaxAmount",
                        "Amount",
                    }
                };
                List<object[]> DirectSalesOrderContentData = new List<object[]>();
                excel.GenerateWorksheet("DirectSalesOrderContent", DirectSalesOrderContentHeaders, DirectSalesOrderContentData);
                #endregion
                
                #region DirectSalesOrder
                var DirectSalesOrderFilter = new DirectSalesOrderFilter();
                DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
                DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
                DirectSalesOrderFilter.OrderType = OrderType.ASC;
                DirectSalesOrderFilter.Skip = 0;
                DirectSalesOrderFilter.Take = int.MaxValue;
                List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);

                var DirectSalesOrderHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "BuyerStoreId",
                        "StorePhone",
                        "StoreAddress",
                        "StoreDeliveryAddress",
                        "TaxCode",
                        "SaleEmployeeId",
                        "OrderDate",
                        "DeliveryDate",
                        "EditedPriceStatusId",
                        "Note",
                        "SubTotal",
                        "GeneralDiscountPercentage",
                        "GeneralDiscountAmount",
                        "TotalTaxAmount",
                        "Total",
                        "RequestStateId",
                    }
                };
                List<object[]> DirectSalesOrderData = new List<object[]>();
                for (int i = 0; i < DirectSalesOrders.Count; i++)
                {
                    var DirectSalesOrder = DirectSalesOrders[i];
                    DirectSalesOrderData.Add(new Object[]
                    {
                        DirectSalesOrder.Id,
                        DirectSalesOrder.Code,
                        DirectSalesOrder.BuyerStoreId,
                        DirectSalesOrder.StorePhone,
                        DirectSalesOrder.StoreAddress,
                        DirectSalesOrder.StoreDeliveryAddress,
                        DirectSalesOrder.TaxCode,
                        DirectSalesOrder.SaleEmployeeId,
                        DirectSalesOrder.OrderDate,
                        DirectSalesOrder.DeliveryDate,
                        DirectSalesOrder.EditedPriceStatusId,
                        DirectSalesOrder.Note,
                        DirectSalesOrder.SubTotal,
                        DirectSalesOrder.GeneralDiscountPercentage,
                        DirectSalesOrder.GeneralDiscountAmount,
                        DirectSalesOrder.TotalTaxAmount,
                        DirectSalesOrder.Total,
                        DirectSalesOrder.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("DirectSalesOrder", DirectSalesOrderHeaders, DirectSalesOrderData);
                #endregion
                #region Item
                var ItemFilter = new ItemFilter();
                ItemFilter.Selects = ItemSelect.ALL;
                ItemFilter.OrderBy = ItemOrder.Id;
                ItemFilter.OrderType = OrderType.ASC;
                ItemFilter.Skip = 0;
                ItemFilter.Take = int.MaxValue;
                List<Item> Items = await ItemService.List(ItemFilter);

                var ItemHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ProductId",
                        "Code",
                        "Name",
                        "ScanCode",
                        "SalePrice",
                        "RetailPrice",
                        "StatusId",
                    }
                };
                List<object[]> ItemData = new List<object[]>();
                for (int i = 0; i < Items.Count; i++)
                {
                    var Item = Items[i];
                    ItemData.Add(new Object[]
                    {
                        Item.Id,
                        Item.ProductId,
                        Item.Code,
                        Item.Name,
                        Item.ScanCode,
                        Item.SalePrice,
                        Item.RetailPrice,
                        Item.StatusId,
                    });
                }
                excel.GenerateWorksheet("Item", ItemHeaders, ItemData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrderContent.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter();
            DirectSalesOrderContentFilter = DirectSalesOrderContentService.ToFilter(DirectSalesOrderContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                DirectSalesOrderContentFilter.Id = new IdFilter { Equal = Id };
                int count = await DirectSalesOrderContentService.Count(DirectSalesOrderContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private DirectSalesOrderContent ConvertDTOToEntity(DirectSalesOrderContent_DirectSalesOrderContentDTO DirectSalesOrderContent_DirectSalesOrderContentDTO)
        {
            DirectSalesOrderContent DirectSalesOrderContent = new DirectSalesOrderContent();
            DirectSalesOrderContent.Id = DirectSalesOrderContent_DirectSalesOrderContentDTO.Id;
            DirectSalesOrderContent.DirectSalesOrderId = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrderId;
            DirectSalesOrderContent.ItemId = DirectSalesOrderContent_DirectSalesOrderContentDTO.ItemId;
            DirectSalesOrderContent.UnitOfMeasureId = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasureId;
            DirectSalesOrderContent.Quantity = DirectSalesOrderContent_DirectSalesOrderContentDTO.Quantity;
            DirectSalesOrderContent.PrimaryUnitOfMeasureId = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasureId;
            DirectSalesOrderContent.RequestedQuantity = DirectSalesOrderContent_DirectSalesOrderContentDTO.RequestedQuantity;
            DirectSalesOrderContent.Price = DirectSalesOrderContent_DirectSalesOrderContentDTO.Price;
            DirectSalesOrderContent.DiscountPercentage = DirectSalesOrderContent_DirectSalesOrderContentDTO.DiscountPercentage;
            DirectSalesOrderContent.DiscountAmount = DirectSalesOrderContent_DirectSalesOrderContentDTO.DiscountAmount;
            DirectSalesOrderContent.GeneralDiscountPercentage = DirectSalesOrderContent_DirectSalesOrderContentDTO.GeneralDiscountPercentage;
            DirectSalesOrderContent.GeneralDiscountAmount = DirectSalesOrderContent_DirectSalesOrderContentDTO.GeneralDiscountAmount;
            DirectSalesOrderContent.TaxPercentage = DirectSalesOrderContent_DirectSalesOrderContentDTO.TaxPercentage;
            DirectSalesOrderContent.TaxAmount = DirectSalesOrderContent_DirectSalesOrderContentDTO.TaxAmount;
            DirectSalesOrderContent.Amount = DirectSalesOrderContent_DirectSalesOrderContentDTO.Amount;
            DirectSalesOrderContent.DirectSalesOrder = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder == null ? null : new DirectSalesOrder
            {
                Id = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.Id,
                Code = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.Code,
                BuyerStoreId = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.BuyerStoreId,
                StorePhone = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.StorePhone,
                StoreAddress = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.StoreAddress,
                StoreDeliveryAddress = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.StoreDeliveryAddress,
                TaxCode = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.TaxCode,
                SaleEmployeeId = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.SaleEmployeeId,
                OrderDate = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.OrderDate,
                DeliveryDate = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.DeliveryDate,
                EditedPriceStatusId = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.EditedPriceStatusId,
                Note = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.Note,
                SubTotal = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.SubTotal,
                GeneralDiscountPercentage = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.GeneralDiscountPercentage,
                GeneralDiscountAmount = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.GeneralDiscountAmount,
                TotalTaxAmount = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.TotalTaxAmount,
                Total = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.Total,
                RequestStateId = DirectSalesOrderContent_DirectSalesOrderContentDTO.DirectSalesOrder.RequestStateId,
            };
            DirectSalesOrderContent.Item = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item == null ? null : new Item
            {
                Id = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.Id,
                ProductId = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.ProductId,
                Code = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.Code,
                Name = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.Name,
                ScanCode = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.ScanCode,
                SalePrice = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.SalePrice,
                RetailPrice = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.RetailPrice,
                StatusId = DirectSalesOrderContent_DirectSalesOrderContentDTO.Item.StatusId,
            };
            DirectSalesOrderContent.PrimaryUnitOfMeasure = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Id,
                Code = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Code,
                Name = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Name,
                Description = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure.Description,
                StatusId = DirectSalesOrderContent_DirectSalesOrderContentDTO.PrimaryUnitOfMeasure.StatusId,
            };
            DirectSalesOrderContent.UnitOfMeasure = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure.Id,
                Code = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure.Code,
                Name = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure.Name,
                Description = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure.Description,
                StatusId = DirectSalesOrderContent_DirectSalesOrderContentDTO.UnitOfMeasure.StatusId,
            };
            DirectSalesOrderContent.BaseLanguage = CurrentContext.Language;
            return DirectSalesOrderContent;
        }

        private DirectSalesOrderContentFilter ConvertFilterDTOToFilterEntity(DirectSalesOrderContent_DirectSalesOrderContentFilterDTO DirectSalesOrderContent_DirectSalesOrderContentFilterDTO)
        {
            DirectSalesOrderContentFilter DirectSalesOrderContentFilter = new DirectSalesOrderContentFilter();
            DirectSalesOrderContentFilter.Selects = DirectSalesOrderContentSelect.ALL;
            DirectSalesOrderContentFilter.Skip = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Skip;
            DirectSalesOrderContentFilter.Take = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Take;
            DirectSalesOrderContentFilter.OrderBy = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.OrderBy;
            DirectSalesOrderContentFilter.OrderType = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.OrderType;

            DirectSalesOrderContentFilter.Id = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Id;
            DirectSalesOrderContentFilter.DirectSalesOrderId = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.DirectSalesOrderId;
            DirectSalesOrderContentFilter.ItemId = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.ItemId;
            DirectSalesOrderContentFilter.UnitOfMeasureId = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.UnitOfMeasureId;
            DirectSalesOrderContentFilter.Quantity = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Quantity;
            DirectSalesOrderContentFilter.PrimaryUnitOfMeasureId = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.PrimaryUnitOfMeasureId;
            DirectSalesOrderContentFilter.RequestedQuantity = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.RequestedQuantity;
            DirectSalesOrderContentFilter.Price = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Price;
            DirectSalesOrderContentFilter.DiscountPercentage = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.DiscountPercentage;
            DirectSalesOrderContentFilter.DiscountAmount = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.DiscountAmount;
            DirectSalesOrderContentFilter.GeneralDiscountPercentage = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderContentFilter.GeneralDiscountAmount = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderContentFilter.TaxPercentage = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.TaxPercentage;
            DirectSalesOrderContentFilter.TaxAmount = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.TaxAmount;
            DirectSalesOrderContentFilter.Amount = DirectSalesOrderContent_DirectSalesOrderContentFilterDTO.Amount;
            return DirectSalesOrderContentFilter;
        }

        [Route(DirectSalesOrderContentRoute.FilterListDirectSalesOrder), HttpPost]
        public async Task<List<DirectSalesOrderContent_DirectSalesOrderDTO>> FilterListDirectSalesOrder([FromBody] DirectSalesOrderContent_DirectSalesOrderFilterDTO DirectSalesOrderContent_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Skip = 0;
            DirectSalesOrderFilter.Take = 20;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
            DirectSalesOrderFilter.OrderType = OrderType.ASC;
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Id = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.StorePhone = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StorePhone;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.StoreDeliveryAddress = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StoreDeliveryAddress;
            DirectSalesOrderFilter.TaxCode = DirectSalesOrderContent_DirectSalesOrderFilterDTO.TaxCode;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrderContent_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrderContent_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrderContent_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrderContent_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrderContent_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrderContent_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.RequestStateId;

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrderContent_DirectSalesOrderDTO> DirectSalesOrderContent_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(x => new DirectSalesOrderContent_DirectSalesOrderDTO(x)).ToList();
            return DirectSalesOrderContent_DirectSalesOrderDTOs;
        }
        [Route(DirectSalesOrderContentRoute.FilterListItem), HttpPost]
        public async Task<List<DirectSalesOrderContent_ItemDTO>> FilterListItem([FromBody] DirectSalesOrderContent_ItemFilterDTO DirectSalesOrderContent_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrderContent_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrderContent_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrderContent_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrderContent_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrderContent_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrderContent_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrderContent_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectSalesOrderContent_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrderContent_ItemDTO> DirectSalesOrderContent_ItemDTOs = Items
                .Select(x => new DirectSalesOrderContent_ItemDTO(x)).ToList();
            return DirectSalesOrderContent_ItemDTOs;
        }
        [Route(DirectSalesOrderContentRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrderContent_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] DirectSalesOrderContent_UnitOfMeasureFilterDTO DirectSalesOrderContent_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = DirectSalesOrderContent_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<DirectSalesOrderContent_UnitOfMeasureDTO> DirectSalesOrderContent_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new DirectSalesOrderContent_UnitOfMeasureDTO(x)).ToList();
            return DirectSalesOrderContent_UnitOfMeasureDTOs;
        }

        [Route(DirectSalesOrderContentRoute.SingleListDirectSalesOrder), HttpPost]
        public async Task<List<DirectSalesOrderContent_DirectSalesOrderDTO>> SingleListDirectSalesOrder([FromBody] DirectSalesOrderContent_DirectSalesOrderFilterDTO DirectSalesOrderContent_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Skip = 0;
            DirectSalesOrderFilter.Take = 20;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
            DirectSalesOrderFilter.OrderType = OrderType.ASC;
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Id = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.StorePhone = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StorePhone;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.StoreDeliveryAddress = DirectSalesOrderContent_DirectSalesOrderFilterDTO.StoreDeliveryAddress;
            DirectSalesOrderFilter.TaxCode = DirectSalesOrderContent_DirectSalesOrderFilterDTO.TaxCode;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrderContent_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrderContent_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrderContent_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrderContent_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrderContent_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrderContent_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrderContent_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrderContent_DirectSalesOrderFilterDTO.RequestStateId;

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrderContent_DirectSalesOrderDTO> DirectSalesOrderContent_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(x => new DirectSalesOrderContent_DirectSalesOrderDTO(x)).ToList();
            return DirectSalesOrderContent_DirectSalesOrderDTOs;
        }
        [Route(DirectSalesOrderContentRoute.SingleListItem), HttpPost]
        public async Task<List<DirectSalesOrderContent_ItemDTO>> SingleListItem([FromBody] DirectSalesOrderContent_ItemFilterDTO DirectSalesOrderContent_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrderContent_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrderContent_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrderContent_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrderContent_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrderContent_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrderContent_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrderContent_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectSalesOrderContent_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrderContent_ItemDTO> DirectSalesOrderContent_ItemDTOs = Items
                .Select(x => new DirectSalesOrderContent_ItemDTO(x)).ToList();
            return DirectSalesOrderContent_ItemDTOs;
        }
        [Route(DirectSalesOrderContentRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrderContent_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] DirectSalesOrderContent_UnitOfMeasureFilterDTO DirectSalesOrderContent_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = DirectSalesOrderContent_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = DirectSalesOrderContent_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<DirectSalesOrderContent_UnitOfMeasureDTO> DirectSalesOrderContent_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new DirectSalesOrderContent_UnitOfMeasureDTO(x)).ToList();
            return DirectSalesOrderContent_UnitOfMeasureDTOs;
        }

    }
}

