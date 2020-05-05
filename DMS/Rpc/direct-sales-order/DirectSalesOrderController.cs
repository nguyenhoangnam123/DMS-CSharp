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
using DMS.Services.MDirectSalesOrder;
using DMS.Services.MStore;
using DMS.Services.MEditedPriceStatus;
using DMS.Services.MRequestState;
using DMS.Services.MAppUser;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrderRoute : Root
    {
        public const string Master = Module + "/direct-sales-order/direct-sales-order-master";
        public const string Detail = Module + "/direct-sales-order/direct-sales-order-detail";
        private const string Default = Rpc + Module + "/direct-sales-order";
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
        
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListEditedPriceStatus = Default + "/filter-list-edited-price-status";
        public const string FilterListRequestState = Default + "/filter-list-request-state";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListEditedPriceStatus = Default + "/single-list-edited-price-status";
        public const string SingleListRequestState = Default + "/single-list-request-state";
        public const string SingleListAppUser = Default + "/single-list-app-user";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(DirectSalesOrderFilter.Id), FieldType.ID },
            { nameof(DirectSalesOrderFilter.Code), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.BuyerStoreId), FieldType.ID },
            { nameof(DirectSalesOrderFilter.StorePhone), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.StoreAddress), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.StoreDeliveryAddress), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.TaxCode), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.SaleEmployeeId), FieldType.ID },
            { nameof(DirectSalesOrderFilter.OrderDate), FieldType.DATE },
            { nameof(DirectSalesOrderFilter.DeliveryDate), FieldType.DATE },
            { nameof(DirectSalesOrderFilter.EditedPriceStatusId), FieldType.ID },
            { nameof(DirectSalesOrderFilter.Note), FieldType.STRING },
            { nameof(DirectSalesOrderFilter.SubTotal), FieldType.LONG },
            { nameof(DirectSalesOrderFilter.GeneralDiscountPercentage), FieldType.DECIMAL },
            { nameof(DirectSalesOrderFilter.GeneralDiscountAmount), FieldType.LONG },
            { nameof(DirectSalesOrderFilter.TotalTaxAmount), FieldType.LONG },
            { nameof(DirectSalesOrderFilter.Total), FieldType.LONG },
            { nameof(DirectSalesOrderFilter.RequestStateId), FieldType.ID },
        };
    }

    public class DirectSalesOrderController : RpcController
    {
        private IStoreService StoreService;
        private IEditedPriceStatusService EditedPriceStatusService;
        private IRequestStateService RequestStateService;
        private IAppUserService AppUserService;
        private IDirectSalesOrderService DirectSalesOrderService;
        private ICurrentContext CurrentContext;
        public DirectSalesOrderController(
            IStoreService StoreService,
            IEditedPriceStatusService EditedPriceStatusService,
            IRequestStateService RequestStateService,
            IAppUserService AppUserService,
            IDirectSalesOrderService DirectSalesOrderService,
            ICurrentContext CurrentContext
        )
        {
            this.StoreService = StoreService;
            this.EditedPriceStatusService = EditedPriceStatusService;
            this.RequestStateService = RequestStateService;
            this.AppUserService = AppUserService;
            this.DirectSalesOrderService = DirectSalesOrderService;
            this.CurrentContext = CurrentContext;
        }

        [Route(DirectSalesOrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            DirectSalesOrderFilter = DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
            return count;
        }

        [Route(DirectSalesOrderRoute.List), HttpPost]
        public async Task<ActionResult<List<DirectSalesOrder_DirectSalesOrderDTO>>> List([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
            DirectSalesOrderFilter = DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            List<DirectSalesOrder_DirectSalesOrderDTO> DirectSalesOrder_DirectSalesOrderDTOs = DirectSalesOrders
                .Select(c => new DirectSalesOrder_DirectSalesOrderDTO(c)).ToList();
            return DirectSalesOrder_DirectSalesOrderDTOs;
        }

        [Route(DirectSalesOrderRoute.Get), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Get([FromBody]DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = await DirectSalesOrderService.Get(DirectSalesOrder_DirectSalesOrderDTO.Id);
            return new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
        }

        [Route(DirectSalesOrderRoute.Create), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Create([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Create(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Update), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Update([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Update(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }

        [Route(DirectSalesOrderRoute.Delete), HttpPost]
        public async Task<ActionResult<DirectSalesOrder_DirectSalesOrderDTO>> Delete([FromBody] DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(DirectSalesOrder_DirectSalesOrderDTO.Id))
                return Forbid();

            DirectSalesOrder DirectSalesOrder = ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO);
            DirectSalesOrder = await DirectSalesOrderService.Delete(DirectSalesOrder);
            DirectSalesOrder_DirectSalesOrderDTO = new DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder);
            if (DirectSalesOrder.IsValidated)
                return DirectSalesOrder_DirectSalesOrderDTO;
            else
                return BadRequest(DirectSalesOrder_DirectSalesOrderDTO);
        }
        
        [Route(DirectSalesOrderRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter = DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            DirectSalesOrderFilter.Id = new IdFilter { In = Ids };
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.Id;
            DirectSalesOrderFilter.Skip = 0;
            DirectSalesOrderFilter.Take = int.MaxValue;

            List<DirectSalesOrder> DirectSalesOrders = await DirectSalesOrderService.List(DirectSalesOrderFilter);
            DirectSalesOrders = await DirectSalesOrderService.BulkDelete(DirectSalesOrders);
            return true;
        }
        
        [Route(DirectSalesOrderRoute.Import), HttpPost]
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
            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = EditedPriceStatusSelect.ALL
            };
            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            RequestStateFilter RequestStateFilter = new RequestStateFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = RequestStateSelect.ALL
            };
            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            AppUserFilter SaleEmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> SaleEmployees = await AppUserService.List(SaleEmployeeFilter);
            List<DirectSalesOrder> DirectSalesOrders = new List<DirectSalesOrder>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(DirectSalesOrders);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int BuyerStoreIdColumn = 2 + StartColumn;
                int StorePhoneColumn = 3 + StartColumn;
                int StoreAddressColumn = 4 + StartColumn;
                int StoreDeliveryAddressColumn = 5 + StartColumn;
                int TaxCodeColumn = 6 + StartColumn;
                int SaleEmployeeIdColumn = 7 + StartColumn;
                int OrderDateColumn = 8 + StartColumn;
                int DeliveryDateColumn = 9 + StartColumn;
                int EditedPriceStatusIdColumn = 10 + StartColumn;
                int NoteColumn = 11 + StartColumn;
                int SubTotalColumn = 12 + StartColumn;
                int GeneralDiscountPercentageColumn = 13 + StartColumn;
                int GeneralDiscountAmountColumn = 14 + StartColumn;
                int TotalTaxAmountColumn = 15 + StartColumn;
                int TotalColumn = 16 + StartColumn;
                int RequestStateIdColumn = 17 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string BuyerStoreIdValue = worksheet.Cells[i + StartRow, BuyerStoreIdColumn].Value?.ToString();
                    string StorePhoneValue = worksheet.Cells[i + StartRow, StorePhoneColumn].Value?.ToString();
                    string StoreAddressValue = worksheet.Cells[i + StartRow, StoreAddressColumn].Value?.ToString();
                    string StoreDeliveryAddressValue = worksheet.Cells[i + StartRow, StoreDeliveryAddressColumn].Value?.ToString();
                    string TaxCodeValue = worksheet.Cells[i + StartRow, TaxCodeColumn].Value?.ToString();
                    string SaleEmployeeIdValue = worksheet.Cells[i + StartRow, SaleEmployeeIdColumn].Value?.ToString();
                    string OrderDateValue = worksheet.Cells[i + StartRow, OrderDateColumn].Value?.ToString();
                    string DeliveryDateValue = worksheet.Cells[i + StartRow, DeliveryDateColumn].Value?.ToString();
                    string EditedPriceStatusIdValue = worksheet.Cells[i + StartRow, EditedPriceStatusIdColumn].Value?.ToString();
                    string NoteValue = worksheet.Cells[i + StartRow, NoteColumn].Value?.ToString();
                    string SubTotalValue = worksheet.Cells[i + StartRow, SubTotalColumn].Value?.ToString();
                    string GeneralDiscountPercentageValue = worksheet.Cells[i + StartRow, GeneralDiscountPercentageColumn].Value?.ToString();
                    string GeneralDiscountAmountValue = worksheet.Cells[i + StartRow, GeneralDiscountAmountColumn].Value?.ToString();
                    string TotalTaxAmountValue = worksheet.Cells[i + StartRow, TotalTaxAmountColumn].Value?.ToString();
                    string TotalValue = worksheet.Cells[i + StartRow, TotalColumn].Value?.ToString();
                    string RequestStateIdValue = worksheet.Cells[i + StartRow, RequestStateIdColumn].Value?.ToString();
                    
                    DirectSalesOrder DirectSalesOrder = new DirectSalesOrder();
                    DirectSalesOrder.Code = CodeValue;
                    DirectSalesOrder.StorePhone = StorePhoneValue;
                    DirectSalesOrder.StoreAddress = StoreAddressValue;
                    DirectSalesOrder.StoreDeliveryAddress = StoreDeliveryAddressValue;
                    DirectSalesOrder.TaxCode = TaxCodeValue;
                    DirectSalesOrder.OrderDate = DateTime.TryParse(OrderDateValue, out DateTime OrderDate) ? OrderDate : DateTime.Now;
                    DirectSalesOrder.DeliveryDate = DateTime.TryParse(DeliveryDateValue, out DateTime DeliveryDate) ? DeliveryDate : DateTime.Now;
                    DirectSalesOrder.Note = NoteValue;
                    DirectSalesOrder.SubTotal = long.TryParse(SubTotalValue, out long SubTotal) ? SubTotal : 0;
                    DirectSalesOrder.GeneralDiscountPercentage = decimal.TryParse(GeneralDiscountPercentageValue, out decimal GeneralDiscountPercentage) ? GeneralDiscountPercentage : 0;
                    DirectSalesOrder.GeneralDiscountAmount = long.TryParse(GeneralDiscountAmountValue, out long GeneralDiscountAmount) ? GeneralDiscountAmount : 0;
                    DirectSalesOrder.TotalTaxAmount = long.TryParse(TotalTaxAmountValue, out long TotalTaxAmount) ? TotalTaxAmount : 0;
                    DirectSalesOrder.Total = long.TryParse(TotalValue, out long Total) ? Total : 0;
                    Store BuyerStore = BuyerStores.Where(x => x.Id.ToString() == BuyerStoreIdValue).FirstOrDefault();
                    DirectSalesOrder.BuyerStoreId = BuyerStore == null ? 0 : BuyerStore.Id;
                    DirectSalesOrder.BuyerStore = BuyerStore;
                    EditedPriceStatus EditedPriceStatus = EditedPriceStatuses.Where(x => x.Id.ToString() == EditedPriceStatusIdValue).FirstOrDefault();
                    DirectSalesOrder.EditedPriceStatusId = EditedPriceStatus == null ? 0 : EditedPriceStatus.Id;
                    DirectSalesOrder.EditedPriceStatus = EditedPriceStatus;
                    RequestState RequestState = RequestStates.Where(x => x.Id.ToString() == RequestStateIdValue).FirstOrDefault();
                    DirectSalesOrder.RequestStateId = RequestState == null ? 0 : RequestState.Id;
                    DirectSalesOrder.RequestState = RequestState;
                    AppUser SaleEmployee = SaleEmployees.Where(x => x.Id.ToString() == SaleEmployeeIdValue).FirstOrDefault();
                    DirectSalesOrder.SaleEmployeeId = SaleEmployee == null ? 0 : SaleEmployee.Id;
                    DirectSalesOrder.SaleEmployee = SaleEmployee;
                    
                    DirectSalesOrders.Add(DirectSalesOrder);
                }
            }
            DirectSalesOrders = await DirectSalesOrderService.Import(DirectSalesOrders);
            if (DirectSalesOrders.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < DirectSalesOrders.Count; i++)
                {
                    DirectSalesOrder DirectSalesOrder = DirectSalesOrders[i];
                    if (!DirectSalesOrder.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.Id)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.Id)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.Code)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.Code)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.BuyerStoreId)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.BuyerStoreId)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.StorePhone)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.StorePhone)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.StoreAddress)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.StoreAddress)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.StoreDeliveryAddress)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.StoreDeliveryAddress)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.TaxCode)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.TaxCode)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.SaleEmployeeId)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.SaleEmployeeId)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.OrderDate)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.OrderDate)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.DeliveryDate)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.DeliveryDate)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.EditedPriceStatusId)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.EditedPriceStatusId)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.Note)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.Note)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.SubTotal)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.SubTotal)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.GeneralDiscountPercentage)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.GeneralDiscountPercentage)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.GeneralDiscountAmount)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.GeneralDiscountAmount)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.TotalTaxAmount)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.TotalTaxAmount)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.Total)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.Total)];
                        if (DirectSalesOrder.Errors.ContainsKey(nameof(DirectSalesOrder.RequestStateId)))
                            Error += DirectSalesOrder.Errors[nameof(DirectSalesOrder.RequestStateId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(DirectSalesOrderRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrder
                var DirectSalesOrderFilter = ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO);
                DirectSalesOrderFilter.Skip = 0;
                DirectSalesOrderFilter.Take = int.MaxValue;
                DirectSalesOrderFilter = DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
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
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region EditedPriceStatus
                var EditedPriceStatusFilter = new EditedPriceStatusFilter();
                EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
                EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
                EditedPriceStatusFilter.OrderType = OrderType.ASC;
                EditedPriceStatusFilter.Skip = 0;
                EditedPriceStatusFilter.Take = int.MaxValue;
                List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);

                var EditedPriceStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EditedPriceStatusData = new List<object[]>();
                for (int i = 0; i < EditedPriceStatuses.Count; i++)
                {
                    var EditedPriceStatus = EditedPriceStatuses[i];
                    EditedPriceStatusData.Add(new Object[]
                    {
                        EditedPriceStatus.Id,
                        EditedPriceStatus.Code,
                        EditedPriceStatus.Name,
                    });
                }
                excel.GenerateWorksheet("EditedPriceStatus", EditedPriceStatusHeaders, EditedPriceStatusData);
                #endregion
                #region RequestState
                var RequestStateFilter = new RequestStateFilter();
                RequestStateFilter.Selects = RequestStateSelect.ALL;
                RequestStateFilter.OrderBy = RequestStateOrder.Id;
                RequestStateFilter.OrderType = OrderType.ASC;
                RequestStateFilter.Skip = 0;
                RequestStateFilter.Take = int.MaxValue;
                List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);

                var RequestStateHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> RequestStateData = new List<object[]>();
                for (int i = 0; i < RequestStates.Count; i++)
                {
                    var RequestState = RequestStates[i];
                    RequestStateData.Add(new Object[]
                    {
                        RequestState.Id,
                        RequestState.Code,
                        RequestState.Name,
                    });
                }
                excel.GenerateWorksheet("RequestState", RequestStateHeaders, RequestStateData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrder.xlsx");
        }

        [Route(DirectSalesOrderRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region DirectSalesOrder
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
                excel.GenerateWorksheet("DirectSalesOrder", DirectSalesOrderHeaders, DirectSalesOrderData);
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
                        Store.StatusId,
                        Store.WorkflowDefinitionId,
                        Store.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("Store", StoreHeaders, StoreData);
                #endregion
                #region EditedPriceStatus
                var EditedPriceStatusFilter = new EditedPriceStatusFilter();
                EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
                EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
                EditedPriceStatusFilter.OrderType = OrderType.ASC;
                EditedPriceStatusFilter.Skip = 0;
                EditedPriceStatusFilter.Take = int.MaxValue;
                List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);

                var EditedPriceStatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> EditedPriceStatusData = new List<object[]>();
                for (int i = 0; i < EditedPriceStatuses.Count; i++)
                {
                    var EditedPriceStatus = EditedPriceStatuses[i];
                    EditedPriceStatusData.Add(new Object[]
                    {
                        EditedPriceStatus.Id,
                        EditedPriceStatus.Code,
                        EditedPriceStatus.Name,
                    });
                }
                excel.GenerateWorksheet("EditedPriceStatus", EditedPriceStatusHeaders, EditedPriceStatusData);
                #endregion
                #region RequestState
                var RequestStateFilter = new RequestStateFilter();
                RequestStateFilter.Selects = RequestStateSelect.ALL;
                RequestStateFilter.OrderBy = RequestStateOrder.Id;
                RequestStateFilter.OrderType = OrderType.ASC;
                RequestStateFilter.Skip = 0;
                RequestStateFilter.Take = int.MaxValue;
                List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);

                var RequestStateHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> RequestStateData = new List<object[]>();
                for (int i = 0; i < RequestStates.Count; i++)
                {
                    var RequestState = RequestStates[i];
                    RequestStateData.Add(new Object[]
                    {
                        RequestState.Id,
                        RequestState.Code,
                        RequestState.Name,
                    });
                }
                excel.GenerateWorksheet("RequestState", RequestStateHeaders, RequestStateData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "DirectSalesOrder.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter = DirectSalesOrderService.ToFilter(DirectSalesOrderFilter);
            if (Id == 0)
            {

            }
            else
            {
                DirectSalesOrderFilter.Id = new IdFilter { Equal = Id };
                int count = await DirectSalesOrderService.Count(DirectSalesOrderFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private DirectSalesOrder ConvertDTOToEntity(DirectSalesOrder_DirectSalesOrderDTO DirectSalesOrder_DirectSalesOrderDTO)
        {
            DirectSalesOrder DirectSalesOrder = new DirectSalesOrder();
            DirectSalesOrder.Id = DirectSalesOrder_DirectSalesOrderDTO.Id;
            DirectSalesOrder.Code = DirectSalesOrder_DirectSalesOrderDTO.Code;
            DirectSalesOrder.BuyerStoreId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStoreId;
            DirectSalesOrder.StorePhone = DirectSalesOrder_DirectSalesOrderDTO.StorePhone;
            DirectSalesOrder.StoreAddress = DirectSalesOrder_DirectSalesOrderDTO.StoreAddress;
            DirectSalesOrder.StoreDeliveryAddress = DirectSalesOrder_DirectSalesOrderDTO.StoreDeliveryAddress;
            DirectSalesOrder.TaxCode = DirectSalesOrder_DirectSalesOrderDTO.TaxCode;
            DirectSalesOrder.SaleEmployeeId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployeeId;
            DirectSalesOrder.OrderDate = DirectSalesOrder_DirectSalesOrderDTO.OrderDate;
            DirectSalesOrder.DeliveryDate = DirectSalesOrder_DirectSalesOrderDTO.DeliveryDate;
            DirectSalesOrder.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatusId;
            DirectSalesOrder.Note = DirectSalesOrder_DirectSalesOrderDTO.Note;
            DirectSalesOrder.SubTotal = DirectSalesOrder_DirectSalesOrderDTO.SubTotal;
            DirectSalesOrder.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountPercentage;
            DirectSalesOrder.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderDTO.GeneralDiscountAmount;
            DirectSalesOrder.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderDTO.TotalTaxAmount;
            DirectSalesOrder.Total = DirectSalesOrder_DirectSalesOrderDTO.Total;
            DirectSalesOrder.RequestStateId = DirectSalesOrder_DirectSalesOrderDTO.RequestStateId;
            DirectSalesOrder.BuyerStore = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore == null ? null : new Store
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Name,
                ParentStoreId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ParentStoreId,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OrganizationId,
                StoreTypeId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreTypeId,
                StoreGroupingId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StoreGroupingId,
                ResellerId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ResellerId,
                Telephone = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Telephone,
                ProvinceId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.ProvinceId,
                DistrictId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DistrictId,
                WardId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.WardId,
                Address = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Address,
                DeliveryAddress = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryAddress,
                Latitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Latitude,
                Longitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.Longitude,
                DeliveryLatitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryLatitude,
                DeliveryLongitude = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.DeliveryLongitude,
                OwnerName = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerName,
                OwnerPhone = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerPhone,
                OwnerEmail = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.OwnerEmail,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.StatusId,
                WorkflowDefinitionId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.WorkflowDefinitionId,
                RequestStateId = DirectSalesOrder_DirectSalesOrderDTO.BuyerStore.RequestStateId,
            };
            DirectSalesOrder.EditedPriceStatus = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus == null ? null : new EditedPriceStatus
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.EditedPriceStatus.Name,
            };
            DirectSalesOrder.RequestState = DirectSalesOrder_DirectSalesOrderDTO.RequestState == null ? null : new RequestState
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.RequestState.Id,
                Code = DirectSalesOrder_DirectSalesOrderDTO.RequestState.Code,
                Name = DirectSalesOrder_DirectSalesOrderDTO.RequestState.Name,
            };
            DirectSalesOrder.SaleEmployee = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Id,
                Username = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Username,
                Password = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Password,
                DisplayName = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.DisplayName,
                Address = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Address,
                Email = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Email,
                Phone = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Phone,
                Position = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Position,
                Department = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Department,
                OrganizationId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.OrganizationId,
                SexId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.SexId,
                StatusId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.StatusId,
                Avatar = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Avatar,
                Birthday = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.Birthday,
                ProvinceId = DirectSalesOrder_DirectSalesOrderDTO.SaleEmployee.ProvinceId,
            };
            DirectSalesOrder.BaseLanguage = CurrentContext.Language;
            return DirectSalesOrder;
        }

        private DirectSalesOrderFilter ConvertFilterDTOToFilterEntity(DirectSalesOrder_DirectSalesOrderFilterDTO DirectSalesOrder_DirectSalesOrderFilterDTO)
        {
            DirectSalesOrderFilter DirectSalesOrderFilter = new DirectSalesOrderFilter();
            DirectSalesOrderFilter.Selects = DirectSalesOrderSelect.ALL;
            DirectSalesOrderFilter.Skip = DirectSalesOrder_DirectSalesOrderFilterDTO.Skip;
            DirectSalesOrderFilter.Take = DirectSalesOrder_DirectSalesOrderFilterDTO.Take;
            DirectSalesOrderFilter.OrderBy = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderBy;
            DirectSalesOrderFilter.OrderType = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderType;

            DirectSalesOrderFilter.Id = DirectSalesOrder_DirectSalesOrderFilterDTO.Id;
            DirectSalesOrderFilter.Code = DirectSalesOrder_DirectSalesOrderFilterDTO.Code;
            DirectSalesOrderFilter.BuyerStoreId = DirectSalesOrder_DirectSalesOrderFilterDTO.BuyerStoreId;
            DirectSalesOrderFilter.StorePhone = DirectSalesOrder_DirectSalesOrderFilterDTO.StorePhone;
            DirectSalesOrderFilter.StoreAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreAddress;
            DirectSalesOrderFilter.StoreDeliveryAddress = DirectSalesOrder_DirectSalesOrderFilterDTO.StoreDeliveryAddress;
            DirectSalesOrderFilter.TaxCode = DirectSalesOrder_DirectSalesOrderFilterDTO.TaxCode;
            DirectSalesOrderFilter.SaleEmployeeId = DirectSalesOrder_DirectSalesOrderFilterDTO.SaleEmployeeId;
            DirectSalesOrderFilter.OrderDate = DirectSalesOrder_DirectSalesOrderFilterDTO.OrderDate;
            DirectSalesOrderFilter.DeliveryDate = DirectSalesOrder_DirectSalesOrderFilterDTO.DeliveryDate;
            DirectSalesOrderFilter.EditedPriceStatusId = DirectSalesOrder_DirectSalesOrderFilterDTO.EditedPriceStatusId;
            DirectSalesOrderFilter.Note = DirectSalesOrder_DirectSalesOrderFilterDTO.Note;
            DirectSalesOrderFilter.SubTotal = DirectSalesOrder_DirectSalesOrderFilterDTO.SubTotal;
            DirectSalesOrderFilter.GeneralDiscountPercentage = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountPercentage;
            DirectSalesOrderFilter.GeneralDiscountAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.GeneralDiscountAmount;
            DirectSalesOrderFilter.TotalTaxAmount = DirectSalesOrder_DirectSalesOrderFilterDTO.TotalTaxAmount;
            DirectSalesOrderFilter.Total = DirectSalesOrder_DirectSalesOrderFilterDTO.Total;
            DirectSalesOrderFilter.RequestStateId = DirectSalesOrder_DirectSalesOrderFilterDTO.RequestStateId;
            return DirectSalesOrderFilter;
        }

        [Route(DirectSalesOrderRoute.FilterListStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> FilterListStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = DirectSalesOrder_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
            return DirectSalesOrder_StoreDTOs;
        }
        [Route(DirectSalesOrderRoute.FilterListEditedPriceStatus), HttpPost]
        public async Task<List<DirectSalesOrder_EditedPriceStatusDTO>> FilterListEditedPriceStatus([FromBody] DirectSalesOrder_EditedPriceStatusFilterDTO DirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = DirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = DirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = DirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<DirectSalesOrder_EditedPriceStatusDTO> DirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new DirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return DirectSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(DirectSalesOrderRoute.FilterListRequestState), HttpPost]
        public async Task<List<DirectSalesOrder_RequestStateDTO>> FilterListRequestState([FromBody] DirectSalesOrder_RequestStateFilterDTO DirectSalesOrder_RequestStateFilterDTO)
        {
            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = DirectSalesOrder_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = DirectSalesOrder_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = DirectSalesOrder_RequestStateFilterDTO.Name;

            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            List<DirectSalesOrder_RequestStateDTO> DirectSalesOrder_RequestStateDTOs = RequestStates
                .Select(x => new DirectSalesOrder_RequestStateDTO(x)).ToList();
            return DirectSalesOrder_RequestStateDTOs;
        }
        [Route(DirectSalesOrderRoute.FilterListAppUser), HttpPost]
        public async Task<List<DirectSalesOrder_AppUserDTO>> FilterListAppUser([FromBody] DirectSalesOrder_AppUserFilterDTO DirectSalesOrder_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = DirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = DirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = DirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.Position = DirectSalesOrder_AppUserFilterDTO.Position;
            AppUserFilter.Department = DirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = DirectSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = DirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = DirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DirectSalesOrder_AppUserDTO> DirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new DirectSalesOrder_AppUserDTO(x)).ToList();
            return DirectSalesOrder_AppUserDTOs;
        }

        [Route(DirectSalesOrderRoute.SingleListStore), HttpPost]
        public async Task<List<DirectSalesOrder_StoreDTO>> SingleListStore([FromBody] DirectSalesOrder_StoreFilterDTO DirectSalesOrder_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = DirectSalesOrder_StoreFilterDTO.Id;
            StoreFilter.Code = DirectSalesOrder_StoreFilterDTO.Code;
            StoreFilter.Name = DirectSalesOrder_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = DirectSalesOrder_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = DirectSalesOrder_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = DirectSalesOrder_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = DirectSalesOrder_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = DirectSalesOrder_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = DirectSalesOrder_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = DirectSalesOrder_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = DirectSalesOrder_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = DirectSalesOrder_StoreFilterDTO.WardId;
            StoreFilter.Address = DirectSalesOrder_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = DirectSalesOrder_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = DirectSalesOrder_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = DirectSalesOrder_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = DirectSalesOrder_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = DirectSalesOrder_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = DirectSalesOrder_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = DirectSalesOrder_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = DirectSalesOrder_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = DirectSalesOrder_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<DirectSalesOrder_StoreDTO> DirectSalesOrder_StoreDTOs = Stores
                .Select(x => new DirectSalesOrder_StoreDTO(x)).ToList();
            return DirectSalesOrder_StoreDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListEditedPriceStatus), HttpPost]
        public async Task<List<DirectSalesOrder_EditedPriceStatusDTO>> SingleListEditedPriceStatus([FromBody] DirectSalesOrder_EditedPriceStatusFilterDTO DirectSalesOrder_EditedPriceStatusFilterDTO)
        {
            EditedPriceStatusFilter EditedPriceStatusFilter = new EditedPriceStatusFilter();
            EditedPriceStatusFilter.Skip = 0;
            EditedPriceStatusFilter.Take = 20;
            EditedPriceStatusFilter.OrderBy = EditedPriceStatusOrder.Id;
            EditedPriceStatusFilter.OrderType = OrderType.ASC;
            EditedPriceStatusFilter.Selects = EditedPriceStatusSelect.ALL;
            EditedPriceStatusFilter.Id = DirectSalesOrder_EditedPriceStatusFilterDTO.Id;
            EditedPriceStatusFilter.Code = DirectSalesOrder_EditedPriceStatusFilterDTO.Code;
            EditedPriceStatusFilter.Name = DirectSalesOrder_EditedPriceStatusFilterDTO.Name;

            List<EditedPriceStatus> EditedPriceStatuses = await EditedPriceStatusService.List(EditedPriceStatusFilter);
            List<DirectSalesOrder_EditedPriceStatusDTO> DirectSalesOrder_EditedPriceStatusDTOs = EditedPriceStatuses
                .Select(x => new DirectSalesOrder_EditedPriceStatusDTO(x)).ToList();
            return DirectSalesOrder_EditedPriceStatusDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListRequestState), HttpPost]
        public async Task<List<DirectSalesOrder_RequestStateDTO>> SingleListRequestState([FromBody] DirectSalesOrder_RequestStateFilterDTO DirectSalesOrder_RequestStateFilterDTO)
        {
            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = DirectSalesOrder_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = DirectSalesOrder_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = DirectSalesOrder_RequestStateFilterDTO.Name;

            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            List<DirectSalesOrder_RequestStateDTO> DirectSalesOrder_RequestStateDTOs = RequestStates
                .Select(x => new DirectSalesOrder_RequestStateDTO(x)).ToList();
            return DirectSalesOrder_RequestStateDTOs;
        }
        [Route(DirectSalesOrderRoute.SingleListAppUser), HttpPost]
        public async Task<List<DirectSalesOrder_AppUserDTO>> SingleListAppUser([FromBody] DirectSalesOrder_AppUserFilterDTO DirectSalesOrder_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = DirectSalesOrder_AppUserFilterDTO.Id;
            AppUserFilter.Username = DirectSalesOrder_AppUserFilterDTO.Username;
            AppUserFilter.Password = DirectSalesOrder_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = DirectSalesOrder_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = DirectSalesOrder_AppUserFilterDTO.Address;
            AppUserFilter.Email = DirectSalesOrder_AppUserFilterDTO.Email;
            AppUserFilter.Phone = DirectSalesOrder_AppUserFilterDTO.Phone;
            AppUserFilter.Position = DirectSalesOrder_AppUserFilterDTO.Position;
            AppUserFilter.Department = DirectSalesOrder_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = DirectSalesOrder_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = DirectSalesOrder_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = DirectSalesOrder_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = DirectSalesOrder_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = DirectSalesOrder_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<DirectSalesOrder_AppUserDTO> DirectSalesOrder_AppUserDTOs = AppUsers
                .Select(x => new DirectSalesOrder_AppUserDTO(x)).ToList();
            return DirectSalesOrder_AppUserDTOs;
        }

    }
}

