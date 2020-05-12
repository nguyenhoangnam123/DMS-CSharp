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
using DMS.Services.MERoute;
using DMS.Services.MAppUser;
using DMS.Services.MRequestState;
using DMS.Services.MStatus;
using DMS.Enums;
using DMS.Services.MStore;
using DMS.Services.MERouteType;
using DMS.Services.MOrganization;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;

namespace DMS.Rpc.e_route
{
    public class ERouteRoute : Root
    {
        public const string Master = Module + "/e-route/e-route-master";
        public const string Detail = Module + "/e-route/e-route-detail";
        private const string Default = Rpc + Module + "/e-route";
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
        public const string FilterListStore = Default + "/filter-list-store";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListERouteType = Default + "/single-list-eroute-type";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListRequestState = Default + "/single-list-request-state";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ERouteFilter.Code), FieldType.STRING },
            { nameof(ERouteFilter.Name), FieldType.STRING },
            { nameof(ERouteFilter.SaleEmployeeId), FieldType.ID },
            { nameof(ERouteFilter.StartDate), FieldType.DATE },
            { nameof(ERouteFilter.EndDate), FieldType.DATE },
            { nameof(ERouteFilter.RequestStateId), FieldType.ID },
            { nameof(ERouteFilter.StatusId), FieldType.ID },
            { nameof(ERouteFilter.CreatorId), FieldType.ID },
        };
    }

    public class ERouteController : RpcController
    {
        private IAppUserService AppUserService;
        private IERouteTypeService ERouteTypeService;
        private IOrganizationService OrganizationService;
        private IRequestStateService RequestStateService;
        private IStatusService StatusService;
        private IERouteService ERouteService;
        private IStoreService StoreService;
        private IStoreGroupingService StoreGroupingService;
        private IStoreTypeService StoreTypeService;
        private ICurrentContext CurrentContext;
        public ERouteController(
            IAppUserService AppUserService,
            IERouteTypeService ERouteTypeService,
            IOrganizationService OrganizationService,
            IRequestStateService RequestStateService,
            IStatusService StatusService,
            IERouteService ERouteService,
            IStoreService StoreService,
            IStoreGroupingService StoreGroupingService,
            IStoreTypeService StoreTypeService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.ERouteTypeService = ERouteTypeService;
            this.OrganizationService = OrganizationService;
            this.RequestStateService = RequestStateService;
            this.StatusService = StatusService;
            this.ERouteService = ERouteService;
            this.StoreService = StoreService;
            this.StoreGroupingService = StoreGroupingService;
            this.StoreTypeService = StoreTypeService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ERouteRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO);
            ERouteFilter = ERouteService.ToFilter(ERouteFilter);
            int count = await ERouteService.Count(ERouteFilter);
            return count;
        }

        [Route(ERouteRoute.List), HttpPost]
        public async Task<ActionResult<List<ERoute_ERouteDTO>>> List([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO);
            ERouteFilter = ERouteService.ToFilter(ERouteFilter);
            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            List<ERoute_ERouteDTO> ERoute_ERouteDTOs = ERoutes
                .Select(c => new ERoute_ERouteDTO(c)).ToList();
            return ERoute_ERouteDTOs;
        }

        [Route(ERouteRoute.Get), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Get([FromBody]ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = await ERouteService.Get(ERoute_ERouteDTO.Id);
            return new ERoute_ERouteDTO(ERoute);
        }

        [Route(ERouteRoute.Create), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Create([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Create(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }

        [Route(ERouteRoute.Update), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Update([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Update(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }

        [Route(ERouteRoute.Delete), HttpPost]
        public async Task<ActionResult<ERoute_ERouteDTO>> Delete([FromBody] ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(ERoute_ERouteDTO.Id))
                return Forbid();

            ERoute ERoute = ConvertDTOToEntity(ERoute_ERouteDTO);
            ERoute = await ERouteService.Delete(ERoute);
            ERoute_ERouteDTO = new ERoute_ERouteDTO(ERoute);
            if (ERoute.IsValidated)
                return ERoute_ERouteDTO;
            else
                return BadRequest(ERoute_ERouteDTO);
        }
        
        [Route(ERouteRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter = ERouteService.ToFilter(ERouteFilter);
            ERouteFilter.Id = new IdFilter { In = Ids };
            ERouteFilter.Selects = ERouteSelect.Id | ERouteSelect.RequestState;
            ERouteFilter.Skip = 0;
            ERouteFilter.Take = int.MaxValue;

            List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);
            ERoutes = await ERouteService.BulkDelete(ERoutes);
            if (ERoutes.Any(x => !x.IsValidated))
                return BadRequest(ERoutes.Where(x => !x.IsValidated));
            return true;
        }
        
        [Route(ERouteRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUserFilter CreatorFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Creators = await AppUserService.List(CreatorFilter);
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
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ERoute> ERoutes = new List<ERoute>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(ERoutes);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int SaleEmployeeIdColumn = 3 + StartColumn;
                int StartDateColumn = 4 + StartColumn;
                int EndDateColumn = 5 + StartColumn;
                int RequestStateIdColumn = 6 + StartColumn;
                int StatusIdColumn = 7 + StartColumn;
                int CreatorIdColumn = 11 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string SaleEmployeeIdValue = worksheet.Cells[i + StartRow, SaleEmployeeIdColumn].Value?.ToString();
                    string StartDateValue = worksheet.Cells[i + StartRow, StartDateColumn].Value?.ToString();
                    string EndDateValue = worksheet.Cells[i + StartRow, EndDateColumn].Value?.ToString();
                    string RequestStateIdValue = worksheet.Cells[i + StartRow, RequestStateIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    
                    ERoute ERoute = new ERoute();
                    ERoute.Code = CodeValue;
                    ERoute.Name = NameValue;
                    ERoute.StartDate = DateTime.TryParse(StartDateValue, out DateTime StartDate) ? StartDate : DateTime.Now;
                    ERoute.EndDate = DateTime.TryParse(EndDateValue, out DateTime EndDate) ? EndDate : DateTime.Now;
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    ERoute.CreatorId = Creator == null ? 0 : Creator.Id;
                    ERoute.Creator = Creator;
                    RequestState RequestState = RequestStates.Where(x => x.Id.ToString() == RequestStateIdValue).FirstOrDefault();
                    ERoute.RequestStateId = RequestState == null ? 0 : RequestState.Id;
                    ERoute.RequestState = RequestState;
                    AppUser SaleEmployee = SaleEmployees.Where(x => x.Id.ToString() == SaleEmployeeIdValue).FirstOrDefault();
                    ERoute.SaleEmployeeId = SaleEmployee == null ? 0 : SaleEmployee.Id;
                    ERoute.SaleEmployee = SaleEmployee;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    ERoute.StatusId = Status == null ? 0 : Status.Id;
                    ERoute.Status = Status;
                    
                    ERoutes.Add(ERoute);
                }
            }
            ERoutes = await ERouteService.Import(ERoutes);
            if (ERoutes.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < ERoutes.Count; i++)
                {
                    ERoute ERoute = ERoutes[i];
                    if (!ERoute.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.Id)))
                            Error += ERoute.Errors[nameof(ERoute.Id)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.Code)))
                            Error += ERoute.Errors[nameof(ERoute.Code)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.Name)))
                            Error += ERoute.Errors[nameof(ERoute.Name)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.SaleEmployeeId)))
                            Error += ERoute.Errors[nameof(ERoute.SaleEmployeeId)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.StartDate)))
                            Error += ERoute.Errors[nameof(ERoute.StartDate)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.EndDate)))
                            Error += ERoute.Errors[nameof(ERoute.EndDate)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.RequestStateId)))
                            Error += ERoute.Errors[nameof(ERoute.RequestStateId)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.StatusId)))
                            Error += ERoute.Errors[nameof(ERoute.StatusId)];
                        if (ERoute.Errors.ContainsKey(nameof(ERoute.CreatorId)))
                            Error += ERoute.Errors[nameof(ERoute.CreatorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(ERouteRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ERoute
                var ERouteFilter = ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO);
                ERouteFilter.Skip = 0;
                ERouteFilter.Take = int.MaxValue;
                ERouteFilter = ERouteService.ToFilter(ERouteFilter);
                List<ERoute> ERoutes = await ERouteService.List(ERouteFilter);

                var ERouteHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "SaleEmployeeId",
                        "StartDate",
                        "EndDate",
                        "RequestStateId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> ERouteData = new List<object[]>();
                for (int i = 0; i < ERoutes.Count; i++)
                {
                    var ERoute = ERoutes[i];
                    ERouteData.Add(new Object[]
                    {
                        ERoute.Id,
                        ERoute.Code,
                        ERoute.Name,
                        ERoute.SaleEmployeeId,
                        ERoute.StartDate,
                        ERoute.EndDate,
                        ERoute.RequestStateId,
                        ERoute.StatusId,
                        ERoute.CreatorId,
                    });
                }
                excel.GenerateWorksheet("ERoute", ERouteHeaders, ERouteData);
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
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ERoute.xlsx");
        }

        [Route(ERouteRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region ERoute
                var ERouteHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "SaleEmployeeId",
                        "StartDate",
                        "EndDate",
                        "RequestStateId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> ERouteData = new List<object[]>();
                excel.GenerateWorksheet("ERoute", ERouteHeaders, ERouteData);
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
                #region Status
                var StatusFilter = new StatusFilter();
                StatusFilter.Selects = StatusSelect.ALL;
                StatusFilter.OrderBy = StatusOrder.Id;
                StatusFilter.OrderType = OrderType.ASC;
                StatusFilter.Skip = 0;
                StatusFilter.Take = int.MaxValue;
                List<Status> Statuses = await StatusService.List(StatusFilter);

                var StatusHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> StatusData = new List<object[]>();
                for (int i = 0; i < Statuses.Count; i++)
                {
                    var Status = Statuses[i];
                    StatusData.Add(new Object[]
                    {
                        Status.Id,
                        Status.Code,
                        Status.Name,
                    });
                }
                excel.GenerateWorksheet("Status", StatusHeaders, StatusData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "ERoute.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter = ERouteService.ToFilter(ERouteFilter);
            if (Id == 0)
            {

            }
            else
            {
                ERouteFilter.Id = new IdFilter { Equal = Id };
                int count = await ERouteService.Count(ERouteFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private ERoute ConvertDTOToEntity(ERoute_ERouteDTO ERoute_ERouteDTO)
        {
            ERoute ERoute = new ERoute();
            ERoute.Id = ERoute_ERouteDTO.Id;
            ERoute.Code = ERoute_ERouteDTO.Code;
            ERoute.Name = ERoute_ERouteDTO.Name;
            ERoute.SaleEmployeeId = ERoute_ERouteDTO.SaleEmployeeId;
            ERoute.StartDate = ERoute_ERouteDTO.StartDate;
            ERoute.EndDate = ERoute_ERouteDTO.EndDate;
            ERoute.ERouteTypeId = ERoute_ERouteDTO.ERouteTypeId;
            ERoute.RequestStateId = ERoute_ERouteDTO.RequestStateId;
            ERoute.StatusId = ERoute_ERouteDTO.StatusId;
            ERoute.CreatorId = ERoute_ERouteDTO.CreatorId;
            ERoute.Creator = ERoute_ERouteDTO.Creator == null ? null : new AppUser
            {
                Id = ERoute_ERouteDTO.Creator.Id,
                Username = ERoute_ERouteDTO.Creator.Username,
                Password = ERoute_ERouteDTO.Creator.Password,
                DisplayName = ERoute_ERouteDTO.Creator.DisplayName,
                Address = ERoute_ERouteDTO.Creator.Address,
                Email = ERoute_ERouteDTO.Creator.Email,
                Phone = ERoute_ERouteDTO.Creator.Phone,
                Position = ERoute_ERouteDTO.Creator.Position,
                Department = ERoute_ERouteDTO.Creator.Department,
                OrganizationId = ERoute_ERouteDTO.Creator.OrganizationId,
                SexId = ERoute_ERouteDTO.Creator.SexId,
                StatusId = ERoute_ERouteDTO.Creator.StatusId,
                Avatar = ERoute_ERouteDTO.Creator.Avatar,
                Birthday = ERoute_ERouteDTO.Creator.Birthday,
                ProvinceId = ERoute_ERouteDTO.Creator.ProvinceId,
            };
            ERoute.ERouteType = ERoute_ERouteDTO.ERouteType == null ? null : new ERouteType
            {
                Id = ERoute_ERouteDTO.ERouteType.Id,
                Code = ERoute_ERouteDTO.ERouteType.Code,
                Name = ERoute_ERouteDTO.ERouteType.Name,
            };
            ERoute.RequestState = ERoute_ERouteDTO.RequestState == null ? null : new RequestState
            {
                Id = ERoute_ERouteDTO.RequestState.Id,
                Code = ERoute_ERouteDTO.RequestState.Code,
                Name = ERoute_ERouteDTO.RequestState.Name,
            };
            ERoute.SaleEmployee = ERoute_ERouteDTO.SaleEmployee == null ? null : new AppUser
            {
                Id = ERoute_ERouteDTO.SaleEmployee.Id,
                Username = ERoute_ERouteDTO.SaleEmployee.Username,
                Password = ERoute_ERouteDTO.SaleEmployee.Password,
                DisplayName = ERoute_ERouteDTO.SaleEmployee.DisplayName,
                Address = ERoute_ERouteDTO.SaleEmployee.Address,
                Email = ERoute_ERouteDTO.SaleEmployee.Email,
                Phone = ERoute_ERouteDTO.SaleEmployee.Phone,
                Position = ERoute_ERouteDTO.SaleEmployee.Position,
                Department = ERoute_ERouteDTO.SaleEmployee.Department,
                OrganizationId = ERoute_ERouteDTO.SaleEmployee.OrganizationId,
                SexId = ERoute_ERouteDTO.SaleEmployee.SexId,
                StatusId = ERoute_ERouteDTO.SaleEmployee.StatusId,
                Avatar = ERoute_ERouteDTO.SaleEmployee.Avatar,
                Birthday = ERoute_ERouteDTO.SaleEmployee.Birthday,
                ProvinceId = ERoute_ERouteDTO.SaleEmployee.ProvinceId,
            };
            ERoute.Status = ERoute_ERouteDTO.Status == null ? null : new Status
            {
                Id = ERoute_ERouteDTO.Status.Id,
                Code = ERoute_ERouteDTO.Status.Code,
                Name = ERoute_ERouteDTO.Status.Name,
            };
            ERoute.ERouteContents = ERoute_ERouteDTO.ERouteContents?.Select(x => new ERouteContent
            {
                Id = x.Id,
                ERouteId = x.ERouteId,
                StoreId = x.StoreId,
                OrderNumber = x.OrderNumber,
                Monday = x.Monday,
                Tuesday = x.Tuesday,
                Wednesday = x.Wednesday,
                Thursday = x.Thursday,
                Friday = x.Friday,
                Saturday = x.Saturday,
                Sunday = x.Sunday,
                Week1 = x.Week1,
                Week2 = x.Week2,
                Week3 = x.Week3,
                Week4 = x.Week4,
                Store = new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    Name = x.Store.Name,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    ResellerId = x.Store.ResellerId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    DeliveryAddress = x.Store.DeliveryAddress,
                    Latitude = x.Store.Latitude,
                    Longitude = x.Store.Longitude,
                    DeliveryLatitude = x.Store.DeliveryLatitude,
                    DeliveryLongitude = x.Store.DeliveryLongitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    StatusId = x.Store.StatusId,
                },
            }).ToList();
            ERoute.BaseLanguage = CurrentContext.Language;
            return ERoute;
        }

        private ERouteFilter ConvertFilterDTOToFilterEntity(ERoute_ERouteFilterDTO ERoute_ERouteFilterDTO)
        {
            ERouteFilter ERouteFilter = new ERouteFilter();
            ERouteFilter.Selects = ERouteSelect.ALL;
            ERouteFilter.Skip = ERoute_ERouteFilterDTO.Skip;
            ERouteFilter.Take = ERoute_ERouteFilterDTO.Take;
            ERouteFilter.OrderBy = ERoute_ERouteFilterDTO.OrderBy;
            ERouteFilter.OrderType = ERoute_ERouteFilterDTO.OrderType;

            ERouteFilter.Id = ERoute_ERouteFilterDTO.Id;
            ERouteFilter.Code = ERoute_ERouteFilterDTO.Code;
            ERouteFilter.Name = ERoute_ERouteFilterDTO.Name;
            ERouteFilter.SaleEmployeeId = ERoute_ERouteFilterDTO.SaleEmployeeId;
            ERouteFilter.StartDate = ERoute_ERouteFilterDTO.StartDate;
            ERouteFilter.EndDate = ERoute_ERouteFilterDTO.EndDate;
            ERouteFilter.RequestStateId = ERoute_ERouteFilterDTO.RequestStateId;
            ERouteFilter.StatusId = ERoute_ERouteFilterDTO.StatusId;
            ERouteFilter.CreatorId = ERoute_ERouteFilterDTO.CreatorId;
            ERouteFilter.StoreId = ERoute_ERouteFilterDTO.StoreId;
            ERouteFilter.CreatedAt = ERoute_ERouteFilterDTO.CreatedAt;
            ERouteFilter.UpdatedAt = ERoute_ERouteFilterDTO.UpdatedAt;
            return ERouteFilter;
        }

        [Route(ERouteRoute.FilterListAppUser), HttpPost]
        public async Task<List<ERoute_AppUserDTO>> FilterListAppUser([FromBody] ERoute_AppUserFilterDTO ERoute_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ERoute_AppUserFilterDTO.Id;
            AppUserFilter.Username = ERoute_AppUserFilterDTO.Username;
            AppUserFilter.Password = ERoute_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ERoute_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ERoute_AppUserFilterDTO.Address;
            AppUserFilter.Email = ERoute_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ERoute_AppUserFilterDTO.Phone;
            AppUserFilter.Position = ERoute_AppUserFilterDTO.Position;
            AppUserFilter.Department = ERoute_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ERoute_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = ERoute_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = ERoute_AppUserFilterDTO.StatusId;
            AppUserFilter.Birthday = ERoute_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = ERoute_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ERoute_AppUserDTO> ERoute_AppUserDTOs = AppUsers
                .Select(x => new ERoute_AppUserDTO(x)).ToList();
            return ERoute_AppUserDTOs;
        }
        [Route(ERouteRoute.FilterListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> FilterListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERoute_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERoute_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERoute_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERoute_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;
        }

        [Route(ERouteRoute.SingleListAppUser), HttpPost]
        public async Task<List<ERoute_AppUserDTO>> SingleListAppUser([FromBody] ERoute_AppUserFilterDTO ERoute_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = ERoute_AppUserFilterDTO.Id;
            AppUserFilter.Username = ERoute_AppUserFilterDTO.Username;
            AppUserFilter.Password = ERoute_AppUserFilterDTO.Password;
            AppUserFilter.DisplayName = ERoute_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = ERoute_AppUserFilterDTO.Address;
            AppUserFilter.Email = ERoute_AppUserFilterDTO.Email;
            AppUserFilter.Phone = ERoute_AppUserFilterDTO.Phone;
            AppUserFilter.Position = ERoute_AppUserFilterDTO.Position;
            AppUserFilter.Department = ERoute_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = ERoute_AppUserFilterDTO.OrganizationId;
            AppUserFilter.SexId = ERoute_AppUserFilterDTO.SexId;
            AppUserFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };
            AppUserFilter.Birthday = ERoute_AppUserFilterDTO.Birthday;
            AppUserFilter.ProvinceId = ERoute_AppUserFilterDTO.ProvinceId;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<ERoute_AppUserDTO> ERoute_AppUserDTOs = AppUsers
                .Select(x => new ERoute_AppUserDTO(x)).ToList();
            return ERoute_AppUserDTOs;
        }
        [Route(ERouteRoute.SingleListERouteType), HttpPost]
        public async Task<List<ERoute_ERouteTypeDTO>> SingleListERouteType([FromBody] ERoute_ERouteTypeFilterDTO ERoute_ERouteTypeFilterDTO)
        {
            ERouteTypeFilter ERouteTypeFilter = new ERouteTypeFilter();
            ERouteTypeFilter.Skip = 0;
            ERouteTypeFilter.Take = 20;
            ERouteTypeFilter.OrderBy = ERouteTypeOrder.Id;
            ERouteTypeFilter.OrderType = OrderType.ASC;
            ERouteTypeFilter.Selects = ERouteTypeSelect.ALL;
            ERouteTypeFilter.Id = ERoute_ERouteTypeFilterDTO.Id;
            ERouteTypeFilter.Code = ERoute_ERouteTypeFilterDTO.Code;
            ERouteTypeFilter.Name = ERoute_ERouteTypeFilterDTO.Name;

            List<ERouteType> ERouteTypes = await ERouteTypeService.List(ERouteTypeFilter);
            List<ERoute_ERouteTypeDTO> ERoute_ERouteTypeDTOs = ERouteTypes
                .Select(x => new ERoute_ERouteTypeDTO(x)).ToList();
            return ERoute_ERouteTypeDTOs;
        }
        [Route(ERouteRoute.SingleListOrganization), HttpPost]
        public async Task<List<ERoute_OrganizationDTO>> SingleListOrganization([FromBody] ERoute_OrganizationFilterDTO ERoute_OrganizationFilterDTO)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = 99999;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = ERoute_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = ERoute_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = ERoute_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = ERoute_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = ERoute_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = ERoute_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = ERoute_OrganizationFilterDTO.Phone;
            OrganizationFilter.Address = ERoute_OrganizationFilterDTO.Address;
            OrganizationFilter.Email = ERoute_OrganizationFilterDTO.Email;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<ERoute_OrganizationDTO> ERoute_OrganizationDTOs = Organizations
                .Select(x => new ERoute_OrganizationDTO(x)).ToList();
            return ERoute_OrganizationDTOs;
        }
        [Route(ERouteRoute.SingleListRequestState), HttpPost]
        public async Task<List<ERoute_RequestStateDTO>> SingleListRequestState([FromBody] ERoute_RequestStateFilterDTO ERoute_RequestStateFilterDTO)
        {
            RequestStateFilter RequestStateFilter = new RequestStateFilter();
            RequestStateFilter.Skip = 0;
            RequestStateFilter.Take = 20;
            RequestStateFilter.OrderBy = RequestStateOrder.Id;
            RequestStateFilter.OrderType = OrderType.ASC;
            RequestStateFilter.Selects = RequestStateSelect.ALL;
            RequestStateFilter.Id = ERoute_RequestStateFilterDTO.Id;
            RequestStateFilter.Code = ERoute_RequestStateFilterDTO.Code;
            RequestStateFilter.Name = ERoute_RequestStateFilterDTO.Name;

            List<RequestState> RequestStates = await RequestStateService.List(RequestStateFilter);
            List<ERoute_RequestStateDTO> ERoute_RequestStateDTOs = RequestStates
                .Select(x => new ERoute_RequestStateDTO(x)).ToList();
            return ERoute_RequestStateDTOs;
        }
        [Route(ERouteRoute.SingleListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> SingleListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = 0;
            StoreFilter.Take = 20;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.StoreGroupingId = ERoute_StoreFilterDTO.StoreGroupingId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.DeliveryLatitude = ERoute_StoreFilterDTO.DeliveryLatitude;
            StoreFilter.DeliveryLongitude = ERoute_StoreFilterDTO.DeliveryLongitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;
        }
        [Route(ERouteRoute.SingleListStoreGrouping), HttpPost]
        public async Task<List<ERoute_StoreGroupingDTO>> SingleListStoreGrouping([FromBody] ERoute_StoreGroupingFilterDTO ERoute_StoreGroupingFilterDTO)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter();
            StoreGroupingFilter.Skip = 0;
            StoreGroupingFilter.Take = 99999;
            StoreGroupingFilter.OrderBy = StoreGroupingOrder.Id;
            StoreGroupingFilter.OrderType = OrderType.ASC;
            StoreGroupingFilter.Selects = StoreGroupingSelect.ALL;
            StoreGroupingFilter.Code = ERoute_StoreGroupingFilterDTO.Code;
            StoreGroupingFilter.Name = ERoute_StoreGroupingFilterDTO.Name;
            StoreGroupingFilter.Level = ERoute_StoreGroupingFilterDTO.Level;
            StoreGroupingFilter.Path = ERoute_StoreGroupingFilterDTO.Path;
            StoreGroupingFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            List<StoreGrouping> StoreGroupings = await StoreGroupingService.List(StoreGroupingFilter);
            List<ERoute_StoreGroupingDTO> ERoute_StoreGroupingDTOs = StoreGroupings
                .Select(x => new ERoute_StoreGroupingDTO(x)).ToList();
            return ERoute_StoreGroupingDTOs;
        }
        [Route(ERouteRoute.SingleListStoreType), HttpPost]
        public async Task<List<ERoute_StoreTypeDTO>> SingleListStoreType([FromBody] ERoute_StoreTypeFilterDTO ERoute_StoreTypeFilterDTO)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter();
            StoreTypeFilter.Skip = 0;
            StoreTypeFilter.Take = 20;
            StoreTypeFilter.OrderBy = StoreTypeOrder.Id;
            StoreTypeFilter.OrderType = OrderType.ASC;
            StoreTypeFilter.Selects = StoreTypeSelect.ALL;
            StoreTypeFilter.Id = ERoute_StoreTypeFilterDTO.Id;
            StoreTypeFilter.Code = ERoute_StoreTypeFilterDTO.Code;
            StoreTypeFilter.Name = ERoute_StoreTypeFilterDTO.Name;
            StoreTypeFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<StoreType> StoreTypes = await StoreTypeService.List(StoreTypeFilter);
            List<ERoute_StoreTypeDTO> ERoute_StoreTypeDTOs = StoreTypes
                .Select(x => new ERoute_StoreTypeDTO(x)).ToList();
            return ERoute_StoreTypeDTOs;
        }
        [Route(ERouteRoute.SingleListStatus), HttpPost]
        public async Task<List<ERoute_StatusDTO>> SingleListStatus([FromBody] ERoute_StatusFilterDTO ERoute_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<ERoute_StatusDTO> ERoute_StatusDTOs = Statuses
                .Select(x => new ERoute_StatusDTO(x)).ToList();
            return ERoute_StatusDTOs;
        }

        [Route(ERouteRoute.CountStore), HttpPost]
        public async Task<long> CountStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERoute_StoreFilterDTO.StatusId;
            return await StoreService.Count(StoreFilter);
        }

        [Route(ERouteRoute.ListStore), HttpPost]
        public async Task<List<ERoute_StoreDTO>> ListStore([FromBody] ERoute_StoreFilterDTO ERoute_StoreFilterDTO)
        {
            StoreFilter StoreFilter = new StoreFilter();
            StoreFilter.Skip = ERoute_StoreFilterDTO.Skip;
            StoreFilter.Take = ERoute_StoreFilterDTO.Take;
            StoreFilter.OrderBy = StoreOrder.Id;
            StoreFilter.OrderType = OrderType.ASC;
            StoreFilter.Selects = StoreSelect.ALL;
            StoreFilter.Id = ERoute_StoreFilterDTO.Id;
            StoreFilter.Code = ERoute_StoreFilterDTO.Code;
            StoreFilter.Name = ERoute_StoreFilterDTO.Name;
            StoreFilter.ParentStoreId = ERoute_StoreFilterDTO.ParentStoreId;
            StoreFilter.OrganizationId = ERoute_StoreFilterDTO.OrganizationId;
            StoreFilter.StoreTypeId = ERoute_StoreFilterDTO.StoreTypeId;
            StoreFilter.ResellerId = ERoute_StoreFilterDTO.ResellerId;
            StoreFilter.Telephone = ERoute_StoreFilterDTO.Telephone;
            StoreFilter.ProvinceId = ERoute_StoreFilterDTO.ProvinceId;
            StoreFilter.DistrictId = ERoute_StoreFilterDTO.DistrictId;
            StoreFilter.WardId = ERoute_StoreFilterDTO.WardId;
            StoreFilter.Address = ERoute_StoreFilterDTO.Address;
            StoreFilter.DeliveryAddress = ERoute_StoreFilterDTO.DeliveryAddress;
            StoreFilter.Latitude = ERoute_StoreFilterDTO.Latitude;
            StoreFilter.Longitude = ERoute_StoreFilterDTO.Longitude;
            StoreFilter.OwnerName = ERoute_StoreFilterDTO.OwnerName;
            StoreFilter.OwnerPhone = ERoute_StoreFilterDTO.OwnerPhone;
            StoreFilter.OwnerEmail = ERoute_StoreFilterDTO.OwnerEmail;
            StoreFilter.StatusId = ERoute_StoreFilterDTO.StatusId;

            List<Store> Stores = await StoreService.List(StoreFilter);
            List<ERoute_StoreDTO> ERoute_StoreDTOs = Stores
                .Select(x => new ERoute_StoreDTO(x)).ToList();
            return ERoute_StoreDTOs;
        }
    }
}

