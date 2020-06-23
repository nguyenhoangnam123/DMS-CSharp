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
using DMS.Services.MKpiGeneral;
using DMS.Services.MAppUser;
using DMS.Services.MKpiYear;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MKpiCriteriaGeneral;
using DMS.Services.MKpiPeriod;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DMS.Rpc.kpi_general
{
    public class KpiGeneralController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiYearService KpiYearService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IKpiCriteriaGeneralService KpiCriteriaGeneralService;
        private IKpiGeneralService KpiGeneralService;
        private IKpiPeriodService KpiPeriodService;
        private ICurrentContext CurrentContext;
        public KpiGeneralController(
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IKpiCriteriaGeneralService KpiCriteriaGeneralService,
            IKpiGeneralService KpiGeneralService,
            IKpiPeriodService KpiPeriodService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiYearService = KpiYearService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.KpiCriteriaGeneralService = KpiCriteriaGeneralService;
            this.KpiGeneralService = KpiGeneralService;
            this.KpiPeriodService = KpiPeriodService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiGeneralRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiGeneralFilter KpiGeneralFilter = ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO);
            KpiGeneralFilter = KpiGeneralService.ToFilter(KpiGeneralFilter);
            int count = await KpiGeneralService.Count(KpiGeneralFilter);
            return count;
        }

        [Route(KpiGeneralRoute.List), HttpPost]
        public async Task<ActionResult<List<KpiGeneral_KpiGeneralDTO>>> List([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiGeneralFilter KpiGeneralFilter = ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO);
            KpiGeneralFilter = KpiGeneralService.ToFilter(KpiGeneralFilter);
            List<KpiGeneral> KpiGenerals = await KpiGeneralService.List(KpiGeneralFilter);
            List<KpiGeneral_KpiGeneralDTO> KpiGeneral_KpiGeneralDTOs = KpiGenerals
                .Select(c => new KpiGeneral_KpiGeneralDTO(c)).ToList();
            return KpiGeneral_KpiGeneralDTOs;
        }

        [Route(KpiGeneralRoute.Get), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Get([FromBody]KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();

            KpiGeneral KpiGeneral = await KpiGeneralService.Get(KpiGeneral_KpiGeneralDTO.Id);
            return new KpiGeneral_KpiGeneralDTO(KpiGeneral);
        }
        private Tuple<GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            switch(date.Month)
            {
                case 1:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 2:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH02;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 3:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH03;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER01;
                    break;
                case 4:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH04;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 5:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 9:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH09;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 10:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH10;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 11:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH11;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
                case 12:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH12;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER04;
                    break;
            }
            return Tuple.Create(monthName, quarterName);
        }
        [Route(KpiGeneralRoute.GetDraft), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> GetDraft()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(new KpiCriteriaGeneralFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaGeneralSelect.ALL,
            });
            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL,
            });
            var KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO();
            KpiGeneral_KpiGeneralDTO.KpiPeriods = KpiPeriods.Select(x => new KpiGeneral_KpiPeriodDTO(x)).ToList();
            KpiGeneral_KpiGeneralDTO.KpiGeneralContents = KpiCriteriaGenerals.Select(x => new KpiGeneral_KpiGeneralContentDTO
            {
                KpiCriteriaGeneralId = x.Id,
                KpiCriteriaGeneral = new KpiGeneral_KpiCriteriaGeneralDTO(x),
                KpiGeneralContentKpiPeriodMappings = KpiPeriods.ToDictionary(x => x.Id, y => 0L),
                Status = new KpiGeneral_StatusDTO
                {
                    Id = Enums.StatusEnum.ACTIVE.Id,
                    Code = Enums.StatusEnum.ACTIVE.Code,
                    Name = Enums.StatusEnum.ACTIVE.Name
                },
            }).ToList();
            (KpiGeneral_KpiGeneralDTO.CurrentMonth, KpiGeneral_KpiGeneralDTO.CurrentQuarter) = ConvertDateTime(StaticParams.DateTimeNow);
            return KpiGeneral_KpiGeneralDTO;
        }
        [Route(KpiGeneralRoute.Create), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Create([FromBody] KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();

            KpiGeneral KpiGeneral = ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO);
            KpiGeneral = await KpiGeneralService.Create(KpiGeneral);
            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            if (KpiGeneral.IsValidated)
                return KpiGeneral_KpiGeneralDTO;
            else
                return BadRequest(KpiGeneral_KpiGeneralDTO);
        }

        [Route(KpiGeneralRoute.Update), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Update([FromBody] KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();

            KpiGeneral KpiGeneral = ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO);
            KpiGeneral = await KpiGeneralService.Update(KpiGeneral);
            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            if (KpiGeneral.IsValidated)
                return KpiGeneral_KpiGeneralDTO;
            else
                return BadRequest(KpiGeneral_KpiGeneralDTO);
        }

        [Route(KpiGeneralRoute.Delete), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Delete([FromBody] KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();

            KpiGeneral KpiGeneral = ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO);
            KpiGeneral = await KpiGeneralService.Delete(KpiGeneral);
            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            if (KpiGeneral.IsValidated)
                return KpiGeneral_KpiGeneralDTO;
            else
                return BadRequest(KpiGeneral_KpiGeneralDTO);
        }

        [Route(KpiGeneralRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter();
            KpiGeneralFilter = KpiGeneralService.ToFilter(KpiGeneralFilter);
            KpiGeneralFilter.Id = new IdFilter { In = Ids };
            KpiGeneralFilter.Selects = KpiGeneralSelect.Id;
            KpiGeneralFilter.Skip = 0;
            KpiGeneralFilter.Take = int.MaxValue;

            List<KpiGeneral> KpiGenerals = await KpiGeneralService.List(KpiGeneralFilter);
            KpiGenerals = await KpiGeneralService.BulkDelete(KpiGenerals);
            return true;
        }

        [Route(KpiGeneralRoute.Import), HttpPost]
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
            KpiYearFilter KpiYearFilter = new KpiYearFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiYearSelect.ALL
            };
            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiGeneral> KpiGenerals = new List<KpiGeneral>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(KpiGenerals);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int OrganizationIdColumn = 1 + StartColumn;
                int EmployeeIdColumn = 2 + StartColumn;
                int KpiYearIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                int CreatorIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string EmployeeIdValue = worksheet.Cells[i + StartRow, EmployeeIdColumn].Value?.ToString();
                    string KpiYearIdValue = worksheet.Cells[i + StartRow, KpiYearIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();

                    KpiGeneral KpiGeneral = new KpiGeneral();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    KpiGeneral.CreatorId = Creator == null ? 0 : Creator.Id;
                    KpiGeneral.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    KpiGeneral.EmployeeId = Employee == null ? 0 : Employee.Id;
                    KpiGeneral.Employee = Employee;
                    KpiYear KpiYear = KpiYears.Where(x => x.Id.ToString() == KpiYearIdValue).FirstOrDefault();
                    KpiGeneral.KpiYearId = KpiYear == null ? 0 : KpiYear.Id;
                    KpiGeneral.KpiYear = KpiYear;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    KpiGeneral.StatusId = Status == null ? 0 : Status.Id;
                    KpiGeneral.Status = Status;

                    KpiGenerals.Add(KpiGeneral);
                }
            }
            KpiGenerals = await KpiGeneralService.Import(KpiGenerals);
            if (KpiGenerals.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < KpiGenerals.Count; i++)
                {
                    KpiGeneral KpiGeneral = KpiGenerals[i];
                    if (!KpiGeneral.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.Id)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.Id)];
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.OrganizationId)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.OrganizationId)];
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.EmployeeId)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.EmployeeId)];
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.KpiYearId)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.KpiYearId)];
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.StatusId)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.StatusId)];
                        if (KpiGeneral.Errors.ContainsKey(nameof(KpiGeneral.CreatorId)))
                            Error += KpiGeneral.Errors[nameof(KpiGeneral.CreatorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }

        [Route(KpiGeneralRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiGeneral
                var KpiGeneralFilter = ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO);
                KpiGeneralFilter.Skip = 0;
                KpiGeneralFilter.Take = int.MaxValue;
                KpiGeneralFilter = KpiGeneralService.ToFilter(KpiGeneralFilter);
                List<KpiGeneral> KpiGenerals = await KpiGeneralService.List(KpiGeneralFilter);

                var KpiGeneralHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "OrganizationId",
                        "EmployeeId",
                        "KpiYearId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> KpiGeneralData = new List<object[]>();
                for (int i = 0; i < KpiGenerals.Count; i++)
                {
                    var KpiGeneral = KpiGenerals[i];
                    KpiGeneralData.Add(new Object[]
                    {
                        KpiGeneral.Id,
                        KpiGeneral.OrganizationId,
                        KpiGeneral.EmployeeId,
                        KpiGeneral.KpiYearId,
                        KpiGeneral.StatusId,
                        KpiGeneral.CreatorId,
                    });
                }
                excel.GenerateWorksheet("KpiGeneral", KpiGeneralHeaders, KpiGeneralData);
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
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
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
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region KpiYear
                var KpiYearFilter = new KpiYearFilter();
                KpiYearFilter.Selects = KpiYearSelect.ALL;
                KpiYearFilter.OrderBy = KpiYearOrder.Id;
                KpiYearFilter.OrderType = OrderType.ASC;
                KpiYearFilter.Skip = 0;
                KpiYearFilter.Take = int.MaxValue;
                List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);

                var KpiYearHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiYearData = new List<object[]>();
                for (int i = 0; i < KpiYears.Count; i++)
                {
                    var KpiYear = KpiYears[i];
                    KpiYearData.Add(new Object[]
                    {
                        KpiYear.Id,
                        KpiYear.Code,
                        KpiYear.Name,
                    });
                }
                excel.GenerateWorksheet("KpiYear", KpiYearHeaders, KpiYearData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
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

                #region KpiCriteriaGeneral
                var KpiCriteriaGeneralFilter = new KpiCriteriaGeneralFilter();
                KpiCriteriaGeneralFilter.Selects = KpiCriteriaGeneralSelect.ALL;
                KpiCriteriaGeneralFilter.OrderBy = KpiCriteriaGeneralOrder.Id;
                KpiCriteriaGeneralFilter.OrderType = OrderType.ASC;
                KpiCriteriaGeneralFilter.Skip = 0;
                KpiCriteriaGeneralFilter.Take = int.MaxValue;
                List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(KpiCriteriaGeneralFilter);

                var KpiCriteriaGeneralHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiCriteriaGeneralData = new List<object[]>();
                for (int i = 0; i < KpiCriteriaGenerals.Count; i++)
                {
                    var KpiCriteriaGeneral = KpiCriteriaGenerals[i];
                    KpiCriteriaGeneralData.Add(new Object[]
                    {
                        KpiCriteriaGeneral.Id,
                        KpiCriteriaGeneral.Code,
                        KpiCriteriaGeneral.Name,
                    });
                }
                excel.GenerateWorksheet("KpiCriteriaGeneral", KpiCriteriaGeneralHeaders, KpiCriteriaGeneralData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiGeneral.xlsx");
        }

        [Route(KpiGeneralRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region KpiGeneral
                var KpiGeneralHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "OrganizationId",
                        "EmployeeId",
                        "KpiYearId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> KpiGeneralData = new List<object[]>();
                excel.GenerateWorksheet("KpiGeneral", KpiGeneralHeaders, KpiGeneralData);
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
                        "PositionId",
                        "Department",
                        "OrganizationId",
                        "StatusId",
                        "Avatar",
                        "ProvinceId",
                        "SexId",
                        "Birthday",
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
                        AppUser.PositionId,
                        AppUser.Department,
                        AppUser.OrganizationId,
                        AppUser.StatusId,
                        AppUser.Avatar,
                        AppUser.ProvinceId,
                        AppUser.SexId,
                        AppUser.Birthday,
                    });
                }
                excel.GenerateWorksheet("AppUser", AppUserHeaders, AppUserData);
                #endregion
                #region KpiYear
                var KpiYearFilter = new KpiYearFilter();
                KpiYearFilter.Selects = KpiYearSelect.ALL;
                KpiYearFilter.OrderBy = KpiYearOrder.Id;
                KpiYearFilter.OrderType = OrderType.ASC;
                KpiYearFilter.Skip = 0;
                KpiYearFilter.Take = int.MaxValue;
                List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);

                var KpiYearHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiYearData = new List<object[]>();
                for (int i = 0; i < KpiYears.Count; i++)
                {
                    var KpiYear = KpiYears[i];
                    KpiYearData.Add(new Object[]
                    {
                        KpiYear.Id,
                        KpiYear.Code,
                        KpiYear.Name,
                    });
                }
                excel.GenerateWorksheet("KpiYear", KpiYearHeaders, KpiYearData);
                #endregion
                #region Organization
                var OrganizationFilter = new OrganizationFilter();
                OrganizationFilter.Selects = OrganizationSelect.ALL;
                OrganizationFilter.OrderBy = OrganizationOrder.Id;
                OrganizationFilter.OrderType = OrderType.ASC;
                OrganizationFilter.Skip = 0;
                OrganizationFilter.Take = int.MaxValue;
                List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);

                var OrganizationHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                        "ParentId",
                        "Path",
                        "Level",
                        "StatusId",
                        "Phone",
                        "Email",
                        "Address",
                    }
                };
                List<object[]> OrganizationData = new List<object[]>();
                for (int i = 0; i < Organizations.Count; i++)
                {
                    var Organization = Organizations[i];
                    OrganizationData.Add(new Object[]
                    {
                        Organization.Id,
                        Organization.Code,
                        Organization.Name,
                        Organization.ParentId,
                        Organization.Path,
                        Organization.Level,
                        Organization.StatusId,
                        Organization.Phone,
                        Organization.Email,
                        Organization.Address,
                    });
                }
                excel.GenerateWorksheet("Organization", OrganizationHeaders, OrganizationData);
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
                #region KpiCriteriaGeneral
                var KpiCriteriaGeneralFilter = new KpiCriteriaGeneralFilter();
                KpiCriteriaGeneralFilter.Selects = KpiCriteriaGeneralSelect.ALL;
                KpiCriteriaGeneralFilter.OrderBy = KpiCriteriaGeneralOrder.Id;
                KpiCriteriaGeneralFilter.OrderType = OrderType.ASC;
                KpiCriteriaGeneralFilter.Skip = 0;
                KpiCriteriaGeneralFilter.Take = int.MaxValue;
                List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(KpiCriteriaGeneralFilter);

                var KpiCriteriaGeneralHeaders = new List<string[]>()
                {
                    new string[] {
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> KpiCriteriaGeneralData = new List<object[]>();
                for (int i = 0; i < KpiCriteriaGenerals.Count; i++)
                {
                    var KpiCriteriaGeneral = KpiCriteriaGenerals[i];
                    KpiCriteriaGeneralData.Add(new Object[]
                    {
                        KpiCriteriaGeneral.Id,
                        KpiCriteriaGeneral.Code,
                        KpiCriteriaGeneral.Name,
                    });
                }
                excel.GenerateWorksheet("KpiCriteriaGeneral", KpiCriteriaGeneralHeaders, KpiCriteriaGeneralData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "KpiGeneral.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter();
            KpiGeneralFilter = KpiGeneralService.ToFilter(KpiGeneralFilter);
            if (Id == 0)
            {

            }
            else
            {
                KpiGeneralFilter.Id = new IdFilter { Equal = Id };
                int count = await KpiGeneralService.Count(KpiGeneralFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private KpiGeneral ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            KpiGeneral KpiGeneral = new KpiGeneral();
            KpiGeneral.Id = KpiGeneral_KpiGeneralDTO.Id;
            KpiGeneral.OrganizationId = KpiGeneral_KpiGeneralDTO.OrganizationId;
            KpiGeneral.EmployeeId = KpiGeneral_KpiGeneralDTO.EmployeeId;
            KpiGeneral.EmployeeIds = KpiGeneral_KpiGeneralDTO.EmployeeIds;
            KpiGeneral.KpiYearId = KpiGeneral_KpiGeneralDTO.KpiYearId;
            KpiGeneral.StatusId = KpiGeneral_KpiGeneralDTO.StatusId;
            KpiGeneral.CreatorId = KpiGeneral_KpiGeneralDTO.CreatorId;
            KpiGeneral.Creator = KpiGeneral_KpiGeneralDTO.Creator == null ? null : new AppUser
            {
                Id = KpiGeneral_KpiGeneralDTO.Creator.Id,
                Username = KpiGeneral_KpiGeneralDTO.Creator.Username,
                DisplayName = KpiGeneral_KpiGeneralDTO.Creator.DisplayName,
                Address = KpiGeneral_KpiGeneralDTO.Creator.Address,
                Email = KpiGeneral_KpiGeneralDTO.Creator.Email,
                Phone = KpiGeneral_KpiGeneralDTO.Creator.Phone,
                PositionId = KpiGeneral_KpiGeneralDTO.Creator.PositionId,
                Department = KpiGeneral_KpiGeneralDTO.Creator.Department,
                OrganizationId = KpiGeneral_KpiGeneralDTO.Creator.OrganizationId,
                StatusId = KpiGeneral_KpiGeneralDTO.Creator.StatusId,
                Avatar = KpiGeneral_KpiGeneralDTO.Creator.Avatar,
                ProvinceId = KpiGeneral_KpiGeneralDTO.Creator.ProvinceId,
                SexId = KpiGeneral_KpiGeneralDTO.Creator.SexId,
                Birthday = KpiGeneral_KpiGeneralDTO.Creator.Birthday,
            };
            KpiGeneral.Employee = KpiGeneral_KpiGeneralDTO.Employee == null ? null : new AppUser
            {
                Id = KpiGeneral_KpiGeneralDTO.Employee.Id,
                Username = KpiGeneral_KpiGeneralDTO.Employee.Username,
                DisplayName = KpiGeneral_KpiGeneralDTO.Employee.DisplayName,
                Address = KpiGeneral_KpiGeneralDTO.Employee.Address,
                Email = KpiGeneral_KpiGeneralDTO.Employee.Email,
                Phone = KpiGeneral_KpiGeneralDTO.Employee.Phone,
                PositionId = KpiGeneral_KpiGeneralDTO.Employee.PositionId,
                Department = KpiGeneral_KpiGeneralDTO.Employee.Department,
                OrganizationId = KpiGeneral_KpiGeneralDTO.Employee.OrganizationId,
                StatusId = KpiGeneral_KpiGeneralDTO.Employee.StatusId,
                Avatar = KpiGeneral_KpiGeneralDTO.Employee.Avatar,
                ProvinceId = KpiGeneral_KpiGeneralDTO.Employee.ProvinceId,
                SexId = KpiGeneral_KpiGeneralDTO.Employee.SexId,
                Birthday = KpiGeneral_KpiGeneralDTO.Employee.Birthday,
            };
            KpiGeneral.KpiYear = KpiGeneral_KpiGeneralDTO.KpiYear == null ? null : new KpiYear
            {
                Id = KpiGeneral_KpiGeneralDTO.KpiYear.Id,
                Code = KpiGeneral_KpiGeneralDTO.KpiYear.Code,
                Name = KpiGeneral_KpiGeneralDTO.KpiYear.Name,
            };
            KpiGeneral.Organization = KpiGeneral_KpiGeneralDTO.Organization == null ? null : new Organization
            {
                Id = KpiGeneral_KpiGeneralDTO.Organization.Id,
                Code = KpiGeneral_KpiGeneralDTO.Organization.Code,
                Name = KpiGeneral_KpiGeneralDTO.Organization.Name,
                ParentId = KpiGeneral_KpiGeneralDTO.Organization.ParentId,
                Path = KpiGeneral_KpiGeneralDTO.Organization.Path,
                Level = KpiGeneral_KpiGeneralDTO.Organization.Level,
                StatusId = KpiGeneral_KpiGeneralDTO.Organization.StatusId,
                Phone = KpiGeneral_KpiGeneralDTO.Organization.Phone,
                Email = KpiGeneral_KpiGeneralDTO.Organization.Email,
                Address = KpiGeneral_KpiGeneralDTO.Organization.Address,
            };
            KpiGeneral.Status = KpiGeneral_KpiGeneralDTO.Status == null ? null : new Status
            {
                Id = KpiGeneral_KpiGeneralDTO.Status.Id,
                Code = KpiGeneral_KpiGeneralDTO.Status.Code,
                Name = KpiGeneral_KpiGeneralDTO.Status.Name,
            };
            KpiGeneral.KpiGeneralContents = KpiGeneral_KpiGeneralDTO.KpiGeneralContents?
                .Select(x => new KpiGeneralContent
                {
                    Id = x.Id,
                    KpiCriteriaGeneralId = x.KpiCriteriaGeneralId,
                    StatusId = x.StatusId,
                    KpiCriteriaGeneral = x.KpiCriteriaGeneral == null ? null : new KpiCriteriaGeneral
                    {
                        Id = x.KpiCriteriaGeneral.Id,
                        Code = x.KpiCriteriaGeneral.Code,
                        Name = x.KpiCriteriaGeneral.Name,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    KpiGeneralContentKpiPeriodMappings = x.KpiGeneralContentKpiPeriodMappings.Select(p => new KpiGeneralContentKpiPeriodMapping
                    {
                        Value = p.Value,
                        KpiPeriodId = p.Key 
                    }).ToList()
                }).ToList();
            KpiGeneral.BaseLanguage = CurrentContext.Language;
            return KpiGeneral;
        }

        private KpiGeneralFilter ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter();
            KpiGeneralFilter.Selects = KpiGeneralSelect.ALL;
            KpiGeneralFilter.Skip = KpiGeneral_KpiGeneralFilterDTO.Skip;
            KpiGeneralFilter.Take = KpiGeneral_KpiGeneralFilterDTO.Take;
            KpiGeneralFilter.OrderBy = KpiGeneral_KpiGeneralFilterDTO.OrderBy;
            KpiGeneralFilter.OrderType = KpiGeneral_KpiGeneralFilterDTO.OrderType;

            KpiGeneralFilter.Id = KpiGeneral_KpiGeneralFilterDTO.Id;
            KpiGeneralFilter.OrganizationId = KpiGeneral_KpiGeneralFilterDTO.OrganizationId;
            KpiGeneralFilter.EmployeeId = KpiGeneral_KpiGeneralFilterDTO.EmployeeId;
            KpiGeneralFilter.KpiYearId = KpiGeneral_KpiGeneralFilterDTO.KpiYearId;
            KpiGeneralFilter.StatusId = KpiGeneral_KpiGeneralFilterDTO.StatusId;
            KpiGeneralFilter.CreatorId = KpiGeneral_KpiGeneralFilterDTO.CreatorId;
            KpiGeneralFilter.CreatedAt = KpiGeneral_KpiGeneralFilterDTO.CreatedAt;
            KpiGeneralFilter.UpdatedAt = KpiGeneral_KpiGeneralFilterDTO.UpdatedAt;
            return KpiGeneralFilter;
        }

        [Route(KpiGeneralRoute.FilterListAppUser), HttpPost]
        public async Task<List<KpiGeneral_AppUserDTO>> FilterListAppUser([FromBody] KpiGeneral_AppUserFilterDTO KpiGeneral_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiGeneral_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiGeneral_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiGeneral_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = KpiGeneral_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = KpiGeneral_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = KpiGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = KpiGeneral_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = KpiGeneral_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = KpiGeneral_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = KpiGeneral_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiGeneral_AppUserDTO> KpiGeneral_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneral_AppUserDTO(x)).ToList();
            return KpiGeneral_AppUserDTOs;
        }
        [Route(KpiGeneralRoute.FilterListKpiYear), HttpPost]
        public async Task<List<KpiGeneral_KpiYearDTO>> FilterListKpiYear([FromBody] KpiGeneral_KpiYearFilterDTO KpiGeneral_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = int.MaxValue;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiGeneral_KpiYearDTO> KpiGeneral_KpiYearDTOs = KpiYears
                .Select(x => new KpiGeneral_KpiYearDTO(x)).ToList();
            return KpiGeneral_KpiYearDTOs;
        }
        [Route(KpiGeneralRoute.FilterListOrganization), HttpPost]
        public async Task<List<KpiGeneral_OrganizationDTO>> FilterListOrganization([FromBody] KpiGeneral_OrganizationFilterDTO KpiGeneral_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiGeneral_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiGeneral_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiGeneral_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiGeneral_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiGeneral_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiGeneral_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiGeneral_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiGeneral_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiGeneral_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiGeneral_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiGeneral_OrganizationDTO> KpiGeneral_OrganizationDTOs = Organizations
                .Select(x => new KpiGeneral_OrganizationDTO(x)).ToList();
            return KpiGeneral_OrganizationDTOs;
        }
        [Route(KpiGeneralRoute.FilterListStatus), HttpPost]
        public async Task<List<KpiGeneral_StatusDTO>> FilterListStatus([FromBody] KpiGeneral_StatusFilterDTO KpiGeneral_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiGeneral_StatusDTO> KpiGeneral_StatusDTOs = Statuses
                .Select(x => new KpiGeneral_StatusDTO(x)).ToList();
            return KpiGeneral_StatusDTOs;
        }

        [Route(KpiGeneralRoute.FilterListKpiCriteriaGeneral), HttpPost]
        public async Task<List<KpiGeneral_KpiCriteriaGeneralDTO>> FilterListKpiCriteriaGeneral([FromBody] KpiGeneral_KpiCriteriaGeneralFilterDTO KpiGeneral_KpiCriteriaGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter = new KpiCriteriaGeneralFilter();
            KpiCriteriaGeneralFilter.Skip = 0;
            KpiCriteriaGeneralFilter.Take = int.MaxValue;
            KpiCriteriaGeneralFilter.Take = 20;
            KpiCriteriaGeneralFilter.OrderBy = KpiCriteriaGeneralOrder.Id;
            KpiCriteriaGeneralFilter.OrderType = OrderType.ASC;
            KpiCriteriaGeneralFilter.Selects = KpiCriteriaGeneralSelect.ALL;

            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(KpiCriteriaGeneralFilter);
            List<KpiGeneral_KpiCriteriaGeneralDTO> KpiGeneral_KpiCriteriaGeneralDTOs = KpiCriteriaGenerals
                .Select(x => new KpiGeneral_KpiCriteriaGeneralDTO(x)).ToList();
            return KpiGeneral_KpiCriteriaGeneralDTOs;
        }

        [Route(KpiGeneralRoute.SingleListAppUser), HttpPost]
        public async Task<List<KpiGeneral_AppUserDTO>> SingleListAppUser([FromBody] KpiGeneral_AppUserFilterDTO KpiGeneral_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = KpiGeneral_AppUserFilterDTO.Address;
            AppUserFilter.Email = KpiGeneral_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiGeneral_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = KpiGeneral_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = KpiGeneral_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = KpiGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = KpiGeneral_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = KpiGeneral_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = KpiGeneral_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = KpiGeneral_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiGeneral_AppUserDTO> KpiGeneral_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneral_AppUserDTO(x)).ToList();
            return KpiGeneral_AppUserDTOs;
        }
        [Route(KpiGeneralRoute.SingleListKpiYear), HttpPost]
        public async Task<List<KpiGeneral_KpiYearDTO>> SingleListKpiYear([FromBody] KpiGeneral_KpiYearFilterDTO KpiGeneral_KpiYearFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiYearFilter KpiYearFilter = new KpiYearFilter();
            KpiYearFilter.Skip = 0;
            KpiYearFilter.Take = int.MaxValue;
            KpiYearFilter.Take = 20;
            KpiYearFilter.OrderBy = KpiYearOrder.Id;
            KpiYearFilter.OrderType = OrderType.ASC;
            KpiYearFilter.Selects = KpiYearSelect.ALL;

            List<KpiYear> KpiYears = await KpiYearService.List(KpiYearFilter);
            List<KpiGeneral_KpiYearDTO> KpiGeneral_KpiYearDTOs = KpiYears
                .Select(x => new KpiGeneral_KpiYearDTO(x)).ToList();
            return KpiGeneral_KpiYearDTOs;
        }
        [Route(KpiGeneralRoute.SingleListOrganization), HttpPost]
        public async Task<List<KpiGeneral_OrganizationDTO>> SingleListOrganization([FromBody] KpiGeneral_OrganizationFilterDTO KpiGeneral_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = KpiGeneral_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = KpiGeneral_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = KpiGeneral_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = KpiGeneral_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = KpiGeneral_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = KpiGeneral_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = KpiGeneral_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = KpiGeneral_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = KpiGeneral_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = KpiGeneral_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<KpiGeneral_OrganizationDTO> KpiGeneral_OrganizationDTOs = Organizations
                .Select(x => new KpiGeneral_OrganizationDTO(x)).ToList();
            return KpiGeneral_OrganizationDTOs;
        }
        [Route(KpiGeneralRoute.SingleListStatus), HttpPost]
        public async Task<List<KpiGeneral_StatusDTO>> SingleListStatus([FromBody] KpiGeneral_StatusFilterDTO KpiGeneral_StatusFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = int.MaxValue;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<KpiGeneral_StatusDTO> KpiGeneral_StatusDTOs = Statuses
                .Select(x => new KpiGeneral_StatusDTO(x)).ToList();
            return KpiGeneral_StatusDTOs;
        }

        [Route(KpiGeneralRoute.SingleListKpiCriteriaGeneral), HttpPost]
        public async Task<List<KpiGeneral_KpiCriteriaGeneralDTO>> SingleListKpiCriteriaGeneral([FromBody] KpiGeneral_KpiCriteriaGeneralFilterDTO KpiGeneral_KpiCriteriaGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiCriteriaGeneralFilter KpiCriteriaGeneralFilter = new KpiCriteriaGeneralFilter();
            KpiCriteriaGeneralFilter.Skip = 0;
            KpiCriteriaGeneralFilter.Take = int.MaxValue;
            KpiCriteriaGeneralFilter.Take = 20;
            KpiCriteriaGeneralFilter.OrderBy = KpiCriteriaGeneralOrder.Id;
            KpiCriteriaGeneralFilter.OrderType = OrderType.ASC;
            KpiCriteriaGeneralFilter.Selects = KpiCriteriaGeneralSelect.ALL;

            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(KpiCriteriaGeneralFilter);
            List<KpiGeneral_KpiCriteriaGeneralDTO> KpiGeneral_KpiCriteriaGeneralDTOs = KpiCriteriaGenerals
                .Select(x => new KpiGeneral_KpiCriteriaGeneralDTO(x)).ToList();
            return KpiGeneral_KpiCriteriaGeneralDTOs;
        }

        [Route(KpiGeneralRoute.CountAppUser), HttpPost]
        public async Task<long> CountAppUser([FromBody] KpiGeneral_AppUserFilterDTO KpiGeneral_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Id = KpiGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiGeneral_AppUserFilterDTO.Email;
            AppUserFilter.Phone = KpiGeneral_AppUserFilterDTO.Phone;
            AppUserFilter.OrganizationId = KpiGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            return await KpiGeneralService.CountAppUser(AppUserFilter, KpiGeneral_AppUserFilterDTO.KpiYearId);
        }
        [Route(KpiGeneralRoute.ListAppUser), HttpPost]
        public async Task<List<KpiGeneral_AppUserDTO>> ListAppUser([FromBody] KpiGeneral_AppUserFilterDTO KpiGeneral_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = KpiGeneral_AppUserFilterDTO.Skip;
            AppUserFilter.Take = KpiGeneral_AppUserFilterDTO.Take;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.Email = KpiGeneral_AppUserFilterDTO.Email;
            AppUserFilter.OrganizationId = KpiGeneral_AppUserFilterDTO.OrganizationId;
            AppUserFilter.Phone = KpiGeneral_AppUserFilterDTO.Phone;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            List<AppUser> AppUsers = await KpiGeneralService.ListAppUser(AppUserFilter, KpiGeneral_AppUserFilterDTO.KpiYearId);
            List<KpiGeneral_AppUserDTO> KpiGeneral_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneral_AppUserDTO(x)).ToList();
            return KpiGeneral_AppUserDTOs;
        }

    }
}

