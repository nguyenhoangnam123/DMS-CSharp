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
using DMS.Services.MERouteChangeRequestContent;
using DMS.Services.MERouteChangeRequest;
using DMS.Services.MStore;

namespace DMS.Rpc.e_route_change_request_content
{
    public class ERouteChangeRequestContentRoute : Root
    {
        public const string Master = Module + "/e-route-change-request-content/e-route-change-request-content-master";
        public const string Detail = Module + "/e-route-change-request-content/e-route-change-request-content-detail";
        private const string Default = Rpc + Module + "/e-route-change-request-content";
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
        
        
        public const string FilterListERouteChangeRequest = Default + "/filter-list-e-route-change-request";
        
        public const string FilterListStore = Default + "/filter-list-store";
        

        
        public const string SingleListERouteChangeRequest = Default + "/single-list-e-route-change-request";
        
        public const string SingleListStore = Default + "/single-list-store";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ERouteChangeRequestContentFilter.Id), FieldType.ID },
            { nameof(ERouteChangeRequestContentFilter.ERouteChangeRequestId), FieldType.ID },
            { nameof(ERouteChangeRequestContentFilter.StoreId), FieldType.ID },
            { nameof(ERouteChangeRequestContentFilter.OrderNumber), FieldType.LONG },
        };
    }

    public class ERouteChangeRequestContentController : RpcController
    {
        private IERouteChangeRequestService ERouteChangeRequestService;
        private IStoreService StoreService;
        private IERouteChangeRequestContentService ERouteChangeRequestContentService;
        private ICurrentContext CurrentContext;
        public ERouteChangeRequestContentController(
            IERouteChangeRequestService ERouteChangeRequestService,
            IStoreService StoreService,
            IERouteChangeRequestContentService ERouteChangeRequestContentService,
            ICurrentContext CurrentContext
        )
        {
            this.ERouteChangeRequestService = ERouteChangeRequestService;
            this.StoreService = StoreService;
            this.ERouteChangeRequestContentService = ERouteChangeRequestContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ERouteChangeRequestContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = ConvertFilterDTOToFilterEntity(ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO);
            ERouteChangeRequestContentFilter = ERouteChangeRequestContentService.ToFilter(ERouteChangeRequestContentFilter);
            int count = await ERouteChangeRequestContentService.Count(ERouteChangeRequestContentFilter);
            return count;
        }

        [Route(ERouteChangeRequestContentRoute.List), HttpPost]
        public async Task<ActionResult<List<ERouteChangeRequestContent_ERouteChangeRequestContentDTO>>> List([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = ConvertFilterDTOToFilterEntity(ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO);
            ERouteChangeRequestContentFilter = ERouteChangeRequestContentService.ToFilter(ERouteChangeRequestContentFilter);
            List<ERouteChangeRequestContent> ERouteChangeRequestContents = await ERouteChangeRequestContentService.List(ERouteChangeRequestContentFilter);
            List<ERouteChangeRequestContent_ERouteChangeRequestContentDTO> ERouteChangeRequestContent_ERouteChangeRequestContentDTOs = ERouteChangeRequestContents
                .Select(c => new ERouteChangeRequestContent_ERouteChangeRequestContentDTO(c)).ToList();
            return ERouteChangeRequestContent_ERouteChangeRequestContentDTOs;
        }

        [Route(ERouteChangeRequestContentRoute.Get), HttpPost]
        public async Task<ActionResult<ERouteChangeRequestContent_ERouteChangeRequestContentDTO>> Get([FromBody]ERouteChangeRequestContent_ERouteChangeRequestContentDTO ERouteChangeRequestContent_ERouteChangeRequestContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id))
                return Forbid();

            ERouteChangeRequestContent ERouteChangeRequestContent = await ERouteChangeRequestContentService.Get(ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id);
            return new ERouteChangeRequestContent_ERouteChangeRequestContentDTO(ERouteChangeRequestContent);
        }

        [Route(ERouteChangeRequestContentRoute.Create), HttpPost]
        public async Task<ActionResult<ERouteChangeRequestContent_ERouteChangeRequestContentDTO>> Create([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentDTO ERouteChangeRequestContent_ERouteChangeRequestContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id))
                return Forbid();

            ERouteChangeRequestContent ERouteChangeRequestContent = ConvertDTOToEntity(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
            ERouteChangeRequestContent = await ERouteChangeRequestContentService.Create(ERouteChangeRequestContent);
            ERouteChangeRequestContent_ERouteChangeRequestContentDTO = new ERouteChangeRequestContent_ERouteChangeRequestContentDTO(ERouteChangeRequestContent);
            if (ERouteChangeRequestContent.IsValidated)
                return ERouteChangeRequestContent_ERouteChangeRequestContentDTO;
            else
                return BadRequest(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
        }

        [Route(ERouteChangeRequestContentRoute.Update), HttpPost]
        public async Task<ActionResult<ERouteChangeRequestContent_ERouteChangeRequestContentDTO>> Update([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentDTO ERouteChangeRequestContent_ERouteChangeRequestContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id))
                return Forbid();

            ERouteChangeRequestContent ERouteChangeRequestContent = ConvertDTOToEntity(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
            ERouteChangeRequestContent = await ERouteChangeRequestContentService.Update(ERouteChangeRequestContent);
            ERouteChangeRequestContent_ERouteChangeRequestContentDTO = new ERouteChangeRequestContent_ERouteChangeRequestContentDTO(ERouteChangeRequestContent);
            if (ERouteChangeRequestContent.IsValidated)
                return ERouteChangeRequestContent_ERouteChangeRequestContentDTO;
            else
                return BadRequest(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
        }

        [Route(ERouteChangeRequestContentRoute.Delete), HttpPost]
        public async Task<ActionResult<ERouteChangeRequestContent_ERouteChangeRequestContentDTO>> Delete([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentDTO ERouteChangeRequestContent_ERouteChangeRequestContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id))
                return Forbid();

            ERouteChangeRequestContent ERouteChangeRequestContent = ConvertDTOToEntity(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
            ERouteChangeRequestContent = await ERouteChangeRequestContentService.Delete(ERouteChangeRequestContent);
            ERouteChangeRequestContent_ERouteChangeRequestContentDTO = new ERouteChangeRequestContent_ERouteChangeRequestContentDTO(ERouteChangeRequestContent);
            if (ERouteChangeRequestContent.IsValidated)
                return ERouteChangeRequestContent_ERouteChangeRequestContentDTO;
            else
                return BadRequest(ERouteChangeRequestContent_ERouteChangeRequestContentDTO);
        }
        
        [Route(ERouteChangeRequestContentRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = new ERouteChangeRequestContentFilter();
            ERouteChangeRequestContentFilter = ERouteChangeRequestContentService.ToFilter(ERouteChangeRequestContentFilter);
            ERouteChangeRequestContentFilter.Id = new IdFilter { In = Ids };
            ERouteChangeRequestContentFilter.Selects = ERouteChangeRequestContentSelect.Id;
            ERouteChangeRequestContentFilter.Skip = 0;
            ERouteChangeRequestContentFilter.Take = int.MaxValue;

            List<ERouteChangeRequestContent> ERouteChangeRequestContents = await ERouteChangeRequestContentService.List(ERouteChangeRequestContentFilter);
            ERouteChangeRequestContents = await ERouteChangeRequestContentService.BulkDelete(ERouteChangeRequestContents);
            return true;
        }
        
        [Route(ERouteChangeRequestContentRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ERouteChangeRequestFilter ERouteChangeRequestFilter = new ERouteChangeRequestFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ERouteChangeRequestSelect.ALL
            };
            List<ERouteChangeRequest> ERouteChangeRequests = await ERouteChangeRequestService.List(ERouteChangeRequestFilter);
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.ALL
            };
            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERouteChangeRequestContent> ERouteChangeRequestContents = new List<ERouteChangeRequestContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ERouteChangeRequestContents);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int ERouteChangeRequestIdColumn = 1 + StartColumn;
                int StoreIdColumn = 2 + StartColumn;
                int OrderNumberColumn = 3 + StartColumn;
                int MondayColumn = 4 + StartColumn;
                int TuesdayColumn = 5 + StartColumn;
                int WednesdayColumn = 6 + StartColumn;
                int ThursdayColumn = 7 + StartColumn;
                int FridayColumn = 8 + StartColumn;
                int SaturdayColumn = 9 + StartColumn;
                int SundayColumn = 10 + StartColumn;
                int Week1Column = 11 + StartColumn;
                int Week2Column = 12 + StartColumn;
                int Week3Column = 13 + StartColumn;
                int Week4Column = 14 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string ERouteChangeRequestIdValue = worksheet.Cells[i + StartRow, ERouteChangeRequestIdColumn].Value?.ToString();
                    string StoreIdValue = worksheet.Cells[i + StartRow, StoreIdColumn].Value?.ToString();
                    string OrderNumberValue = worksheet.Cells[i + StartRow, OrderNumberColumn].Value?.ToString();
                    string MondayValue = worksheet.Cells[i + StartRow, MondayColumn].Value?.ToString();
                    string TuesdayValue = worksheet.Cells[i + StartRow, TuesdayColumn].Value?.ToString();
                    string WednesdayValue = worksheet.Cells[i + StartRow, WednesdayColumn].Value?.ToString();
                    string ThursdayValue = worksheet.Cells[i + StartRow, ThursdayColumn].Value?.ToString();
                    string FridayValue = worksheet.Cells[i + StartRow, FridayColumn].Value?.ToString();
                    string SaturdayValue = worksheet.Cells[i + StartRow, SaturdayColumn].Value?.ToString();
                    string SundayValue = worksheet.Cells[i + StartRow, SundayColumn].Value?.ToString();
                    string Week1Value = worksheet.Cells[i + StartRow, Week1Column].Value?.ToString();
                    string Week2Value = worksheet.Cells[i + StartRow, Week2Column].Value?.ToString();
                    string Week3Value = worksheet.Cells[i + StartRow, Week3Column].Value?.ToString();
                    string Week4Value = worksheet.Cells[i + StartRow, Week4Column].Value?.ToString();
                    
                    ERouteChangeRequestContent ERouteChangeRequestContent = new ERouteChangeRequestContent();
                    ERouteChangeRequestContent.OrderNumber = long.TryParse(OrderNumberValue, out long OrderNumber) ? OrderNumber : 0;
                    ERouteChangeRequest ERouteChangeRequest = ERouteChangeRequests.Where(x => x.Id.ToString() == ERouteChangeRequestIdValue).FirstOrDefault();
                    ERouteChangeRequestContent.ERouteChangeRequestId = ERouteChangeRequest == null ? 0 : ERouteChangeRequest.Id;
                    ERouteChangeRequestContent.ERouteChangeRequest = ERouteChangeRequest;
                    Store Store = Stores.Where(x => x.Id.ToString() == StoreIdValue).FirstOrDefault();
                    ERouteChangeRequestContent.StoreId = Store == null ? 0 : Store.Id;
                    ERouteChangeRequestContent.Store = Store;
                    
                    ERouteChangeRequestContents.Add(ERouteChangeRequestContent);
                }
            }
            ERouteChangeRequestContents = await ERouteChangeRequestContentService.Import(ERouteChangeRequestContents);
            if (ERouteChangeRequestContents.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ERouteChangeRequestContents.Count; i++)
                {
                    ERouteChangeRequestContent ERouteChangeRequestContent = ERouteChangeRequestContents[i];
                    if (!ERouteChangeRequestContent.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Id)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Id)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.ERouteChangeRequestId)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.ERouteChangeRequestId)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.StoreId)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.StoreId)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.OrderNumber)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.OrderNumber)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Monday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Monday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Tuesday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Tuesday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Wednesday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Wednesday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Thursday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Thursday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Friday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Friday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Saturday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Saturday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Sunday)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Sunday)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Week1)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Week1)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Week2)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Week2)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Week3)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Week3)];
                        if (ERouteChangeRequestContent.Errors.ContainsKey(nameof(ERouteChangeRequestContent.Week4)))
                            Error += ERouteChangeRequestContent.Errors[nameof(ERouteChangeRequestContent.Week4)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ERouteChangeRequestContentRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ERouteChangeRequestContent
                var ERouteChangeRequestContentFilter = ConvertFilterDTOToFilterEntity(ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO);
                ERouteChangeRequestContentFilter.Skip = 0;
                ERouteChangeRequestContentFilter.Take = int.MaxValue;
                ERouteChangeRequestContentFilter = ERouteChangeRequestContentService.ToFilter(ERouteChangeRequestContentFilter);
                List<ERouteChangeRequestContent> ERouteChangeRequestContents = await ERouteChangeRequestContentService.List(ERouteChangeRequestContentFilter);

                var ERouteChangeRequestContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ERouteChangeRequestId",
                        "StoreId",
                        "OrderNumber",
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                        "Saturday",
                        "Sunday",
                        "Week1",
                        "Week2",
                        "Week3",
                        "Week4",
                    }
                };
                List<object[]> ERouteChangeRequestContentData = new List<object[]>();
                for (int i = 0; i < ERouteChangeRequestContents.Count; i++)
                {
                    var ERouteChangeRequestContent = ERouteChangeRequestContents[i];
                    ERouteChangeRequestContentData.Add(new Object[]
                    {
                        ERouteChangeRequestContent.Id,
                        ERouteChangeRequestContent.ERouteChangeRequestId,
                        ERouteChangeRequestContent.StoreId,
                        ERouteChangeRequestContent.OrderNumber,
                        ERouteChangeRequestContent.Monday,
                        ERouteChangeRequestContent.Tuesday,
                        ERouteChangeRequestContent.Wednesday,
                        ERouteChangeRequestContent.Thursday,
                        ERouteChangeRequestContent.Friday,
                        ERouteChangeRequestContent.Saturday,
                        ERouteChangeRequestContent.Sunday,
                        ERouteChangeRequestContent.Week1,
                        ERouteChangeRequestContent.Week2,
                        ERouteChangeRequestContent.Week3,
                        ERouteChangeRequestContent.Week4,
                    });
                }
                excel.GenerateWorksheet("ERouteChangeRequestContent", ERouteChangeRequestContentHeaders, ERouteChangeRequestContentData);
                #endregion
                
                #region ERouteChangeRequest
                var ERouteChangeRequestFilter = new ERouteChangeRequestFilter();
                ERouteChangeRequestFilter.Selects = ERouteChangeRequestSelect.ALL;
                ERouteChangeRequestFilter.OrderBy = ERouteChangeRequestOrder.Id;
                ERouteChangeRequestFilter.OrderType = OrderType.ASC;
                ERouteChangeRequestFilter.Skip = 0;
                ERouteChangeRequestFilter.Take = int.MaxValue;
                List<ERouteChangeRequest> ERouteChangeRequests = await ERouteChangeRequestService.List(ERouteChangeRequestFilter);

                var ERouteChangeRequestHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ERouteId",
                        "CreatorId",
                        "RequestStateId",
                    }
                };
                List<object[]> ERouteChangeRequestData = new List<object[]>();
                for (int i = 0; i < ERouteChangeRequests.Count; i++)
                {
                    var ERouteChangeRequest = ERouteChangeRequests[i];
                    ERouteChangeRequestData.Add(new Object[]
                    {
                        ERouteChangeRequest.Id,
                        ERouteChangeRequest.ERouteId,
                        ERouteChangeRequest.CreatorId,
                        ERouteChangeRequest.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("ERouteChangeRequest", ERouteChangeRequestHeaders, ERouteChangeRequestData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ERouteChangeRequestContent.xlsx");
        }

        [Route(ERouteChangeRequestContentRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ERouteChangeRequestContent
                var ERouteChangeRequestContentHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ERouteChangeRequestId",
                        "StoreId",
                        "OrderNumber",
                        "Monday",
                        "Tuesday",
                        "Wednesday",
                        "Thursday",
                        "Friday",
                        "Saturday",
                        "Sunday",
                        "Week1",
                        "Week2",
                        "Week3",
                        "Week4",
                    }
                };
                List<object[]> ERouteChangeRequestContentData = new List<object[]>();
                excel.GenerateWorksheet("ERouteChangeRequestContent", ERouteChangeRequestContentHeaders, ERouteChangeRequestContentData);
                #endregion
                
                #region ERouteChangeRequest
                var ERouteChangeRequestFilter = new ERouteChangeRequestFilter();
                ERouteChangeRequestFilter.Selects = ERouteChangeRequestSelect.ALL;
                ERouteChangeRequestFilter.OrderBy = ERouteChangeRequestOrder.Id;
                ERouteChangeRequestFilter.OrderType = OrderType.ASC;
                ERouteChangeRequestFilter.Skip = 0;
                ERouteChangeRequestFilter.Take = int.MaxValue;
                List<ERouteChangeRequest> ERouteChangeRequests = await ERouteChangeRequestService.List(ERouteChangeRequestFilter);

                var ERouteChangeRequestHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "ERouteId",
                        "CreatorId",
                        "RequestStateId",
                    }
                };
                List<object[]> ERouteChangeRequestData = new List<object[]>();
                for (int i = 0; i < ERouteChangeRequests.Count; i++)
                {
                    var ERouteChangeRequest = ERouteChangeRequests[i];
                    ERouteChangeRequestData.Add(new Object[]
                    {
                        ERouteChangeRequest.Id,
                        ERouteChangeRequest.ERouteId,
                        ERouteChangeRequest.CreatorId,
                        ERouteChangeRequest.RequestStateId,
                    });
                }
                excel.GenerateWorksheet("ERouteChangeRequest", ERouteChangeRequestHeaders, ERouteChangeRequestData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ERouteChangeRequestContent.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = new ERouteChangeRequestContentFilter();
            ERouteChangeRequestContentFilter = ERouteChangeRequestContentService.ToFilter(ERouteChangeRequestContentFilter);
            if (Id == 0)
            {

            }
            else
            {
                ERouteChangeRequestContentFilter.Id = new IdFilter { Equal = Id };
                int count = await ERouteChangeRequestContentService.Count(ERouteChangeRequestContentFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ERouteChangeRequestContent ConvertDTOToEntity(ERouteChangeRequestContent_ERouteChangeRequestContentDTO ERouteChangeRequestContent_ERouteChangeRequestContentDTO)
        {
            ERouteChangeRequestContent ERouteChangeRequestContent = new ERouteChangeRequestContent();
            ERouteChangeRequestContent.Id = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Id;
            ERouteChangeRequestContent.ERouteChangeRequestId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequestId;
            ERouteChangeRequestContent.StoreId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.StoreId;
            ERouteChangeRequestContent.OrderNumber = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.OrderNumber;
            ERouteChangeRequestContent.Monday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Monday;
            ERouteChangeRequestContent.Tuesday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Tuesday;
            ERouteChangeRequestContent.Wednesday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Wednesday;
            ERouteChangeRequestContent.Thursday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Thursday;
            ERouteChangeRequestContent.Friday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Friday;
            ERouteChangeRequestContent.Saturday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Saturday;
            ERouteChangeRequestContent.Sunday = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Sunday;
            ERouteChangeRequestContent.Week1 = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Week1;
            ERouteChangeRequestContent.Week2 = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Week2;
            ERouteChangeRequestContent.Week3 = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Week3;
            ERouteChangeRequestContent.Week4 = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Week4;
            ERouteChangeRequestContent.ERouteChangeRequest = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequest == null ? null : new ERouteChangeRequest
            {
                Id = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequest.Id,
                ERouteId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequest.ERouteId,
                CreatorId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequest.CreatorId,
                RequestStateId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.ERouteChangeRequest.RequestStateId,
            };
            ERouteChangeRequestContent.Store = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store == null ? null : new Store
            {
                Id = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Id,
                Code = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Code,
                Name = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Name,
                ParentStoreId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.ParentStoreId,
                OrganizationId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.OrganizationId,
                StoreTypeId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.StoreTypeId,
                StoreGroupingId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.StoreGroupingId,
                ResellerId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.ResellerId,
                Telephone = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Telephone,
                ProvinceId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.ProvinceId,
                DistrictId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.DistrictId,
                WardId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.WardId,
                Address = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Address,
                DeliveryAddress = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.DeliveryAddress,
                Latitude = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Latitude,
                Longitude = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.Longitude,
                DeliveryLatitude = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.DeliveryLatitude,
                DeliveryLongitude = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.DeliveryLongitude,
                OwnerName = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.OwnerName,
                OwnerPhone = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.OwnerPhone,
                OwnerEmail = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.OwnerEmail,
                StatusId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.StatusId,
                WorkflowDefinitionId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.WorkflowDefinitionId,
                RequestStateId = ERouteChangeRequestContent_ERouteChangeRequestContentDTO.Store.RequestStateId,
            };
            ERouteChangeRequestContent.BaseLanguage = CurrentContext.Language;
            return ERouteChangeRequestContent;
        }

        private ERouteChangeRequestContentFilter ConvertFilterDTOToFilterEntity(ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO)
        {
            ERouteChangeRequestContentFilter ERouteChangeRequestContentFilter = new ERouteChangeRequestContentFilter();
            ERouteChangeRequestContentFilter.Selects = ERouteChangeRequestContentSelect.ALL;
            ERouteChangeRequestContentFilter.Skip = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.Skip;
            ERouteChangeRequestContentFilter.Take = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.Take;
            ERouteChangeRequestContentFilter.OrderBy = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.OrderBy;
            ERouteChangeRequestContentFilter.OrderType = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.OrderType;

            ERouteChangeRequestContentFilter.Id = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.Id;
            ERouteChangeRequestContentFilter.ERouteChangeRequestId = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.ERouteChangeRequestId;
            ERouteChangeRequestContentFilter.StoreId = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.StoreId;
            ERouteChangeRequestContentFilter.OrderNumber = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.OrderNumber;
            ERouteChangeRequestContentFilter.CreatedAt = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.CreatedAt;
            ERouteChangeRequestContentFilter.UpdatedAt = ERouteChangeRequestContent_ERouteChangeRequestContentFilterDTO.UpdatedAt;
            return ERouteChangeRequestContentFilter;
        }

        [Route(ERouteChangeRequestContentRoute.FilterListERouteChangeRequest), HttpPost]
        public async Task<List<ERouteChangeRequestContent_ERouteChangeRequestDTO>> FilterListERouteChangeRequest([FromBody] ERouteChangeRequestContent_ERouteChangeRequestFilterDTO ERouteChangeRequestContent_ERouteChangeRequestFilterDTO)
        {
            ERouteChangeRequestFilter ERouteChangeRequestFilter = new ERouteChangeRequestFilter();
            ERouteChangeRequestFilter.Skip = 0;
            ERouteChangeRequestFilter.Take = 20;
            ERouteChangeRequestFilter.OrderBy = ERouteChangeRequestOrder.Id;
            ERouteChangeRequestFilter.OrderType = OrderType.ASC;
            ERouteChangeRequestFilter.Selects = ERouteChangeRequestSelect.ALL;
            ERouteChangeRequestFilter.Id = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.Id;
            ERouteChangeRequestFilter.ERouteId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.ERouteId;
            ERouteChangeRequestFilter.CreatorId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.CreatorId;
            ERouteChangeRequestFilter.RequestStateId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.RequestStateId;

            List<ERouteChangeRequest> ERouteChangeRequests = await ERouteChangeRequestService.List(ERouteChangeRequestFilter);
            List<ERouteChangeRequestContent_ERouteChangeRequestDTO> ERouteChangeRequestContent_ERouteChangeRequestDTOs = ERouteChangeRequests
                .Select(x => new ERouteChangeRequestContent_ERouteChangeRequestDTO(x)).ToList();
            return ERouteChangeRequestContent_ERouteChangeRequestDTOs;
        }
        [Route(ERouteChangeRequestContentRoute.FilterListStore), HttpPost]
        public async Task<List<ERouteChangeRequestContent_StoreDTO>> FilterListStore([FromBody] ERouteChangeRequestContent_StoreFilterDTO ERouteChangeRequestContent_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERouteChangeRequestContent_StoreFilterDTO.Id;
            StoreFilter.Code = ERouteChangeRequestContent_StoreFilterDTO.Code;
            StoreFilter.Name = ERouteChangeRequestContent_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERouteChangeRequestContent_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERouteChangeRequestContent_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERouteChangeRequestContent_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERouteChangeRequestContent_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERouteChangeRequestContent_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERouteChangeRequestContent_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERouteChangeRequestContent_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERouteChangeRequestContent_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERouteChangeRequestContent_StoreFilterDTO.WardId;
            StoreFilter.Address = ERouteChangeRequestContent_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERouteChangeRequestContent_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERouteChangeRequestContent_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERouteChangeRequestContent_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERouteChangeRequestContent_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERouteChangeRequestContent_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERouteChangeRequestContent_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERouteChangeRequestContent_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERouteChangeRequestContent_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERouteChangeRequestContent_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERouteChangeRequestContent_StoreDTO> ERouteChangeRequestContent_StoreDTOs = Stores
                .Select(x => new ERouteChangeRequestContent_StoreDTO(x)).ToList();
            return ERouteChangeRequestContent_StoreDTOs;
        }

        [Route(ERouteChangeRequestContentRoute.SingleListERouteChangeRequest), HttpPost]
        public async Task<List<ERouteChangeRequestContent_ERouteChangeRequestDTO>> SingleListERouteChangeRequest([FromBody] ERouteChangeRequestContent_ERouteChangeRequestFilterDTO ERouteChangeRequestContent_ERouteChangeRequestFilterDTO)
        {
            ERouteChangeRequestFilter ERouteChangeRequestFilter = new ERouteChangeRequestFilter();
            ERouteChangeRequestFilter.Skip = 0;
            ERouteChangeRequestFilter.Take = 20;
            ERouteChangeRequestFilter.OrderBy = ERouteChangeRequestOrder.Id;
            ERouteChangeRequestFilter.OrderType = OrderType.ASC;
            ERouteChangeRequestFilter.Selects = ERouteChangeRequestSelect.ALL;
            ERouteChangeRequestFilter.Id = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.Id;
            ERouteChangeRequestFilter.ERouteId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.ERouteId;
            ERouteChangeRequestFilter.CreatorId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.CreatorId;
            ERouteChangeRequestFilter.RequestStateId = ERouteChangeRequestContent_ERouteChangeRequestFilterDTO.RequestStateId;

            List<ERouteChangeRequest> ERouteChangeRequests = await ERouteChangeRequestService.List(ERouteChangeRequestFilter);
            List<ERouteChangeRequestContent_ERouteChangeRequestDTO> ERouteChangeRequestContent_ERouteChangeRequestDTOs = ERouteChangeRequests
                .Select(x => new ERouteChangeRequestContent_ERouteChangeRequestDTO(x)).ToList();
            return ERouteChangeRequestContent_ERouteChangeRequestDTOs;
        }
        [Route(ERouteChangeRequestContentRoute.SingleListStore), HttpPost]
        public async Task<List<ERouteChangeRequestContent_StoreDTO>> SingleListStore([FromBody] ERouteChangeRequestContent_StoreFilterDTO ERouteChangeRequestContent_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERouteChangeRequestContent_StoreFilterDTO.Id;
            StoreFilter.Code = ERouteChangeRequestContent_StoreFilterDTO.Code;
            StoreFilter.Name = ERouteChangeRequestContent_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERouteChangeRequestContent_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERouteChangeRequestContent_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERouteChangeRequestContent_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERouteChangeRequestContent_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERouteChangeRequestContent_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERouteChangeRequestContent_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERouteChangeRequestContent_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERouteChangeRequestContent_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERouteChangeRequestContent_StoreFilterDTO.WardId;
            StoreFilter.Address = ERouteChangeRequestContent_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERouteChangeRequestContent_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERouteChangeRequestContent_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERouteChangeRequestContent_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERouteChangeRequestContent_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERouteChangeRequestContent_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERouteChangeRequestContent_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERouteChangeRequestContent_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERouteChangeRequestContent_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERouteChangeRequestContent_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERouteChangeRequestContent_StoreDTO> ERouteChangeRequestContent_StoreDTOs = Stores
                .Select(x => new ERouteChangeRequestContent_StoreDTO(x)).ToList();
            return ERouteChangeRequestContent_StoreDTOs;
        }

    }
}

