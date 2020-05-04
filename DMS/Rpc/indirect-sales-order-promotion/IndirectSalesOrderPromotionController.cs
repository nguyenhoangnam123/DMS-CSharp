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
using DMS.Services.MIndirectSalesOrderPromotion;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MItem;
using DMS.Services.MUnitOfMeasure;
using DMS.Enums;

namespace DMS.Rpc.indirect_sales_order_promotion
{
    public class IndirectSalesOrderPromotionRoute : Root
    {
        public const string Master = Module + "/indirect-sales-order-promotion/indirect-sales-order-promotion-master";
        public const string Detail = Module + "/indirect-sales-order-promotion/indirect-sales-order-promotion-detail";
        private const string Default = Rpc + Module + "/indirect-sales-order-promotion";
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
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";
        
        public const string SingleListIndirectSalesOrder = Default + "/single-list-indirect-sales-order";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectSalesOrderPromotionFilter.Id), FieldType.ID },
            { nameof(IndirectSalesOrderPromotionFilter.IndirectSalesOrderId), FieldType.ID },
            { nameof(IndirectSalesOrderPromotionFilter.ItemId), FieldType.ID },
            { nameof(IndirectSalesOrderPromotionFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(IndirectSalesOrderPromotionFilter.Quantity), FieldType.LONG },
            { nameof(IndirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId), FieldType.ID },
            { nameof(IndirectSalesOrderPromotionFilter.RequestedQuantity), FieldType.LONG },
            { nameof(IndirectSalesOrderPromotionFilter.Note), FieldType.STRING },
        };
    }

    public class IndirectSalesOrderPromotionController : RpcController
    {
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private IItemService ItemService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IIndirectSalesOrderPromotionService IndirectSalesOrderPromotionService;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderPromotionController(
            IIndirectSalesOrderService IndirectSalesOrderService,
            IItemService ItemService,
            IUnitOfMeasureService UnitOfMeasureService,
            IIndirectSalesOrderPromotionService IndirectSalesOrderPromotionService,
            ICurrentContext CurrentContext
        )
        {
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.ItemService = ItemService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.IndirectSalesOrderPromotionService = IndirectSalesOrderPromotionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderPromotionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO);
            IndirectSalesOrderPromotionFilter = IndirectSalesOrderPromotionService.ToFilter(IndirectSalesOrderPromotionFilter);
            int count = await IndirectSalesOrderPromotionService.Count(IndirectSalesOrderPromotionFilter);
            return count;
        }

        [Route(IndirectSalesOrderPromotionRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO>>> List([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO);
            IndirectSalesOrderPromotionFilter = IndirectSalesOrderPromotionService.ToFilter(IndirectSalesOrderPromotionFilter);
            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.List(IndirectSalesOrderPromotionFilter);
            List<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO> IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTOs = IndirectSalesOrderPromotions
                .Select(c => new IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO(c)).ToList();
            return IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTOs;
        }

        [Route(IndirectSalesOrderPromotionRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO>> Get([FromBody]IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id))
                return Forbid();

            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = await IndirectSalesOrderPromotionService.Get(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id);
            return new IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotion);
        }

        [Route(IndirectSalesOrderPromotionRoute.Create), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO>> Create([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id))
                return Forbid();

            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = ConvertDTOToEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
            IndirectSalesOrderPromotion = await IndirectSalesOrderPromotionService.Create(IndirectSalesOrderPromotion);
            IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO = new IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotion);
            if (IndirectSalesOrderPromotion.IsValidated)
                return IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO;
            else
                return BadRequest(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
        }

        [Route(IndirectSalesOrderPromotionRoute.Update), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO>> Update([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id))
                return Forbid();

            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = ConvertDTOToEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
            IndirectSalesOrderPromotion = await IndirectSalesOrderPromotionService.Update(IndirectSalesOrderPromotion);
            IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO = new IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotion);
            if (IndirectSalesOrderPromotion.IsValidated)
                return IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO;
            else
                return BadRequest(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
        }

        [Route(IndirectSalesOrderPromotionRoute.Delete), HttpPost]
        public async Task<ActionResult<IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO>> Delete([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id))
                return Forbid();

            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = ConvertDTOToEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
            IndirectSalesOrderPromotion = await IndirectSalesOrderPromotionService.Delete(IndirectSalesOrderPromotion);
            IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO = new IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotion);
            if (IndirectSalesOrderPromotion.IsValidated)
                return IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO;
            else
                return BadRequest(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO);
        }
        
        [Route(IndirectSalesOrderPromotionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
            IndirectSalesOrderPromotionFilter = IndirectSalesOrderPromotionService.ToFilter(IndirectSalesOrderPromotionFilter);
            IndirectSalesOrderPromotionFilter.Id = new IdFilter { In = Ids };
            IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.Id;
            IndirectSalesOrderPromotionFilter.Skip = 0;
            IndirectSalesOrderPromotionFilter.Take = int.MaxValue;

            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.List(IndirectSalesOrderPromotionFilter);
            IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.BulkDelete(IndirectSalesOrderPromotions);
            return true;
        }
        
        [Route(IndirectSalesOrderPromotionRoute.Import), HttpPost]
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
            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = new List<IndirectSalesOrderPromotion>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(IndirectSalesOrderPromotions);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int IndirectSalesOrderIdColumn = 1 + StartColumn;
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
                    string IndirectSalesOrderIdValue = worksheet.Cells[i + StartRow, IndirectSalesOrderIdColumn].Value?.ToString();
                    string ItemIdValue = worksheet.Cells[i + StartRow, ItemIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string QuantityValue = worksheet.Cells[i + StartRow, QuantityColumn].Value?.ToString();
                    string PrimaryUnitOfMeasureIdValue = worksheet.Cells[i + StartRow, PrimaryUnitOfMeasureIdColumn].Value?.ToString();
                    string RequestedQuantityValue = worksheet.Cells[i + StartRow, RequestedQuantityColumn].Value?.ToString();
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    
                    IndirectSalesOrderPromotion IndirectSalesOrderPromotion = new IndirectSalesOrderPromotion();
                    IndirectSalesOrderPromotion.Quantity = long.TryParse(QuantityValue, out long Quantity) ? Quantity : 0;
                    IndirectSalesOrderPromotion.RequestedQuantity = long.TryParse(RequestedQuantityValue, out long RequestedQuantity) ? RequestedQuantity : 0;
                    IndirectSalesOrderPromotion.Note = NoteValue;
                    IndirectSalesOrder IndirectSalesOrder = IndirectSalesOrders.Where(x => x.Id.ToString() == IndirectSalesOrderIdValue).FirstOrDefault();
                    IndirectSalesOrderPromotion.IndirectSalesOrderId = IndirectSalesOrder == null ? 0 : IndirectSalesOrder.Id;
                    IndirectSalesOrderPromotion.IndirectSalesOrder = IndirectSalesOrder;
                    Item Item = Items.Where(x => x.Id.ToString() == ItemIdValue).FirstOrDefault();
                    IndirectSalesOrderPromotion.ItemId = Item == null ? 0 : Item.Id;
                    IndirectSalesOrderPromotion.Item = Item;
                    UnitOfMeasure PrimaryUnitOfMeasure = PrimaryUnitOfMeasures.Where(x => x.Id.ToString() == PrimaryUnitOfMeasureIdValue).FirstOrDefault();
                    IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId = PrimaryUnitOfMeasure == null ? 0 : PrimaryUnitOfMeasure.Id;
                    IndirectSalesOrderPromotion.PrimaryUnitOfMeasure = PrimaryUnitOfMeasure;
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures.Where(x => x.Id.ToString() == UnitOfMeasureIdValue).FirstOrDefault();
                    IndirectSalesOrderPromotion.UnitOfMeasureId = UnitOfMeasure == null ? 0 : UnitOfMeasure.Id;
                    IndirectSalesOrderPromotion.UnitOfMeasure = UnitOfMeasure;
                    
                    IndirectSalesOrderPromotions.Add(IndirectSalesOrderPromotion);
                }
            }
            IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.Import(IndirectSalesOrderPromotions);
            if (IndirectSalesOrderPromotions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < IndirectSalesOrderPromotions.Count; i++)
                {
                    IndirectSalesOrderPromotion IndirectSalesOrderPromotion = IndirectSalesOrderPromotions[i];
                    if (!IndirectSalesOrderPromotion.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.Id)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.Id)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.IndirectSalesOrderId)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.IndirectSalesOrderId)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.ItemId)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.ItemId)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.UnitOfMeasureId)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.UnitOfMeasureId)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.Quantity)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.Quantity)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.RequestedQuantity)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.RequestedQuantity)];
                        if (IndirectSalesOrderPromotion.Errors.ContainsKey(nameof(IndirectSalesOrderPromotion.Note)))
                            Error += IndirectSalesOrderPromotion.Errors[nameof(IndirectSalesOrderPromotion.Note)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(IndirectSalesOrderPromotionRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrderPromotion
                var IndirectSalesOrderPromotionFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO);
                IndirectSalesOrderPromotionFilter.Skip = 0;
                IndirectSalesOrderPromotionFilter.Take = int.MaxValue;
                IndirectSalesOrderPromotionFilter = IndirectSalesOrderPromotionService.ToFilter(IndirectSalesOrderPromotionFilter);
                List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.List(IndirectSalesOrderPromotionFilter);

                var IndirectSalesOrderPromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "IndirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Note",
                    }
                };
                List<object[]> IndirectSalesOrderPromotionData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrderPromotions.Count; i++)
                {
                    var IndirectSalesOrderPromotion = IndirectSalesOrderPromotions[i];
                    IndirectSalesOrderPromotionData.Add(new Object[]
                    {
                        IndirectSalesOrderPromotion.Id,
                        IndirectSalesOrderPromotion.IndirectSalesOrderId,
                        IndirectSalesOrderPromotion.ItemId,
                        IndirectSalesOrderPromotion.UnitOfMeasureId,
                        IndirectSalesOrderPromotion.Quantity,
                        IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId,
                        IndirectSalesOrderPromotion.RequestedQuantity,
                        IndirectSalesOrderPromotion.Note,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrderPromotion", IndirectSalesOrderPromotionHeaders, IndirectSalesOrderPromotionData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrderPromotion.xlsx");
        }

        [Route(IndirectSalesOrderPromotionRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrderPromotion
                var IndirectSalesOrderPromotionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "IndirectSalesOrderId",
                        "ItemId",
                        "UnitOfMeasureId",
                        "Quantity",
                        "PrimaryUnitOfMeasureId",
                        "RequestedQuantity",
                        "Note",
                    }
                };
                List<object[]> IndirectSalesOrderPromotionData = new List<object[]>();
                excel.GenerateWorksheet("IndirectSalesOrderPromotion", IndirectSalesOrderPromotionHeaders, IndirectSalesOrderPromotionData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrderPromotion.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
            IndirectSalesOrderPromotionFilter = IndirectSalesOrderPromotionService.ToFilter(IndirectSalesOrderPromotionFilter);
            if (Id == 0)
            {

            }
            else
            {
                IndirectSalesOrderPromotionFilter.Id = new IdFilter { Equal = Id };
                int count = await IndirectSalesOrderPromotionService.Count(IndirectSalesOrderPromotionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private IndirectSalesOrderPromotion ConvertDTOToEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO)
        {
            IndirectSalesOrderPromotion IndirectSalesOrderPromotion = new IndirectSalesOrderPromotion();
            IndirectSalesOrderPromotion.Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Id;
            IndirectSalesOrderPromotion.IndirectSalesOrderId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrderId;
            IndirectSalesOrderPromotion.ItemId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.ItemId;
            IndirectSalesOrderPromotion.UnitOfMeasureId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasureId;
            IndirectSalesOrderPromotion.Quantity = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Quantity;
            IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderPromotion.RequestedQuantity = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.RequestedQuantity;
            IndirectSalesOrderPromotion.Note = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Note;
            IndirectSalesOrderPromotion.IndirectSalesOrder = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder == null ? null : new IndirectSalesOrder
            {
                Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.Id,
                Code = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.Code,
                BuyerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.BuyerStoreId,
                PhoneNumber = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.PhoneNumber,
                StoreAddress = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.StoreAddress,
                DeliveryAddress = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.DeliveryAddress,
                SellerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.SellerStoreId,
                SaleEmployeeId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.SaleEmployeeId,
                OrderDate = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.OrderDate,
                DeliveryDate = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.DeliveryDate,
                IndirectSalesOrderStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.IndirectSalesOrderStatusId,
                EditedPriceStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.EditedPriceStatusId,
                Note = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.Note,
                SubTotal = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.SubTotal,
                GeneralDiscountPercentage = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.GeneralDiscountPercentage,
                GeneralDiscountAmount = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.GeneralDiscountAmount,
                TotalTaxAmount = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.TotalTaxAmount,
                Total = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.IndirectSalesOrder.Total,
            };
            IndirectSalesOrderPromotion.Item = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item == null ? null : new Item
            {
                Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.Id,
                ProductId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.ProductId,
                Code = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.Code,
                Name = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.Name,
                ScanCode = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.ScanCode,
                SalePrice = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.SalePrice,
                RetailPrice = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.RetailPrice,
                StatusId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.Item.StatusId,
            };
            IndirectSalesOrderPromotion.PrimaryUnitOfMeasure = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Id,
                Code = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Code,
                Name = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Name,
                Description = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.Description,
                StatusId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.PrimaryUnitOfMeasure.StatusId,
            };
            IndirectSalesOrderPromotion.UnitOfMeasure = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure.Id,
                Code = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure.Code,
                Name = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure.Name,
                Description = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure.Description,
                StatusId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionDTO.UnitOfMeasure.StatusId,
            };
            IndirectSalesOrderPromotion.BaseLanguage = CurrentContext.Language;
            return IndirectSalesOrderPromotion;
        }

        private IndirectSalesOrderPromotionFilter ConvertFilterDTOToFilterEntity(IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO)
        {
            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
            IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.ALL;
            IndirectSalesOrderPromotionFilter.Skip = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.Skip;
            IndirectSalesOrderPromotionFilter.Take = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.Take;
            IndirectSalesOrderPromotionFilter.OrderBy = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.OrderBy;
            IndirectSalesOrderPromotionFilter.OrderType = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.OrderType;

            IndirectSalesOrderPromotionFilter.Id = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.Id;
            IndirectSalesOrderPromotionFilter.IndirectSalesOrderId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderPromotionFilter.ItemId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.ItemId;
            IndirectSalesOrderPromotionFilter.UnitOfMeasureId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.Quantity = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.Quantity;
            IndirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.RequestedQuantity = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.RequestedQuantity;
            IndirectSalesOrderPromotionFilter.Note = IndirectSalesOrderPromotion_IndirectSalesOrderPromotionFilterDTO.Note;
            return IndirectSalesOrderPromotionFilter;
        }

        [Route(IndirectSalesOrderPromotionRoute.FilterListIndirectSalesOrder), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_IndirectSalesOrderDTO>> FilterListIndirectSalesOrder([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = 20;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
            IndirectSalesOrderFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Id = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.IndirectSalesOrderStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Total;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrderPromotion_IndirectSalesOrderDTO> IndirectSalesOrderPromotion_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new IndirectSalesOrderPromotion_IndirectSalesOrderDTO(x)).ToList();
            return IndirectSalesOrderPromotion_IndirectSalesOrderDTOs;
        }
        [Route(IndirectSalesOrderPromotionRoute.FilterListItem), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_ItemDTO>> FilterListItem([FromBody] IndirectSalesOrderPromotion_ItemFilterDTO IndirectSalesOrderPromotion_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrderPromotion_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrderPromotion_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrderPromotion_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrderPromotion_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrderPromotion_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrderPromotion_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrderPromotion_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectSalesOrderPromotion_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrderPromotion_ItemDTO> IndirectSalesOrderPromotion_ItemDTOs = Items
                .Select(x => new IndirectSalesOrderPromotion_ItemDTO(x)).ToList();
            return IndirectSalesOrderPromotion_ItemDTOs;
        }
        [Route(IndirectSalesOrderPromotionRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrderPromotion_UnitOfMeasureDTO> IndirectSalesOrderPromotion_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrderPromotion_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrderPromotion_UnitOfMeasureDTOs;
        }

        [Route(IndirectSalesOrderPromotionRoute.SingleListIndirectSalesOrder), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_IndirectSalesOrderDTO>> SingleListIndirectSalesOrder([FromBody] IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = 20;
            IndirectSalesOrderFilter.OrderBy = IndirectSalesOrderOrder.Id;
            IndirectSalesOrderFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.ALL;
            IndirectSalesOrderFilter.Id = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Id;
            IndirectSalesOrderFilter.Code = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.IndirectSalesOrderStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO.Total;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrderPromotion_IndirectSalesOrderDTO> IndirectSalesOrderPromotion_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(x => new IndirectSalesOrderPromotion_IndirectSalesOrderDTO(x)).ToList();
            return IndirectSalesOrderPromotion_IndirectSalesOrderDTOs;
        }
        [Route(IndirectSalesOrderPromotionRoute.SingleListItem), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_ItemDTO>> SingleListItem([FromBody] IndirectSalesOrderPromotion_ItemFilterDTO IndirectSalesOrderPromotion_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrderPromotion_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrderPromotion_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrderPromotion_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrderPromotion_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrderPromotion_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrderPromotion_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrderPromotion_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrderPromotion_ItemDTO> IndirectSalesOrderPromotion_ItemDTOs = Items
                .Select(x => new IndirectSalesOrderPromotion_ItemDTO(x)).ToList();
            return IndirectSalesOrderPromotion_ItemDTOs;
        }
        [Route(IndirectSalesOrderPromotionRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrderPromotion_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrderPromotion_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrderPromotion_UnitOfMeasureDTO> IndirectSalesOrderPromotion_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrderPromotion_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrderPromotion_UnitOfMeasureDTOs;
        }

    }
}

