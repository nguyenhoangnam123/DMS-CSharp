using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MAppUser;
using DMS.Services.MIndirectSalesOrder;
using DMS.Services.MIndirectSalesOrderContent;
using DMS.Services.MIndirectSalesOrderPromotion;
using DMS.Services.MIndirectSalesOrderStatus;
using DMS.Services.MItem;
using DMS.Services.MStore;
using DMS.Services.MUnitOfMeasure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrderRoute : Root
    {
        public const string Master = Module + "/indirect-sales-order/indirect-sales-order-master";
        public const string Detail = Module + "/indirect-sales-order/indirect-sales-order-detail";
        private const string Default = Rpc + Module + "/indirect-sales-order";
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

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListIndirectSalesOrderStatus = Default + "/filter-list-indirect-sales-order-status";
        public const string FilterListIndirectSalesOrderContent = Default + "/filter-list-indirect-sales-order-content";
        public const string FilterListIndirectSalesOrderPromotion = Default + "/filter-list-indirect-sales-order-promotion";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListIndirectSalesOrderStatus = Default + "/single-list-indirect-sales-order-status";
        public const string SingleListIndirectSalesOrderContent = Default + "/single-list-indirect-sales-order-content";
        public const string SingleListIndirectSalesOrderPromotion = Default + "/single-list-indirect-sales-order-promotion";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectSalesOrderFilter.Id), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.Code), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.BuyerStoreId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.PhoneNumber), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.StoreAddress), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.DeliveryAddress), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.SellerStoreId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.SaleEmployeeId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.OrderDate), FieldType.DATE },
            { nameof(IndirectSalesOrderFilter.DeliveryDate), FieldType.DATE },
            { nameof(IndirectSalesOrderFilter.IndirectSalesOrderStatusId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.EditedPriceStatusId), FieldType.ID },
            { nameof(IndirectSalesOrderFilter.Note), FieldType.STRING },
            { nameof(IndirectSalesOrderFilter.SubTotal), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.GeneralDiscountPercentage), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.GeneralDiscountAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.TotalTaxAmount), FieldType.LONG },
            { nameof(IndirectSalesOrderFilter.Total), FieldType.LONG },
        };
    }

    public class IndirectSalesOrderController : RpcController
    {
        private IStoreService StoreService;
        private IIndirectSalesOrderStatusService IndirectSalesOrderStatusService;
        private IAppUserService AppUserService;
        private IIndirectSalesOrderContentService IndirectSalesOrderContentService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IIndirectSalesOrderPromotionService IndirectSalesOrderPromotionService;
        private IItemService ItemService;
        private IIndirectSalesOrderService IndirectSalesOrderService;
        private ICurrentContext CurrentContext;
        public IndirectSalesOrderController(
            IStoreService StoreService,
            IIndirectSalesOrderStatusService IndirectSalesOrderStatusService,
            IAppUserService AppUserService,
            IIndirectSalesOrderContentService IndirectSalesOrderContentService,
            IUnitOfMeasureService UnitOfMeasureService,
            IIndirectSalesOrderPromotionService IndirectSalesOrderPromotionService,
            IItemService ItemService,
            IIndirectSalesOrderService IndirectSalesOrderService,
            ICurrentContext CurrentContext
        )
        {
            this.StoreService = StoreService;
            this.IndirectSalesOrderStatusService = IndirectSalesOrderStatusService;
            this.AppUserService = AppUserService;
            this.IndirectSalesOrderContentService = IndirectSalesOrderContentService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.IndirectSalesOrderPromotionService = IndirectSalesOrderPromotionService;
            this.ItemService = ItemService;
            this.IndirectSalesOrderService = IndirectSalesOrderService;
            this.CurrentContext = CurrentContext;
        }

        [Route(IndirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            int count = await IndirectSalesOrderService.Count(IndirectSalesOrderFilter);
            return count;
        }

        [Route(IndirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<IndirectSalesOrder_IndirectSalesOrderDTO>>> List([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            List<IndirectSalesOrder_IndirectSalesOrderDTO> IndirectSalesOrder_IndirectSalesOrderDTOs = IndirectSalesOrders
                .Select(c => new IndirectSalesOrder_IndirectSalesOrderDTO(c)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderDTOs;
        }

        [Route(IndirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<IndirectSalesOrder_IndirectSalesOrderDTO>> Get([FromBody]IndirectSalesOrder_IndirectSalesOrderDTO IndirectSalesOrder_IndirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(IndirectSalesOrder_IndirectSalesOrderDTO.Id))
                return Forbid();

            IndirectSalesOrder IndirectSalesOrder = await IndirectSalesOrderService.Get(IndirectSalesOrder_IndirectSalesOrderDTO.Id);
            return new IndirectSalesOrder_IndirectSalesOrderDTO(IndirectSalesOrder);
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
        
        [Route(IndirectSalesOrderRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
            IndirectSalesOrderFilter.Id = new IdFilter { In = Ids };
            IndirectSalesOrderFilter.Selects = IndirectSalesOrderSelect.Id;
            IndirectSalesOrderFilter.Skip = 0;
            IndirectSalesOrderFilter.Take = int.MaxValue;

            List<IndirectSalesOrder> IndirectSalesOrders = await IndirectSalesOrderService.List(IndirectSalesOrderFilter);
            IndirectSalesOrders = await IndirectSalesOrderService.BulkDelete(IndirectSalesOrders);
            return true;
        }
        
        [Route(IndirectSalesOrderRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            StoreFilter BuyerStoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };
            List<Store> BuyerStores = await StoreService.List(BuyerStoreFilter);
            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = IndirectSalesOrderStatusSelect.ALL
            };
            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);
            AppUserFilter SaleEmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> SaleEmployees = await AppUserService.List(SaleEmployeeFilter);
            StoreFilter SellerStoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };
            List<Store> SellerStores = await StoreService.List(SellerStoreFilter);
            List<IndirectSalesOrder> IndirectSalesOrders = new List<IndirectSalesOrder>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(IndirectSalesOrders);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int BuyerStoreIdColumn = 2 + StartColumn;
                int PhoneNumberColumn = 3 + StartColumn;
                int StoreAddressColumn = 4 + StartColumn;
                int DeliveryAddressColumn = 5 + StartColumn;
                int SellerStoreIdColumn = 6 + StartColumn;
                int SaleEmployeeIdColumn = 7 + StartColumn;
                int OrderDateColumn = 8 + StartColumn;
                int DeliveryDateColumn = 9 + StartColumn;
                int IndirectSalesOrderStatusIdColumn = 10 + StartColumn;
                int EditedPriceStatusIdColumn = 11 + StartColumn;
                int NoteColumn = 12 + StartColumn;
                int SubTotalColumn = 13 + StartColumn;
                int GeneralDiscountPercentageColumn = 14 + StartColumn;
                int GeneralDiscountAmountColumn = 15 + StartColumn;
                int TotalTaxAmountColumn = 16 + StartColumn;
                int TotalColumn = 17 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string BuyerStoreIdValue = worksheet.Cells[i + StartRow, BuyerStoreIdColumn].Value?.ToString();
                    string PhoneNumberValue = worksheet.Cells[i + StartRow, PhoneNumberColumn].Value?.ToString();
                    string StoreAddressValue = worksheet.Cells[i + StartRow, StoreAddressColumn].Value?.ToString();
                    string DeliveryAddressValue = worksheet.Cells[i + StartRow, DeliveryAddressColumn].Value?.ToString();
                    string SellerStoreIdValue = worksheet.Cells[i + StartRow, SellerStoreIdColumn].Value?.ToString();
                    string SaleEmployeeIdValue = worksheet.Cells[i + StartRow, SaleEmployeeIdColumn].Value?.ToString();
                    string OrderDateValue = worksheet.Cells[i + StartRow, OrderDateColumn].Value?.ToString();
                    string DeliveryDateValue = worksheet.Cells[i + StartRow, DeliveryDateColumn].Value?.ToString();
                    string IndirectSalesOrderStatusIdValue = worksheet.Cells[i + StartRow, IndirectSalesOrderStatusIdColumn].Value?.ToString();
                    string EditedPriceStatusIdValue = worksheet.Cells[i + StartRow, EditedPriceStatusIdColumn].Value?.ToString();
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    string SubTotalValue = worksheet.Cells[i + StartRow, SubTotalColumn].Value?.ToString();
                    string GeneralDiscountPercentageValue = worksheet.Cells[i + StartRow, GeneralDiscountPercentageColumn].Value?.ToString();
                    string GeneralDiscountAmountValue = worksheet.Cells[i + StartRow, GeneralDiscountAmountColumn].Value?.ToString();
                    string TotalTaxAmountValue = worksheet.Cells[i + StartRow, TotalTaxAmountColumn].Value?.ToString();
                    string TotalValue = worksheet.Cells[i + StartRow, TotalColumn].Value?.ToString();
                    
                    IndirectSalesOrder IndirectSalesOrder = new IndirectSalesOrder();
                    IndirectSalesOrder.Code = CodeValue;
                    IndirectSalesOrder.PhoneNumber = PhoneNumberValue;
                    IndirectSalesOrder.StoreAddress = StoreAddressValue;
                    IndirectSalesOrder.DeliveryAddress = DeliveryAddressValue;
                    IndirectSalesOrder.OrderDate = DateTime.TryParse(OrderDateValue, out DateTime OrderDate) ? OrderDate : DateTime.Now;
                    IndirectSalesOrder.DeliveryDate = DateTime.TryParse(DeliveryDateValue, out DateTime DeliveryDate) ? DeliveryDate : DateTime.Now;
                    IndirectSalesOrder.Note = NoteValue;
                    IndirectSalesOrder.SubTotal = long.TryParse(SubTotalValue, out long SubTotal) ? SubTotal : 0;
                    IndirectSalesOrder.GeneralDiscountPercentage = long.TryParse(GeneralDiscountPercentageValue, out long GeneralDiscountPercentage) ? GeneralDiscountPercentage : 0;
                    IndirectSalesOrder.GeneralDiscountAmount = long.TryParse(GeneralDiscountAmountValue, out long GeneralDiscountAmount) ? GeneralDiscountAmount : 0;
                    IndirectSalesOrder.TotalTaxAmount = long.TryParse(TotalTaxAmountValue, out long TotalTaxAmount) ? TotalTaxAmount : 0;
                    IndirectSalesOrder.Total = long.TryParse(TotalValue, out long Total) ? Total : 0;
                    Store BuyerStore = BuyerStores.Where(x => x.Id.ToString() == BuyerStoreIdValue).FirstOrDefault();
                    IndirectSalesOrder.BuyerStoreId = BuyerStore == null ? 0 : BuyerStore.Id;
                    IndirectSalesOrder.BuyerStore = BuyerStore;
                    IndirectSalesOrderStatus IndirectSalesOrderStatus = IndirectSalesOrderStatuses.Where(x => x.Id.ToString() == IndirectSalesOrderStatusIdValue).FirstOrDefault();
                    IndirectSalesOrder.IndirectSalesOrderStatusId = IndirectSalesOrderStatus == null ? 0 : IndirectSalesOrderStatus.Id;
                    IndirectSalesOrder.IndirectSalesOrderStatus = IndirectSalesOrderStatus;
                    AppUser SaleEmployee = SaleEmployees.Where(x => x.Id.ToString() == SaleEmployeeIdValue).FirstOrDefault();
                    IndirectSalesOrder.SaleEmployeeId = SaleEmployee == null ? 0 : SaleEmployee.Id;
                    IndirectSalesOrder.SaleEmployee = SaleEmployee;
                    Store SellerStore = SellerStores.Where(x => x.Id.ToString() == SellerStoreIdValue).FirstOrDefault();
                    IndirectSalesOrder.SellerStoreId = SellerStore == null ? 0 : SellerStore.Id;
                    IndirectSalesOrder.SellerStore = SellerStore;
                    
                    IndirectSalesOrders.Add(IndirectSalesOrder);
                }
            }
            IndirectSalesOrders = await IndirectSalesOrderService.Import(IndirectSalesOrders);
            if (IndirectSalesOrders.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < IndirectSalesOrders.Count; i++)
                {
                    IndirectSalesOrder IndirectSalesOrder = IndirectSalesOrders[i];
                    if (!IndirectSalesOrder.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.Id)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.Id)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.Code)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.Code)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.BuyerStoreId)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.BuyerStoreId)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.PhoneNumber)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.PhoneNumber)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.StoreAddress)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.StoreAddress)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.DeliveryAddress)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.DeliveryAddress)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.SellerStoreId)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.SellerStoreId)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.SaleEmployeeId)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.SaleEmployeeId)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.OrderDate)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.OrderDate)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.DeliveryDate)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.DeliveryDate)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.IndirectSalesOrderStatusId)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.IndirectSalesOrderStatusId)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.EditedPriceStatusId)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.EditedPriceStatusId)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.Note)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.Note)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.SubTotal)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.SubTotal)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.GeneralDiscountPercentage)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.GeneralDiscountPercentage)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.GeneralDiscountAmount)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.GeneralDiscountAmount)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.TotalTaxAmount)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.TotalTaxAmount)];
                        if (IndirectSalesOrder.Errors.ContainsKey(nameof(IndirectSalesOrder.Total)))
                            Error += IndirectSalesOrder.Errors[nameof(IndirectSalesOrder.Total)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(IndirectSalesOrderRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrder
                var IndirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(IndirectSalesOrder_IndirectSalesOrderFilterDTO);
                IndirectSalesOrderFilter.Skip = 0;
                IndirectSalesOrderFilter.Take = int.MaxValue;
                IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
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
                
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "StoreStatusId",
                        "StatusId",
                        "WorkflowDefinitionId",
                        "RequestStateId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.StoreStatusId,
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region IndirectSalesOrderStatus
                var IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter();
                IndirectSalesOrderStatusFilter.Selects = IndirectSalesOrderStatusSelect.ALL;
                IndirectSalesOrderStatusFilter.OrderBy = IndirectSalesOrderStatusOrder.Id;
                IndirectSalesOrderStatusFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderStatusFilter.Skip = 0;
                IndirectSalesOrderStatusFilter.Take = int.MaxValue;
                List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);

                var IndirectSalesOrderStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> IndirectSalesOrderStatusData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrderStatuses.Count; i++)
                {
                    var IndirectSalesOrderStatus = IndirectSalesOrderStatuses[i];
                    IndirectSalesOrderStatusData.Add(new Object[]
                    {
                        IndirectSalesOrderStatus.Id,
                        IndirectSalesOrderStatus.Code,
                        IndirectSalesOrderStatus.Name,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrderStatus", IndirectSalesOrderStatusHeaders, IndirectSalesOrderStatusData);
                #endregion
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "Password",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "Position",
                        "Department",
                        "OrganizationId",
                        "SexId",
                        "StatusId",
                        "Avatar",
                        "Birthday",
                        "ProvinceId",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.Password,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.Position,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.SexId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.Birthday,
                        AppUser.ProvinceId,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region IndirectSalesOrderContent
                var IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
                IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.ALL;
                IndirectSalesOrderContentFilter.OrderBy = IndirectSalesOrderContentOrder.Id;
                IndirectSalesOrderContentFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderContentFilter.Skip = 0;
                IndirectSalesOrderContentFilter.Take = int.MaxValue;
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
                #region IndirectSalesOrderPromotion
                var IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
                IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.ALL;
                IndirectSalesOrderPromotionFilter.OrderBy = IndirectSalesOrderPromotionOrder.Id;
                IndirectSalesOrderPromotionFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderPromotionFilter.Skip = 0;
                IndirectSalesOrderPromotionFilter.Take = int.MaxValue;
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrder.xlsx");
        }

        [Route(IndirectSalesOrderRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] IndirectSalesOrder_IndirectSalesOrderFilterDTO IndirectSalesOrder_IndirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region IndirectSalesOrder
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
                excel.GenerateWorksheet("IndirectSalesOrder", IndirectSalesOrderHeaders, IndirectSalesOrderData);
                #endregion
                
                #region Store
                var StoreFilter = new StoreFilter();
                StoreFilter.Selects = StoreSelect.ALL;
                StoreFilter.OrderBy = StoreOrder.Id;
                StoreFilter.OrderType = OrderType.ASC;
                StoreFilter.Skip = 0;
                StoreFilter.Take = int.MaxValue;
                List<Store> Stores = await StoreService.List(StoreFilter);

                var StoreHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "ParentStoreId",
                        "OrganizationId",
                        "StoreTypeId",
                        "StoreGroupingId",
                        "ResellerId",
                        "Telephone",
                        "ProvinceId",
                        "DistrictId",
                        "WardId",
                        "Address",
                        "DeliveryAddress",
                        "Latitude",
                        "Longitude",
                        "DeliveryLatitude",
                        "DeliveryLongitude",
                        "OwnerName",
                        "OwnerPhone",
                        "OwnerEmail",
                        "StoreStatusId",
                        "StatusId",
                        "WorkflowDefinitionId",
                        "RequestStateId",
                    }
                };
                List<object[]> StoreData = new List<object[]>();
                for (int i = 0; i < Stores.Count; i++)
                {
                    var Store = Stores[i];
                    StoreData.Add(new Object[]
                    {
                        Store.Id,
                        Store.Code,
                        Store.Name,
                        Store.ParentStoreId,
                        Store.OrganizationId,
                        Store.StoreTypeId,
                        Store.StoreGroupingId,
                        Store.ResellerId,
                        Store.Telephone,
                        Store.ProvinceId,
                        Store.DistrictId,
                        Store.WardId,
                        Store.Address,
                        Store.DeliveryAddress,
                        Store.Latitude,
                        Store.Longitude,
                        Store.DeliveryLatitude,
                        Store.DeliveryLongitude,
                        Store.OwnerName,
                        Store.OwnerPhone,
                        Store.OwnerEmail,
                        Store.StoreStatusId,
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region IndirectSalesOrderStatus
                var IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter();
                IndirectSalesOrderStatusFilter.Selects = IndirectSalesOrderStatusSelect.ALL;
                IndirectSalesOrderStatusFilter.OrderBy = IndirectSalesOrderStatusOrder.Id;
                IndirectSalesOrderStatusFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderStatusFilter.Skip = 0;
                IndirectSalesOrderStatusFilter.Take = int.MaxValue;
                List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);

                var IndirectSalesOrderStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> IndirectSalesOrderStatusData = new List<object[]>();
                for (int i = 0; i < IndirectSalesOrderStatuses.Count; i++)
                {
                    var IndirectSalesOrderStatus = IndirectSalesOrderStatuses[i];
                    IndirectSalesOrderStatusData.Add(new Object[]
                    {
                        IndirectSalesOrderStatus.Id,
                        IndirectSalesOrderStatus.Code,
                        IndirectSalesOrderStatus.Name,
                    });
                }
                excel.GenerateWorksheet("IndirectSalesOrderStatus", IndirectSalesOrderStatusHeaders, IndirectSalesOrderStatusData);
                #endregion
                #region AppUser
                var AppUserFilter = new AppUserFilter();
                AppUserFilter.Selects = AppUserSelect.ALL;
                AppUserFilter.OrderBy = AppUserOrder.Id;
                AppUserFilter.OrderType = OrderType.ASC;
                AppUserFilter.Skip = 0;
                AppUserFilter.Take = int.MaxValue;
                List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

                var AppUserHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Username",
                        "Password",
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "Position",
                        "Department",
                        "OrganizationId",
                        "SexId",
                        "StatusId",
                        "Avatar",
                        "Birthday",
                        "ProvinceId",
                    }
                };
                List<object[]> AppUserData = new List<object[]>();
                for (int i = 0; i < AppUsers.Count; i++)
                {
                    var AppUser = AppUsers[i];
                    AppUserData.Add(new Object[]
                    {
                        AppUser.Id,
                        AppUser.Username,
                        AppUser.Password,
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.Position,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.SexId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.Birthday,
                        AppUser.ProvinceId,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region IndirectSalesOrderContent
                var IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
                IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.ALL;
                IndirectSalesOrderContentFilter.OrderBy = IndirectSalesOrderContentOrder.Id;
                IndirectSalesOrderContentFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderContentFilter.Skip = 0;
                IndirectSalesOrderContentFilter.Take = int.MaxValue;
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
                #region IndirectSalesOrderPromotion
                var IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
                IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.ALL;
                IndirectSalesOrderPromotionFilter.OrderBy = IndirectSalesOrderPromotionOrder.Id;
                IndirectSalesOrderPromotionFilter.OrderType = OrderType.ASC;
                IndirectSalesOrderPromotionFilter.Skip = 0;
                IndirectSalesOrderPromotionFilter.Take = int.MaxValue;
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "IndirectSalesOrder.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            IndirectSalesOrderFilter IndirectSalesOrderFilter = new IndirectSalesOrderFilter();
            IndirectSalesOrderFilter = IndirectSalesOrderService.ToFilter(IndirectSalesOrderFilter);
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
            IndirectSalesOrder.OrderDate = IndirectSalesOrder_IndirectSalesOrderDTO.OrderDate;
            IndirectSalesOrder.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderDTO.DeliveryDate;
            IndirectSalesOrder.IndirectSalesOrderStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrder.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatusId;
            IndirectSalesOrder.Note = IndirectSalesOrder_IndirectSalesOrderDTO.Note;
            IndirectSalesOrder.SubTotal = IndirectSalesOrder_IndirectSalesOrderDTO.SubTotal;
            IndirectSalesOrder.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountPercentage;
            IndirectSalesOrder.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderDTO.GeneralDiscountAmount;
            IndirectSalesOrder.TotalTaxAmount = IndirectSalesOrder_IndirectSalesOrderDTO.TotalTaxAmount;
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
                ResellerId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.ResellerId,
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
                StoreStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StoreStatusId,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.StatusId,
                WorkflowDefinitionId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.WorkflowDefinitionId,
                RequestStateId = IndirectSalesOrder_IndirectSalesOrderDTO.BuyerStore.RequestStateId,
            };
            IndirectSalesOrder.EditedPriceStatus = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            IndirectSalesOrder.IndirectSalesOrderStatus = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderStatus == null ? null : new IndirectSalesOrderStatus
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderStatus.Id,
                Code = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderStatus.Code,
                Name = IndirectSalesOrder_IndirectSalesOrderDTO.IndirectSalesOrderStatus.Name,
            };
            IndirectSalesOrder.SaleEmployee = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Id,
                Username = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Username,
                Password = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Password,
                DisplayName = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Address,
                Email = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Email,
                Phone = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Phone,
                Position = IndirectSalesOrder_IndirectSalesOrderDTO.SaleEmployee.Position,
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
                ResellerId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.ResellerId,
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
                StoreStatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StoreStatusId,
                StatusId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.StatusId,
                WorkflowDefinitionId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.WorkflowDefinitionId,
                RequestStateId = IndirectSalesOrder_IndirectSalesOrderDTO.SellerStore.RequestStateId,
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
                    SalePrice = x.SalePrice,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountAmount = x.DiscountAmount,
                    GeneralDiscountPercentage = x.GeneralDiscountPercentage,
                    GeneralDiscountAmount = x.GeneralDiscountAmount,
                    Amount = x.Amount,
                    TaxPercentage = x.TaxPercentage,
                    TaxAmount = x.TaxAmount,
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
            IndirectSalesOrderFilter.Code = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Code;
            IndirectSalesOrderFilter.BuyerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.BuyerStoreId;
            IndirectSalesOrderFilter.PhoneNumber = IndirectSalesOrder_IndirectSalesOrderFilterDTO.PhoneNumber;
            IndirectSalesOrderFilter.StoreAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.StoreAddress;
            IndirectSalesOrderFilter.DeliveryAddress = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryAddress;
            IndirectSalesOrderFilter.SellerStoreId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SellerStoreId;
            IndirectSalesOrderFilter.SaleEmployeeId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SaleEmployeeId;
            IndirectSalesOrderFilter.OrderDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.OrderDate;
            IndirectSalesOrderFilter.DeliveryDate = IndirectSalesOrder_IndirectSalesOrderFilterDTO.DeliveryDate;
            IndirectSalesOrderFilter.IndirectSalesOrderStatusId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.IndirectSalesOrderStatusId;
            IndirectSalesOrderFilter.EditedPriceStatusId = IndirectSalesOrder_IndirectSalesOrderFilterDTO.EditedPriceStatusId;
            IndirectSalesOrderFilter.Note = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Note;
            IndirectSalesOrderFilter.SubTotal = IndirectSalesOrder_IndirectSalesOrderFilterDTO.SubTotal;
            IndirectSalesOrderFilter.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderFilter.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderFilter.TotalTaxAmount = IndirectSalesOrder_IndirectSalesOrderFilterDTO.TotalTaxAmount;
            IndirectSalesOrderFilter.Total = IndirectSalesOrder_IndirectSalesOrderFilterDTO.Total;
            return IndirectSalesOrderFilter;
        }

        [Route(IndirectSalesOrderRoute.FilterListStore), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreDTO>> FilterListStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = IndirectSalesOrder_StoreFilterDTO.StoreStatusId;
            StoreFilter.StatusId = IndirectSalesOrder_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectSalesOrder_StoreDTO> IndirectSalesOrder_StoreDTOs = Stores
                .Select(x => new IndirectSalesOrder_StoreDTO(x)).ToList();
            return IndirectSalesOrder_StoreDTOs;
        }

        [Route(IndirectSalesOrderRoute.FilterListIndirectSalesOrderStatus), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderStatusDTO>> FilterListIndirectSalesOrderStatus([FromBody] IndirectSalesOrder_IndirectSalesOrderStatusFilterDTO IndirectSalesOrder_IndirectSalesOrderStatusFilterDTO)
        {
            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter();
            IndirectSalesOrderStatusFilter.Skip = 0;
            IndirectSalesOrderStatusFilter.Take = int.MaxValue;
            IndirectSalesOrderStatusFilter.Take = 20;
            IndirectSalesOrderStatusFilter.OrderBy = IndirectSalesOrderStatusOrder.Id;
            IndirectSalesOrderStatusFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderStatusFilter.Selects = IndirectSalesOrderStatusSelect.ALL;

            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);
            List<IndirectSalesOrder_IndirectSalesOrderStatusDTO> IndirectSalesOrder_IndirectSalesOrderStatusDTOs = IndirectSalesOrderStatuses
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderStatusDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderStatusDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<IndirectSalesOrder_AppUserDTO>> FilterListAppUser([FromBody] IndirectSalesOrder_AppUserFilterDTO IndirectSalesOrder_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = IndirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = IndirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = IndirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = IndirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = IndirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = IndirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = IndirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.Position = IndirectSalesOrder_AppUserFilterDTO.Position;
            AppUserFilter.Department = IndirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = IndirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = IndirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = IndirectSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = IndirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = IndirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<IndirectSalesOrder_AppUserDTO> IndirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new IndirectSalesOrder_AppUserDTO(x)).ToList();
            return IndirectSalesOrder_AppUserDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListIndirectSalesOrderContent), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderContentDTO>> FilterListIndirectSalesOrderContent([FromBody] IndirectSalesOrder_IndirectSalesOrderContentFilterDTO IndirectSalesOrder_IndirectSalesOrderContentFilterDTO)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
            IndirectSalesOrderContentFilter.Skip = 0;
            IndirectSalesOrderContentFilter.Take = 20;
            IndirectSalesOrderContentFilter.OrderBy = IndirectSalesOrderContentOrder.Id;
            IndirectSalesOrderContentFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.ALL;
            IndirectSalesOrderContentFilter.Id = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Id;
            IndirectSalesOrderContentFilter.IndirectSalesOrderId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderContentFilter.ItemId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.ItemId;
            IndirectSalesOrderContentFilter.UnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderContentFilter.Quantity = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Quantity;
            IndirectSalesOrderContentFilter.PrimaryUnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderContentFilter.RequestedQuantity = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.RequestedQuantity;
            IndirectSalesOrderContentFilter.SalePrice = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.SalePrice;
            IndirectSalesOrderContentFilter.DiscountPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.DiscountPercentage;
            IndirectSalesOrderContentFilter.DiscountAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.DiscountAmount;
            IndirectSalesOrderContentFilter.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderContentFilter.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderContentFilter.Amount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Amount;
            IndirectSalesOrderContentFilter.TaxPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.TaxPercentage;
            IndirectSalesOrderContentFilter.TaxAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.TaxAmount;

            List<IndirectSalesOrderContent> IndirectSalesOrderContents = await IndirectSalesOrderContentService.List(IndirectSalesOrderContentFilter);
            List<IndirectSalesOrder_IndirectSalesOrderContentDTO> IndirectSalesOrder_IndirectSalesOrderContentDTOs = IndirectSalesOrderContents
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderContentDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderContentDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrder_UnitOfMeasureDTO>> FilterListUnitOfMeasure([FromBody] IndirectSalesOrder_UnitOfMeasureFilterDTO IndirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = IndirectSalesOrder_UnitOfMeasureFilterDTO.StatusId;

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrder_UnitOfMeasureDTO> IndirectSalesOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrder_UnitOfMeasureDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListIndirectSalesOrderPromotion), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderPromotionDTO>> FilterListIndirectSalesOrderPromotion([FromBody] IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO)
        {
            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
            IndirectSalesOrderPromotionFilter.Skip = 0;
            IndirectSalesOrderPromotionFilter.Take = 20;
            IndirectSalesOrderPromotionFilter.OrderBy = IndirectSalesOrderPromotionOrder.Id;
            IndirectSalesOrderPromotionFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.ALL;
            IndirectSalesOrderPromotionFilter.Id = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Id;
            IndirectSalesOrderPromotionFilter.IndirectSalesOrderId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderPromotionFilter.ItemId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.ItemId;
            IndirectSalesOrderPromotionFilter.UnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.Quantity = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Quantity;
            IndirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.RequestedQuantity = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.RequestedQuantity;
            IndirectSalesOrderPromotionFilter.Note = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Note;

            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.List(IndirectSalesOrderPromotionFilter);
            List<IndirectSalesOrder_IndirectSalesOrderPromotionDTO> IndirectSalesOrder_IndirectSalesOrderPromotionDTOs = IndirectSalesOrderPromotions
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderPromotionDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderPromotionDTOs;
        }
        [Route(IndirectSalesOrderRoute.FilterListItem), HttpPost]
        public async Task<List<IndirectSalesOrder_ItemDTO>> FilterListItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = IndirectSalesOrder_ItemFilterDTO.StatusId;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
            return IndirectSalesOrder_ItemDTOs;
        }

        [Route(IndirectSalesOrderRoute.SingleListStore), HttpPost]
        public async Task<List<IndirectSalesOrder_StoreDTO>> SingleListStore([FromBody] IndirectSalesOrder_StoreFilterDTO IndirectSalesOrder_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = IndirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = IndirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = IndirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = IndirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = IndirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = IndirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = IndirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = IndirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = IndirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = IndirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = IndirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = IndirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = IndirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = IndirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = IndirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = IndirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = IndirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = IndirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = IndirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = IndirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StoreStatusId = IndirectSalesOrder_StoreFilterDTO.StoreStatusId;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<IndirectSalesOrder_StoreDTO> IndirectSalesOrder_StoreDTOs = Stores
                .Select(x => new IndirectSalesOrder_StoreDTO(x)).ToList();
            return IndirectSalesOrder_StoreDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListIndirectSalesOrderStatus), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderStatusDTO>> SingleListIndirectSalesOrderStatus([FromBody] IndirectSalesOrder_IndirectSalesOrderStatusFilterDTO IndirectSalesOrder_IndirectSalesOrderStatusFilterDTO)
        {
            IndirectSalesOrderStatusFilter IndirectSalesOrderStatusFilter = new IndirectSalesOrderStatusFilter();
            IndirectSalesOrderStatusFilter.Skip = 0;
            IndirectSalesOrderStatusFilter.Take = int.MaxValue;
            IndirectSalesOrderStatusFilter.Take = 20;
            IndirectSalesOrderStatusFilter.OrderBy = IndirectSalesOrderStatusOrder.Id;
            IndirectSalesOrderStatusFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderStatusFilter.Selects = IndirectSalesOrderStatusSelect.ALL;

            List<IndirectSalesOrderStatus> IndirectSalesOrderStatuses = await IndirectSalesOrderStatusService.List(IndirectSalesOrderStatusFilter);
            List<IndirectSalesOrder_IndirectSalesOrderStatusDTO> IndirectSalesOrder_IndirectSalesOrderStatusDTOs = IndirectSalesOrderStatuses
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderStatusDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderStatusDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListAppUser), HttpPost]
        public async Task<List<IndirectSalesOrder_AppUserDTO>> SingleListAppUser([FromBody] IndirectSalesOrder_AppUserFilterDTO IndirectSalesOrder_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = IndirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = IndirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = IndirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = IndirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = IndirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = IndirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = IndirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.Position = IndirectSalesOrder_AppUserFilterDTO.Position;
            AppUserFilter.Department = IndirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = IndirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = IndirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Birthday = IndirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = IndirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<IndirectSalesOrder_AppUserDTO> IndirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new IndirectSalesOrder_AppUserDTO(x)).ToList();
            return IndirectSalesOrder_AppUserDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListIndirectSalesOrderContent), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderContentDTO>> SingleListIndirectSalesOrderContent([FromBody] IndirectSalesOrder_IndirectSalesOrderContentFilterDTO IndirectSalesOrder_IndirectSalesOrderContentFilterDTO)
        {
            IndirectSalesOrderContentFilter IndirectSalesOrderContentFilter = new IndirectSalesOrderContentFilter();
            IndirectSalesOrderContentFilter.Skip = 0;
            IndirectSalesOrderContentFilter.Take = 20;
            IndirectSalesOrderContentFilter.OrderBy = IndirectSalesOrderContentOrder.Id;
            IndirectSalesOrderContentFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderContentFilter.Selects = IndirectSalesOrderContentSelect.ALL;
            IndirectSalesOrderContentFilter.Id = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Id;
            IndirectSalesOrderContentFilter.IndirectSalesOrderId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderContentFilter.ItemId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.ItemId;
            IndirectSalesOrderContentFilter.UnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderContentFilter.Quantity = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Quantity;
            IndirectSalesOrderContentFilter.PrimaryUnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderContentFilter.RequestedQuantity = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.RequestedQuantity;
            IndirectSalesOrderContentFilter.SalePrice = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.SalePrice;
            IndirectSalesOrderContentFilter.DiscountPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.DiscountPercentage;
            IndirectSalesOrderContentFilter.DiscountAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.DiscountAmount;
            IndirectSalesOrderContentFilter.GeneralDiscountPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.GeneralDiscountPercentage;
            IndirectSalesOrderContentFilter.GeneralDiscountAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.GeneralDiscountAmount;
            IndirectSalesOrderContentFilter.Amount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.Amount;
            IndirectSalesOrderContentFilter.TaxPercentage = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.TaxPercentage;
            IndirectSalesOrderContentFilter.TaxAmount = IndirectSalesOrder_IndirectSalesOrderContentFilterDTO.TaxAmount;

            List<IndirectSalesOrderContent> IndirectSalesOrderContents = await IndirectSalesOrderContentService.List(IndirectSalesOrderContentFilter);
            List<IndirectSalesOrder_IndirectSalesOrderContentDTO> IndirectSalesOrder_IndirectSalesOrderContentDTOs = IndirectSalesOrderContents
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderContentDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderContentDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<IndirectSalesOrder_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] IndirectSalesOrder_UnitOfMeasureFilterDTO IndirectSalesOrder_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = IndirectSalesOrder_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = IndirectSalesOrder_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = IndirectSalesOrder_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.Description = IndirectSalesOrder_UnitOfMeasureFilterDTO.Description;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<IndirectSalesOrder_UnitOfMeasureDTO> IndirectSalesOrder_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new IndirectSalesOrder_UnitOfMeasureDTO(x)).ToList();
            return IndirectSalesOrder_UnitOfMeasureDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListIndirectSalesOrderPromotion), HttpPost]
        public async Task<List<IndirectSalesOrder_IndirectSalesOrderPromotionDTO>> SingleListIndirectSalesOrderPromotion([FromBody] IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO)
        {
            IndirectSalesOrderPromotionFilter IndirectSalesOrderPromotionFilter = new IndirectSalesOrderPromotionFilter();
            IndirectSalesOrderPromotionFilter.Skip = 0;
            IndirectSalesOrderPromotionFilter.Take = 20;
            IndirectSalesOrderPromotionFilter.OrderBy = IndirectSalesOrderPromotionOrder.Id;
            IndirectSalesOrderPromotionFilter.OrderType = OrderType.ASC;
            IndirectSalesOrderPromotionFilter.Selects = IndirectSalesOrderPromotionSelect.ALL;
            IndirectSalesOrderPromotionFilter.Id = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Id;
            IndirectSalesOrderPromotionFilter.IndirectSalesOrderId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.IndirectSalesOrderId;
            IndirectSalesOrderPromotionFilter.ItemId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.ItemId;
            IndirectSalesOrderPromotionFilter.UnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.UnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.Quantity = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Quantity;
            IndirectSalesOrderPromotionFilter.PrimaryUnitOfMeasureId = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.PrimaryUnitOfMeasureId;
            IndirectSalesOrderPromotionFilter.RequestedQuantity = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.RequestedQuantity;
            IndirectSalesOrderPromotionFilter.Note = IndirectSalesOrder_IndirectSalesOrderPromotionFilterDTO.Note;

            List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions = await IndirectSalesOrderPromotionService.List(IndirectSalesOrderPromotionFilter);
            List<IndirectSalesOrder_IndirectSalesOrderPromotionDTO> IndirectSalesOrder_IndirectSalesOrderPromotionDTOs = IndirectSalesOrderPromotions
                .Select(x => new IndirectSalesOrder_IndirectSalesOrderPromotionDTO(x)).ToList();
            return IndirectSalesOrder_IndirectSalesOrderPromotionDTOs;
        }
        [Route(IndirectSalesOrderRoute.SingleListItem), HttpPost]
        public async Task<List<IndirectSalesOrder_ItemDTO>> SingleListItem([FromBody] IndirectSalesOrder_ItemFilterDTO IndirectSalesOrder_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = 0;
            ItemFilter.Take = 20;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = IndirectSalesOrder_ItemFilterDTO.Id;
            ItemFilter.ProductId = IndirectSalesOrder_ItemFilterDTO.ProductId;
            ItemFilter.Code = IndirectSalesOrder_ItemFilterDTO.Code;
            ItemFilter.Name = IndirectSalesOrder_ItemFilterDTO.Name;
            ItemFilter.ScanCode = IndirectSalesOrder_ItemFilterDTO.ScanCode;
            ItemFilter.SalePrice = IndirectSalesOrder_ItemFilterDTO.SalePrice;
            ItemFilter.RetailPrice = IndirectSalesOrder_ItemFilterDTO.RetailPrice;
            ItemFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Item> Items = await ItemService.List(ItemFilter);
            List<IndirectSalesOrder_ItemDTO> IndirectSalesOrder_ItemDTOs = Items
                .Select(x => new IndirectSalesOrder_ItemDTO(x)).ToList();
            return IndirectSalesOrder_ItemDTOs;
        }

    }
}

