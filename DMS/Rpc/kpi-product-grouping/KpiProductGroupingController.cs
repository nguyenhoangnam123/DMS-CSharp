using DMS.Common;
using DMS.Entities;
using DMS.Helpers;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiProductGrouping;
using DMS.Services.MKpiProductGroupingType;
using DMS.Services.MProduct;
using DMS.Services.MStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_product_grouping
{
    public partial class KpiProductGroupingController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IKpiProductGroupingTypeService KpiProductGroupingTypeService;
        private IStatusService StatusService;
        private IKpiProductGroupingService KpiProductGroupingService;
        private IItemService ItemService;
        private ICurrentContext CurrentContext;
        public KpiProductGroupingController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IKpiProductGroupingTypeService KpiProductGroupingTypeService,
            IStatusService StatusService,
            IKpiProductGroupingService KpiProductGroupingService,
            IItemService ItemService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.KpiProductGroupingTypeService = KpiProductGroupingTypeService;
            this.StatusService = StatusService;
            this.KpiProductGroupingService = KpiProductGroupingService;
            this.ItemService = ItemService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiProductGroupingRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO);
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            int count = await KpiProductGroupingService.Count(KpiProductGroupingFilter);
            return count;
        }

        [Route(KpiProductGroupingRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiProductGrouping_KpiProductGroupingDTO>>> List([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO);
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            List<KpiProductGrouping_KpiProductGroupingDTO> KpiProductGrouping_KpiProductGroupingDTOs = KpiProductGroupings
                .Select(c => new KpiProductGrouping_KpiProductGroupingDTO(c)).ToList();
            return KpiProductGrouping_KpiProductGroupingDTOs;
        }

        [Route(KpiProductGroupingRoute.Get), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Get([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = await KpiProductGroupingService.Get(KpiProductGrouping_KpiProductGroupingDTO.Id);
            return new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
        }

        [Route(KpiProductGroupingRoute.Create), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Create([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Create(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.Update), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Update([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Update(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiProductGrouping_KpiProductGroupingDTO>> Delete([FromBody] KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiProductGrouping_KpiProductGroupingDTO.Id))
                return Forbid();

            KpiProductGrouping KpiProductGrouping = ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO);
            KpiProductGrouping = await KpiProductGroupingService.Delete(KpiProductGrouping);
            KpiProductGrouping_KpiProductGroupingDTO = new KpiProductGrouping_KpiProductGroupingDTO(KpiProductGrouping);
            if (KpiProductGrouping.IsValidated)
                return KpiProductGrouping_KpiProductGroupingDTO;
            else
                return BadRequest(KpiProductGrouping_KpiProductGroupingDTO);
        }

        [Route(KpiProductGroupingRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            KpiProductGroupingFilter.Id = new IdFilter { In = Ids };
            KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.Id;
            KpiProductGroupingFilter.Skip = 0;
            KpiProductGroupingFilter.Take = int.MaxValue;

            List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);
            KpiProductGroupings = await KpiProductGroupingService.BulkDelete(KpiProductGroupings);
            if (KpiProductGroupings.Any(x => !x.IsValidated))
                return BadRequest(KpiProductGroupings.Where(x => !x.IsValidated));
            return true;
        }

        [Route(KpiProductGroupingRoute.Import), HttpPost]
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
            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.ALL
            };
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);
            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL
            };
            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            KpiProductGroupingTypeFilter KpiProductGroupingTypeFilter = new KpiProductGroupingTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiProductGroupingTypeSelect.ALL
            };
            List<KpiProductGroupingType> KpiProductGroupingTypes = await KpiProductGroupingTypeService.List(KpiProductGroupingTypeFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiProductGrouping> KpiProductGroupings = new List<KpiProductGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiProductGroupings);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int OrganizationIdColumn = 1 + StartColumn;
                int KpiYearIdColumn = 2 + StartColumn;
                int KpiPeriodIdColumn = 3 + StartColumn;
                int KpiProductGroupingTypeIdColumn = 4 + StartColumn;
                int StatusIdColumn = 5 + StartColumn;
                int EmployeeIdColumn = 6 + StartColumn;
                int CreatorIdColumn = 7 + StartColumn;
                int RowIdColumn = 11 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string KpiYearIdValue = worksheet.Cells[i + StartRow, KpiYearIdColumn].Value?.ToString();
                    string KpiPeriodIdValue = worksheet.Cells[i + StartRow, KpiPeriodIdColumn].Value?.ToString();
                    string KpiProductGroupingTypeIdValue = worksheet.Cells[i + StartRow, KpiProductGroupingTypeIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string EmployeeIdValue = worksheet.Cells[i + StartRow, EmployeeIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    string RowIdValue = worksheet.Cells[i + StartRow, RowIdColumn].Value?.ToString();

                    KpiProductGrouping KpiProductGrouping = new KpiProductGrouping();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    KpiProductGrouping.CreatorId = Creator == null ? 0 : Creator.Id;
                    KpiProductGrouping.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    KpiProductGrouping.EmployeeId = Employee == null ? 0 : Employee.Id;
                    KpiProductGrouping.Employee = Employee;
                    KpiPeriod KpiPeriod = KpiPeriods.Where(x => x.Id.ToString() == KpiPeriodIdValue).FirstOrDefault();
                    KpiProductGrouping.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                    KpiProductGrouping.KpiPeriod = KpiPeriod;
                    KpiProductGroupingType KpiProductGroupingType = KpiProductGroupingTypes.Where(x => x.Id.ToString() == KpiProductGroupingTypeIdValue).FirstOrDefault();
                    KpiProductGrouping.KpiProductGroupingTypeId = KpiProductGroupingType == null ? 0 : KpiProductGroupingType.Id;
                    KpiProductGrouping.KpiProductGroupingType = KpiProductGroupingType;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    KpiProductGrouping.StatusId = Status == null ? 0 : Status.Id;
                    KpiProductGrouping.Status = Status;

                    KpiProductGroupings.Add(KpiProductGrouping);
                }
            }
            KpiProductGroupings = await KpiProductGroupingService.Import(KpiProductGroupings);
            if (KpiProductGroupings.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiProductGroupings.Count; i++)
                {
                    KpiProductGrouping KpiProductGrouping = KpiProductGroupings[i];
                    if (!KpiProductGrouping.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.Id)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.Id)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.OrganizationId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.OrganizationId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiYearId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiYearId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiPeriodId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiPeriodId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.KpiProductGroupingTypeId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.KpiProductGroupingTypeId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.StatusId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.StatusId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.EmployeeId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.EmployeeId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.CreatorId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.CreatorId)];
                        if (KpiProductGrouping.Errors.ContainsKey(nameof(KpiProductGrouping.RowId)))
                            Error += KpiProductGrouping.Errors[nameof(KpiProductGrouping.RowId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(KpiProductGroupingRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiProductGrouping
                var KpiProductGroupingFilter = ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO);
                KpiProductGroupingFilter.Skip = 0;
                KpiProductGroupingFilter.Take = int.MaxValue;
                KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
                List<KpiProductGrouping> KpiProductGroupings = await KpiProductGroupingService.List(KpiProductGroupingFilter);

                var KpiProductGroupingHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "OrganizationId",
                        "KpiYearId",
                        "KpiPeriodId",
                        "KpiProductGroupingTypeId",
                        "StatusId",
                        "EmployeeId",
                        "CreatorId",
                        "RowId",
                    }
                };
                List<object[]> KpiProductGroupingData = new List<object[]>();
                for (int i = 0; i < KpiProductGroupings.Count; i++)
                {
                    var KpiProductGrouping = KpiProductGroupings[i];
                    KpiProductGroupingData.Add(new Object[]
                    {
                        KpiProductGrouping.Id,
                        KpiProductGrouping.OrganizationId,
                        KpiProductGrouping.KpiYearId,
                        KpiProductGrouping.KpiPeriodId,
                        KpiProductGrouping.KpiProductGroupingTypeId,
                        KpiProductGrouping.StatusId,
                        KpiProductGrouping.EmployeeId,
                        KpiProductGrouping.CreatorId,
                        KpiProductGrouping.RowId,
                    });
                }
                excel.GenerateWorksheet("KpiProductGrouping", KpiProductGroupingHeaders, KpiProductGroupingData);
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
                        "DisplayName",
                        "Address",
                        "Email",
                        "Phone",
                        "SexId",
                        "Birthday",
                        "Avatar",
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "ProvinceId",
                        "Longitude",
                        "Latitude",
                        "StatusId",
                        "GPSUpdatedAt",
                        "RowId",
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
                        AppUser.DisplayName,
                        AppUser.Address,
                        AppUser.Email,
                        AppUser.Phone,
                        AppUser.SexId,
                        AppUser.Birthday,
                        AppUser.Avatar,
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.ProvinceId,
                        AppUser.Longitude,
                        AppUser.Latitude,
                        AppUser.StatusId,
                        AppUser.GPSUpdatedAt,
                        AppUser.RowId,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region KpiPeriod
                var KpiPeriodFilter = new KpiPeriodFilter();
                KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
                KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
                KpiPeriodFilter.OrderType = OrderType.ASC;
                KpiPeriodFilter.Skip = 0;
                KpiPeriodFilter.Take = int.MaxValue;
                List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);

                var KpiPeriodHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiPeriodData = new List<object[]>();
                for (int i = 0; i < KpiPeriods.Count; i++)
                {
                    var KpiPeriod = KpiPeriods[i];
                    KpiPeriodData.Add(new Object[]
                    {
                        KpiPeriod.Id,
                        KpiPeriod.Code,
                        KpiPeriod.Name,
                    });
                }
                excel.GenerateWorksheet("KpiPeriod", KpiPeriodHeaders, KpiPeriodData);
                #endregion
                #region KpiProductGroupingType
                var KpiProductGroupingTypeFilter = new KpiProductGroupingTypeFilter();
                KpiProductGroupingTypeFilter.Selects = KpiProductGroupingTypeSelect.ALL;
                KpiProductGroupingTypeFilter.OrderBy = KpiProductGroupingTypeOrder.Id;
                KpiProductGroupingTypeFilter.OrderType = OrderType.ASC;
                KpiProductGroupingTypeFilter.Skip = 0;
                KpiProductGroupingTypeFilter.Take = int.MaxValue;
                List<KpiProductGroupingType> KpiProductGroupingTypes = await KpiProductGroupingTypeService.List(KpiProductGroupingTypeFilter);

                var KpiProductGroupingTypeHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiProductGroupingTypeData = new List<object[]>();
                for (int i = 0; i < KpiProductGroupingTypes.Count; i++)
                {
                    var KpiProductGroupingType = KpiProductGroupingTypes[i];
                    KpiProductGroupingTypeData.Add(new Object[]
                    {
                        KpiProductGroupingType.Id,
                        KpiProductGroupingType.Code,
                        KpiProductGroupingType.Name,
                    });
                }
                excel.GenerateWorksheet("KpiProductGroupingType", KpiProductGroupingTypeHeaders, KpiProductGroupingTypeData);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiProductGrouping.xlsx");
        }

        [Route(KpiProductGroupingRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate([FromBody] KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            string path = "Templates/KpiProductGrouping_Template.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "KpiProductGrouping.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter = await KpiProductGroupingService.ToFilter(KpiProductGroupingFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiProductGroupingFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiProductGroupingService.Count(KpiProductGroupingFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiProductGrouping ConvertDTOToEntity(KpiProductGrouping_KpiProductGroupingDTO KpiProductGrouping_KpiProductGroupingDTO)
        {
            KpiProductGrouping KpiProductGrouping = new KpiProductGrouping();
            KpiProductGrouping.Id = KpiProductGrouping_KpiProductGroupingDTO.Id;
            KpiProductGrouping.OrganizationId = KpiProductGrouping_KpiProductGroupingDTO.OrganizationId;
            KpiProductGrouping.KpiYearId = KpiProductGrouping_KpiProductGroupingDTO.KpiYearId;
            KpiProductGrouping.KpiPeriodId = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriodId;
            KpiProductGrouping.KpiProductGroupingTypeId = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingTypeId;
            KpiProductGrouping.StatusId = KpiProductGrouping_KpiProductGroupingDTO.StatusId;
            KpiProductGrouping.EmployeeId = KpiProductGrouping_KpiProductGroupingDTO.EmployeeId;
            KpiProductGrouping.CreatorId = KpiProductGrouping_KpiProductGroupingDTO.CreatorId;
            KpiProductGrouping.RowId = KpiProductGrouping_KpiProductGroupingDTO.RowId;
            KpiProductGrouping.Creator = KpiProductGrouping_KpiProductGroupingDTO.Creator == null ? null : new AppUser
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Creator.Id,
                Username = KpiProductGrouping_KpiProductGroupingDTO.Creator.Username,
                DisplayName = KpiProductGrouping_KpiProductGroupingDTO.Creator.DisplayName,
                Address = KpiProductGrouping_KpiProductGroupingDTO.Creator.Address,
                Email = KpiProductGrouping_KpiProductGroupingDTO.Creator.Email,
                Phone = KpiProductGrouping_KpiProductGroupingDTO.Creator.Phone,
            };
            KpiProductGrouping.Employee = KpiProductGrouping_KpiProductGroupingDTO.Employee == null ? null : new AppUser
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Employee.Id,
                Username = KpiProductGrouping_KpiProductGroupingDTO.Employee.Username,
                DisplayName = KpiProductGrouping_KpiProductGroupingDTO.Employee.DisplayName,
                Address = KpiProductGrouping_KpiProductGroupingDTO.Employee.Address,
                Email = KpiProductGrouping_KpiProductGroupingDTO.Employee.Email,
                Phone = KpiProductGrouping_KpiProductGroupingDTO.Employee.Phone,
            };
            KpiProductGrouping.KpiPeriod = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.KpiPeriod.Name,
            };
            KpiProductGrouping.KpiProductGroupingType = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType == null ? null : new KpiProductGroupingType
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.KpiProductGroupingType.Name,
            };
            KpiProductGrouping.Status = KpiProductGrouping_KpiProductGroupingDTO.Status == null ? null : new Status
            {
                Id = KpiProductGrouping_KpiProductGroupingDTO.Status.Id,
                Code = KpiProductGrouping_KpiProductGroupingDTO.Status.Code,
                Name = KpiProductGrouping_KpiProductGroupingDTO.Status.Name,
            };
            KpiProductGrouping.BaseLanguage = CurrentContext.Language;
            return KpiProductGrouping;
        }

        private KpiProductGroupingFilter ConvertFilterDTOToFilterEntity(KpiProductGrouping_KpiProductGroupingFilterDTO KpiProductGrouping_KpiProductGroupingFilterDTO)
        {
            KpiProductGroupingFilter KpiProductGroupingFilter = new KpiProductGroupingFilter();
            KpiProductGroupingFilter.Selects = KpiProductGroupingSelect.ALL;
            KpiProductGroupingFilter.Skip = KpiProductGrouping_KpiProductGroupingFilterDTO.Skip;
            KpiProductGroupingFilter.Take = KpiProductGrouping_KpiProductGroupingFilterDTO.Take;
            KpiProductGroupingFilter.OrderBy = KpiProductGrouping_KpiProductGroupingFilterDTO.OrderBy;
            KpiProductGroupingFilter.OrderType = KpiProductGrouping_KpiProductGroupingFilterDTO.OrderType;

            KpiProductGroupingFilter.Id = KpiProductGrouping_KpiProductGroupingFilterDTO.Id;
            KpiProductGroupingFilter.OrganizationId = KpiProductGrouping_KpiProductGroupingFilterDTO.OrganizationId;
            KpiProductGroupingFilter.KpiYearId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiYearId;
            KpiProductGroupingFilter.KpiPeriodId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiPeriodId;
            KpiProductGroupingFilter.KpiProductGroupingTypeId = KpiProductGrouping_KpiProductGroupingFilterDTO.KpiProductGroupingTypeId;
            KpiProductGroupingFilter.StatusId = KpiProductGrouping_KpiProductGroupingFilterDTO.StatusId;
            KpiProductGroupingFilter.EmployeeId = KpiProductGrouping_KpiProductGroupingFilterDTO.EmployeeId;
            KpiProductGroupingFilter.CreatorId = KpiProductGrouping_KpiProductGroupingFilterDTO.CreatorId;
            KpiProductGroupingFilter.RowId = KpiProductGrouping_KpiProductGroupingFilterDTO.RowId;
            KpiProductGroupingFilter.CreatedAt = KpiProductGrouping_KpiProductGroupingFilterDTO.CreatedAt;
            KpiProductGroupingFilter.UpdatedAt = KpiProductGrouping_KpiProductGroupingFilterDTO.UpdatedAt;
            return KpiProductGroupingFilter;
        }
    }
}
