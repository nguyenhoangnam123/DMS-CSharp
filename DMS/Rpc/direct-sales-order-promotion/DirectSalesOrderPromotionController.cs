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
using DMS.Services.MDirectSalesOrderPromotion;
using DMS.Services.MDirectSalesOrder;
using DMS.Services.MItem;
using DMS.Services.MUnitOfMeasure;

namespace DMS.Rpc.direct_sales_order_promotion
{
    public class DirectSalesOrderPromotionRoute : Root
    {
        public const string Master = Module + "/direct-sales-order-promotion/direct-sales-order-promotion-master";
        public const string Detail = Module + "/direct-sales-order-promotion/direct-sales-order-promotion-detail";
        private const string Default = Rpc + Module + "/direct-sales-order-promotion";
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
            { nameof(DirectSalesOrderPromotionFilter.Id), FieldType.ID },
            { nameof(DirectSalesOrderPromotionFilter.DirectSalesOrderId), FieldType.ID },
            { nameof(DirectSalesOrderPromotionFilter.ItemId), FieldType.ID },
            { nameof(DirectSalesOrderPromotionFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(DirectSalesOrderPromotionFilter.Quantity), FieldType.LONG },
            { nameof(DirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId), FieldType.ID },
            { nameof(DirectSalesOrderPromotionFilter.RequestedQuantity), FieldType.LONG },
            { nameof(DirectSalesOrderPromotionFilter.Note), FieldType.STRING },
        };
    }

    public class DirectSalesOrderPromotionController : RpcController
    {
        private IDirectSalesOrderService DirectSalesOrderService;
        private IItemService ItemService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IDirectSalesOrderPromotionService DirectSalesOrderPromotionService;
        private ICurrentContext CurrentContext;
        public DirectSalesOrderPromotionController(
            IDirectSalesOrderService DirectSalesOrderService,
            IItemService ItemService,
            IUnitOfMeasureService UnitOfMeasureService,
            IDirectSalesOrderPromotionService DirectSalesOrderPromotionService,
            ICurrentContext CurrentContext
        )
        {
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.ItemService = ItemService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.DirectSalesOrderPromotionService = DirectSalesOrderPromotionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectSalesOrderPromotionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO);
            DirectSalesOrderPromotionFilter = DirectSalesOrderPromotionService.ToFilter(DirectSalesOrderPromotionFilter);
            int count = await DirectSalesOrderPromotionService.Count(DirectSalesOrderPromotionFilter);
            return count;
        }

        [Route(DirectSalesOrderPromotionRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO>>> List([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO);
            DirectSalesOrderPromotionFilter = DirectSalesOrderPromotionService.ToFilter(DirectSalesOrderPromotionFilter);
            List<DirectSalesOrderPromotion> DirectSalesOrderPromotions = await DirectSalesOrderPromotionService.List(DirectSalesOrderPromotionFilter);
            List<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO> DirectSalesOrderPromotion_DirectSalesOrderPromotionDTOs = DirectSalesOrderPromotions
                .Select(c => new DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(c)).ToList();
            return DirectSalesOrderPromotion_DirectSalesOrderPromotionDTOs;
        }

        [Route(DirectSalesOrderPromotionRoute.Get), HttpPost]
        public async Task<ActionResult<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO>> Get([FromBody]DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id))
                return Forbid();

            DirectSalesOrderPromotion DirectSalesOrderPromotion = await DirectSalesOrderPromotionService.Get(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id);
            return new DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(DirectSalesOrderPromotion);
        }

        [Route(DirectSalesOrderPromotionRoute.Create), HttpPost]
        public async Task<ActionResult<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO>> Create([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id))
                return Forbid();

            DirectSalesOrderPromotion DirectSalesOrderPromotion = ConvertDTOToEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
            DirectSalesOrderPromotion = await DirectSalesOrderPromotionService.Create(DirectSalesOrderPromotion);
            DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO = new DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(DirectSalesOrderPromotion);
            if (DirectSalesOrderPromotion.IsValidated)
                return DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO;
            else
                return BadRequest(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
        }

        [Route(DirectSalesOrderPromotionRoute.Update), HttpPost]
        public async Task<ActionResult<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO>> Update([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id))
                return Forbid();

            DirectSalesOrderPromotion DirectSalesOrderPromotion = ConvertDTOToEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
            DirectSalesOrderPromotion = await DirectSalesOrderPromotionService.Update(DirectSalesOrderPromotion);
            DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO = new DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(DirectSalesOrderPromotion);
            if (DirectSalesOrderPromotion.IsValidated)
                return DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO;
            else
                return BadRequest(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
        }

        [Route(DirectSalesOrderPromotionRoute.Delete), HttpPost]
        public async Task<ActionResult<DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO>> Delete([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id))
                return Forbid();

            DirectSalesOrderPromotion DirectSalesOrderPromotion = ConvertDTOToEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
            DirectSalesOrderPromotion = await DirectSalesOrderPromotionService.Delete(DirectSalesOrderPromotion);
            DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO = new DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(DirectSalesOrderPromotion);
            if (DirectSalesOrderPromotion.IsValidated)
                return DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO;
            else
                return BadRequest(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO);
        }
        
        [Route(DirectSalesOrderPromotionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter();
            DirectSalesOrderPromotionFilter = DirectSalesOrderPromotionService.ToFilter(DirectSalesOrderPromotionFilter);
            DirectSalesOrderPromotionFilter.Id = new IdFilter { In = Ids };
            DirectSalesOrderPromotionFilter.Selects = DirectSalesOrderPromotionSelect.Id;
            DirectSalesOrderPromotionFilter.Skip = 0;
            DirectSalesOrderPromotionFilter.Take = int.MaxValue;

            List<DirectSalesOrderPromotion> DirectSalesOrderPromotions = await DirectSalesOrderPromotionService.List(DirectSalesOrderPromotionFilter);
            DirectSalesOrderPromotions = await DirectSalesOrderPromotionService.BulkDelete(DirectSalesOrderPromotions);
            return true;
        }
        
        [Route(DirectSalesOrderPromotionRoute.Import), HttpPost]
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
            List<DirectSalesOrderPromotion> DirectSalesOrderPromotions = new List<DirectSalesOrderPromotion>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(DirectSalesOrderPromotions);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int DirectSalesOrderIdColumn = 1 + StartColumn;
                int ItemIdColumn = 2 + StartColumn;
                int UnitOfMeasureIdColumn = 3 + StartColumn;
                int QuantityColumn = 4 + StartColumn;
                int PrimaryUnitOfMeasureIdColumn = 5 + StartColumn;
                int RequestedQuantityColumn = 6 + StartColumn;
                int NoteColumn = 7 + StartColumn;

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
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    
                    DirectSalesOrderPromotion DirectSalesOrderPromotion = new DirectSalesOrderPromotion();
                    DirectSalesOrderPromotion.Quantity = long.TryParse(QuantityValue, out long Quantity) ? Quantity : 0;
                    DirectSalesOrderPromotion.RequestedQuantity = long.TryParse(RequestedQuantityValue, out long RequestedQuantity) ? RequestedQuantity : 0;
                    DirectSalesOrderPromotion.Note = NoteValue;
                    DirectSalesOrder DirectSalesOrder = DirectSalesOrders.Where(x => x.Id.ToString() == DirectSalesOrderIdValue).FirstOrDefault();
                    DirectSalesOrderPromotion.DirectSalesOrderId = DirectSalesOrder == null ? 0 : DirectSalesOrder.Id;
                    DirectSalesOrderPromotion.DirectSalesOrder = DirectSalesOrder;
                    Item Item = Items.Where(x => x.Id.ToString() == ItemIdValue).FirstOrDefault();
                    DirectSalesOrderPromotion.ItemId = Item == null ? 0 : Item.Id;
                    DirectSalesOrderPromotion.Item = Item;
                    UnitOfMeasure PrimaryUnitOfMeasure = PrimaryUnitOfMeasures.Where(x => x.Id.ToString() == PrimaryUnitOfMeasureIdValue).FirstOrDefault();
                    DirectSalesOrderPromotion.PrimaryUnitOfMeasureId = PrimaryUnitOfMeasure == null ? 0 : PrimaryUnitOfMeasure.Id;
                    DirectSalesOrderPromotion.PrimaryUnitOfMeasure = PrimaryUnitOfMeasure;
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures.Where(x => x.Id.ToString() == UnitOfMeasureIdValue).FirstOrDefault();
                    DirectSalesOrderPromotion.UnitOfMeasureId = UnitOfMeasure == null ? 0 : UnitOfMeasure.Id;
                    DirectSalesOrderPromotion.UnitOfMeasure = UnitOfMeasure;
                    
                    DirectSalesOrderPromotions.Add(DirectSalesOrderPromotion);
                }
            }
            DirectSalesOrderPromotions = await DirectSalesOrderPromotionService.Import(DirectSalesOrderPromotions);
            if (DirectSalesOrderPromotions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < DirectSalesOrderPromotions.Count; i++)
                {
                    DirectSalesOrderPromotion DirectSalesOrderPromotion = DirectSalesOrderPromotions[i];
                    if (!DirectSalesOrderPromotion.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.Id)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.Id)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.DirectSalesOrderId)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.DirectSalesOrderId)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.ItemId)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.ItemId)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.UnitOfMeasureId)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.UnitOfMeasureId)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.Quantity)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.Quantity)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.PrimaryUnitOfMeasureId)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.PrimaryUnitOfMeasureId)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.RequestedQuantity)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.RequestedQuantity)];
                        if (DirectSalesOrderPromotion.Errors.ContainsKey(nameof(DirectSalesOrderPromotion.Note)))
                            Error += DirectSalesOrderPromotion.Errors[nameof(DirectSalesOrderPromotion.Note)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(DirectSalesOrderPromotionRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrderPromotion
                var DirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO);
                DirectSalesOrderPromotionFilter.Skip = 0;
                DirectSalesOrderPromotionFilter.Take = int.MaxValue;
                DirectSalesOrderPromotionFilter = DirectSalesOrderPromotionService.ToFilter(DirectSalesOrderPromotionFilter);
                List<DirectSalesOrderPromotion> DirectSalesOrderPromotions = await DirectSalesOrderPromotionService.List(DirectSalesOrderPromotionFilter);

                var DirectSalesOrderPromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "DirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Note",
                    }
                };
                List<object[]> DirectSalesOrderPromotionData = new List<object[]>();
                for (int i = 0; i < DirectSalesOrderPromotions.Count; i++)
                {
                    var DirectSalesOrderPromotion = DirectSalesOrderPromotions[i];
                    DirectSalesOrderPromotionData.Add(new Object[]
                    {
                        DirectSalesOrderPromotion.Id,
                        DirectSalesOrderPromotion.DirectSalesOrderId,
                        DirectSalesOrderPromotion.ItemId,
                        DirectSalesOrderPromotion.UnitOfMeasureId,
                        DirectSalesOrderPromotion.Quantity,
                        DirectSalesOrderPromotion.PrimaryUnitOfMeasureId,
                        DirectSalesOrderPromotion.RequestedQuantity,
                        DirectSalesOrderPromotion.Note,
                    });
                }
                excel.GenerateWorksheet("DirectSalesOrderPromotion", DirectSalesOrderPromotionHeaders, DirectSalesOrderPromotionData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrderPromotion.xlsx");
        }

        [Route(DirectSalesOrderPromotionRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrderPromotion
                var DirectSalesOrderPromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "DirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Note",
                    }
                };
                List<object[]> DirectSalesOrderPromotionData = new List<object[]>();
                excel.GenerateWorksheet("DirectSalesOrderPromotion", DirectSalesOrderPromotionHeaders, DirectSalesOrderPromotionData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrderPromotion.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter();
            DirectSalesOrderPromotionFilter = DirectSalesOrderPromotionService.ToFilter(DirectSalesOrderPromotionFilter);
            if (Id == 0)
            {

            }
            else
            {
                DirectSalesOrderPromotionFilter.Id = new IdFilter { Equal = Id };
                int count = await DirectSalesOrderPromotionService.Count(DirectSalesOrderPromotionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private DirectSalesOrderPromotion ConvertDTOToEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO)
        {
            DirectSalesOrderPromotion DirectSalesOrderPromotion = new DirectSalesOrderPromotion();
            DirectSalesOrderPromotion.Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Id;
            DirectSalesOrderPromotion.DirectSalesOrderId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrderId;
            DirectSalesOrderPromotion.ItemId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.ItemId;
            DirectSalesOrderPromotion.UnitOfMeasureId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasureId;
            DirectSalesOrderPromotion.Quantity = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Quantity;
            DirectSalesOrderPromotion.PrimaryUnitOfMeasureId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasureId;
            DirectSalesOrderPromotion.RequestedQuantity = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.RequestedQuantity;
            DirectSalesOrderPromotion.Note = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Note;
            DirectSalesOrderPromotion.DirectSalesOrder = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder == null ? null : new DirectSalesOrder
            {
                Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.Id,
                Code = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.Code,
                BuyerStoreId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.BuyerStoreId,
                StorePhone = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.StorePhone,
                StoreAddress = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.StoreAddress,
                StoreDeliveryAddress = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.StoreDeliveryAddress,
                TaxCode = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.TaxCode,
                SaleEmployeeId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.SaleEmployeeId,
                OrderDate = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.OrderDate,
                DeliveryDate = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.DeliveryDate,
                EditedPriceStatusId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.EditedPriceStatusId,
                Note = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.Note,
                SubTotal = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.SubTotal,
                GeneralDiscountPercentage = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.GeneralDiscountPercentage,
                GeneralDiscountAmount = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.GeneralDiscountAmount,
                TotalTaxAmount = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.TotalTaxAmount,
                Total = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.Total,
                RequestStateId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.DirectSalesOrder.RequestStateId,
            };
            DirectSalesOrderPromotion.Item = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item == null ? null : new Item
            {
                Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.Id,
                ProductId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.ProductId,
                Code = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.Code,
                Name = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.Name,
                ScanCode = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.ScanCode,
                SalePrice = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.SalePrice,
                RetailPrice = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.RetailPrice,
                StatusId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.Item.StatusId,
            };
            DirectSalesOrderPromotion.PrimaryUnitOfMeasure = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Id,
                Code = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Code,
                Name = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Name,
                Description = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Description,
                StatusId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.StatusId,
            };
            DirectSalesOrderPromotion.UnitOfMeasure = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure.Id,
                Code = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure.Code,
                Name = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure.Name,
                Description = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure.Description,
                StatusId = DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO.UnitOfMeasure.StatusId,
            };
            DirectSalesOrderPromotion.BaseLanguage = CurrentContext.Language;
            return DirectSalesOrderPromotion;
        }

        private DirectSalesOrderPromotionFilter ConvertFilterDTOToFilterEntity(DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO)
        {
            DirectSalesOrderPromotionFilter DirectSalesOrderPromotionFilter = new DirectSalesOrderPromotionFilter();
            DirectSalesOrderPromotionFilter.Selects = DirectSalesOrderPromotionSelect.ALL;
            DirectSalesOrderPromotionFilter.Skip = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.Skip;
            DirectSalesOrderPromotionFilter.Take = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.Take;
            DirectSalesOrderPromotionFilter.OrderBy = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.OrderBy;
            DirectSalesOrderPromotionFilter.OrderType = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.OrderType;

            DirectSalesOrderPromotionFilter.Id = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.Id;
            DirectSalesOrderPromotionFilter.DirectSalesOrderId = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.DirectSalesOrderId;
            DirectSalesOrderPromotionFilter.ItemId = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.ItemId;
            DirectSalesOrderPromotionFilter.UnitOfMeasureId = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.UnitOfMeasureId;
            DirectSalesOrderPromotionFilter.Quantity = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.Quantity;
            DirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.PrimaryUnitOfMeasureId;
            DirectSalesOrderPromotionFilter.RequestedQuantity = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.RequestedQuantity;
            DirectSalesOrderPromotionFilter.Note = DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO.Note;
            return DirectSalesOrderPromotionFilter;
        }

        [Route(DirectSalesOrderPromotionRoute.FilterListDirectSalesOrder), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_DirectSalesOrderDTO>> FilterListDirectSalesOrder([FromBody] DirectSalesOrderPromotion_DirectSalesOrderFilterDTO DirectSalesOrderPromotion_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Skip = 0;
            DirectSalesOrderFilter.Take = 20;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
            DirectSalesOrderFilter.OrderType = OrderType.ASC;
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Id = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.StorePhone = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StorePhone;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.StoreDeliveryAddress = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StoreDeliveryAddress;
            DirectSalesOrderFilter.TaxCode = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.TaxCode;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.RequestStateId;

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrderPromotion_DirectSalesOrderDTO> DirectSalesOrderPromotion_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(x => new DirectSalesOrderPromotion_DirectSalesOrderDTO(x)).ToList();
            return DirectSalesOrderPromotion_DirectSalesOrderDTOs;
        }
        [Route(DirectSalesOrderPromotionRoute.FilterListItem), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_ItemDTO>> FilterListItem([FromBody] DirectSalesOrderPromotion_ItemFilterDTO DirectSalesOrderPromotion_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrderPromotion_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrderPromotion_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrderPromotion_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrderPromotion_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrderPromotion_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrderPromotion_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrderPromotion_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectSalesOrderPromotion_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrderPromotion_ItemDTO> DirectSalesOrderPromotion_ItemDTOs = Items
                .Select(x => new DirectSalesOrderPromotion_ItemDTO(x)).ToList();
            return DirectSalesOrderPromotion_ItemDTOs;
        }
        [Route(DirectSalesOrderPromotionRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] DirectSalesOrderPromotion_UnitOfMeasureFilterDTO DirectSalesOrderPromotion_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<DirectSalesOrderPromotion_UnitOfMeasureDTO> DirectSalesOrderPromotion_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new DirectSalesOrderPromotion_UnitOfMeasureDTO(x)).ToList();
            return DirectSalesOrderPromotion_UnitOfMeasureDTOs;
        }

        [Route(DirectSalesOrderPromotionRoute.SingleListDirectSalesOrder), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_DirectSalesOrderDTO>> SingleListDirectSalesOrder([FromBody] DirectSalesOrderPromotion_DirectSalesOrderFilterDTO DirectSalesOrderPromotion_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Skip = 0;
            DirectSalesOrderFilter.Take = 20;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrderOrder.Id;
            DirectSalesOrderFilter.OrderType = OrderType.ASC;
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Id = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.StorePhone = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StorePhone;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.StoreDeliveryAddress = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.StoreDeliveryAddress;
            DirectSalesOrderFilter.TaxCode = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.TaxCode;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrderPromotion_DirectSalesOrderFilterDTO.RequestStateId;

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrderPromotion_DirectSalesOrderDTO> DirectSalesOrderPromotion_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(x => new DirectSalesOrderPromotion_DirectSalesOrderDTO(x)).ToList();
            return DirectSalesOrderPromotion_DirectSalesOrderDTOs;
        }
        [Route(DirectSalesOrderPromotionRoute.SingleListItem), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_ItemDTO>> SingleListItem([FromBody] DirectSalesOrderPromotion_ItemFilterDTO DirectSalesOrderPromotion_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = DirectSalesOrderPromotion_ItemFilterDTO.Id;
            ItemFilter.ProductId = DirectSalesOrderPromotion_ItemFilterDTO.ProductId;
            ItemFilter.Code = DirectSalesOrderPromotion_ItemFilterDTO.Code;
            ItemFilter.Name = DirectSalesOrderPromotion_ItemFilterDTO.Name;
            ItemFilter.ScanCode = DirectSalesOrderPromotion_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = DirectSalesOrderPromotion_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = DirectSalesOrderPromotion_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = DirectSalesOrderPromotion_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<DirectSalesOrderPromotion_ItemDTO> DirectSalesOrderPromotion_ItemDTOs = Items
                .Select(x => new DirectSalesOrderPromotion_ItemDTO(x)).ToList();
            return DirectSalesOrderPromotion_ItemDTOs;
        }
        [Route(DirectSalesOrderPromotionRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<DirectSalesOrderPromotion_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] DirectSalesOrderPromotion_UnitOfMeasureFilterDTO DirectSalesOrderPromotion_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = DirectSalesOrderPromotion_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<DirectSalesOrderPromotion_UnitOfMeasureDTO> DirectSalesOrderPromotion_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new DirectSalesOrderPromotion_UnitOfMeasureDTO(x)).ToList();
            return DirectSalesOrderPromotion_UnitOfMeasureDTOs;
        }

    }
}

