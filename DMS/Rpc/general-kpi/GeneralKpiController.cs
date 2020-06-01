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
using DMS.Services.MGeneralKpi;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MGeneralCriteria;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpiController : RpcController
    {
        private IAppUserService AppUserService;
        private IKpiPeriodService KpiPeriodService;
        private IOrganizationService OrganizationService;
        private IStatusService StatusService;
        private IGeneralKpiService GeneralKpiService;
        private IGeneralCriteriaService GeneralCriteriaService;
        private ICurrentContext CurrentContext;
        public GeneralKpiController(
            IAppUserService AppUserService,
            IKpiPeriodService KpiPeriodService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IGeneralKpiService GeneralKpiService,
            IGeneralCriteriaService GeneralCriteriaService,
            ICurrentContext CurrentContext
        )
        {
            this.AppUserService = AppUserService;
            this.KpiPeriodService = KpiPeriodService;
            this.OrganizationService = OrganizationService;
            this.StatusService = StatusService;
            this.GeneralKpiService = GeneralKpiService;
            this.GeneralCriteriaService = GeneralCriteriaService;
            this.CurrentContext = CurrentContext;
        }

        [Route(GeneralKpiRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] GeneralKpi_GeneralKpiFilterDTO GeneralKpi_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = ConvertFilterDTOToFilterEntity(GeneralKpi_GeneralKpiFilterDTO);
            GeneralKpiFilter = GeneralKpiService.ToFilter(GeneralKpiFilter);
            int count = await GeneralKpiService.Count(GeneralKpiFilter);
            return count;
        }

        [Route(GeneralKpiRoute.List), HttpPost]
        public async Task<ActionResult<List<GeneralKpi_GeneralKpiDTO>>> List([FromBody] GeneralKpi_GeneralKpiFilterDTO GeneralKpi_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = ConvertFilterDTOToFilterEntity(GeneralKpi_GeneralKpiFilterDTO);
            GeneralKpiFilter = GeneralKpiService.ToFilter(GeneralKpiFilter);
            List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);
            List<GeneralKpi_GeneralKpiDTO> GeneralKpi_GeneralKpiDTOs = GeneralKpis
                .Select(c => new GeneralKpi_GeneralKpiDTO(c)).ToList();
            return GeneralKpi_GeneralKpiDTOs;
        }

        [Route(GeneralKpiRoute.Get), HttpPost]
        public async Task<ActionResult<GeneralKpi_GeneralKpiDTO>> Get([FromBody]GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(GeneralKpi_GeneralKpiDTO.Id))
                return Forbid();

            GeneralKpi GeneralKpi = await GeneralKpiService.Get(GeneralKpi_GeneralKpiDTO.Id);
            return new GeneralKpi_GeneralKpiDTO(GeneralKpi);
        }

        [Route(GeneralKpiRoute.Create), HttpPost]
        public async Task<ActionResult<GeneralKpi_GeneralKpiDTO>> Create([FromBody] GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(GeneralKpi_GeneralKpiDTO.Id))
                return Forbid();

            GeneralKpi GeneralKpi = ConvertDTOToEntity(GeneralKpi_GeneralKpiDTO);
            GeneralKpi = await GeneralKpiService.Create(GeneralKpi);
            GeneralKpi_GeneralKpiDTO = new GeneralKpi_GeneralKpiDTO(GeneralKpi);
            if (GeneralKpi.IsValidated)
                return GeneralKpi_GeneralKpiDTO;
            else
                return BadRequest(GeneralKpi_GeneralKpiDTO);
        }

        [Route(GeneralKpiRoute.CreateDraft), HttpPost]
        public async Task<ActionResult<GeneralKpi_GeneralKpiDTO>> CreateDraft([FromBody] GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(GeneralKpi_GeneralKpiDTO.Id))
                return Forbid();

            List<GeneralCriteria> GeneralCriterias = await GeneralCriteriaService.List(new GeneralCriteriaFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = GeneralCriteriaSelect.ALL
            });

            GeneralKpi_GeneralKpiDTO = new GeneralKpi_GeneralKpiDTO
            {
                GeneralKpiCriteriaMappings = new List<GeneralKpi_GeneralKpiCriteriaMappingDTO>()
            };
            foreach (var GeneralCriteria in GeneralCriterias)
            {
                GeneralKpi_GeneralKpiDTO.GeneralKpiCriteriaMappings.Add(new GeneralKpi_GeneralKpiCriteriaMappingDTO
                {
                    GeneralCriteriaId = GeneralCriteria.Id,
                    GeneralCriteria = new GeneralKpi_GeneralCriteriaDTO(GeneralCriteria),
                    
                });
            }
            return GeneralKpi_GeneralKpiDTO;
        }

        [Route(GeneralKpiRoute.Update), HttpPost]
        public async Task<ActionResult<GeneralKpi_GeneralKpiDTO>> Update([FromBody] GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(GeneralKpi_GeneralKpiDTO.Id))
                return Forbid();

            GeneralKpi GeneralKpi = ConvertDTOToEntity(GeneralKpi_GeneralKpiDTO);
            GeneralKpi = await GeneralKpiService.Update(GeneralKpi);
            GeneralKpi_GeneralKpiDTO = new GeneralKpi_GeneralKpiDTO(GeneralKpi);
            if (GeneralKpi.IsValidated)
                return GeneralKpi_GeneralKpiDTO;
            else
                return BadRequest(GeneralKpi_GeneralKpiDTO);
        }

        [Route(GeneralKpiRoute.Delete), HttpPost]
        public async Task<ActionResult<GeneralKpi_GeneralKpiDTO>> Delete([FromBody] GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(GeneralKpi_GeneralKpiDTO.Id))
                return Forbid();

            GeneralKpi GeneralKpi = ConvertDTOToEntity(GeneralKpi_GeneralKpiDTO);
            GeneralKpi = await GeneralKpiService.Delete(GeneralKpi);
            GeneralKpi_GeneralKpiDTO = new GeneralKpi_GeneralKpiDTO(GeneralKpi);
            if (GeneralKpi.IsValidated)
                return GeneralKpi_GeneralKpiDTO;
            else
                return BadRequest(GeneralKpi_GeneralKpiDTO);
        }
        
        [Route(GeneralKpiRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter = GeneralKpiService.ToFilter(GeneralKpiFilter);
            GeneralKpiFilter.Id = new IdFilter { In = Ids };
            GeneralKpiFilter.Selects = GeneralKpiSelect.Id;
            GeneralKpiFilter.Skip = 0;
            GeneralKpiFilter.Take = int.MaxValue;

            List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);
            GeneralKpis = await GeneralKpiService.BulkDelete(GeneralKpis);
            return true;
        }
        
        [Route(GeneralKpiRoute.Import), HttpPost]
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
            StatusFilter StatusFilter = new StatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StatusSelect.ALL
            };
            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<GeneralKpi> GeneralKpis = new List<GeneralKpi>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(GeneralKpis);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int OrganizationIdColumn = 1 + StartColumn;
                int EmployeeIdColumn = 2 + StartColumn;
                int KpiPeriodIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                int CreatorIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string EmployeeIdValue = worksheet.Cells[i + StartRow, EmployeeIdColumn].Value?.ToString();
                    string KpiPeriodIdValue = worksheet.Cells[i + StartRow, KpiPeriodIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    string CreatorIdValue = worksheet.Cells[i + StartRow, CreatorIdColumn].Value?.ToString();
                    
                    GeneralKpi GeneralKpi = new GeneralKpi();
                    AppUser Creator = Creators.Where(x => x.Id.ToString() == CreatorIdValue).FirstOrDefault();
                    GeneralKpi.CreatorId = Creator == null ? 0 : Creator.Id;
                    GeneralKpi.Creator = Creator;
                    AppUser Employee = Employees.Where(x => x.Id.ToString() == EmployeeIdValue).FirstOrDefault();
                    GeneralKpi.EmployeeId = Employee == null ? 0 : Employee.Id;
                    GeneralKpi.Employee = Employee;
                    KpiPeriod KpiPeriod = KpiPeriods.Where(x => x.Id.ToString() == KpiPeriodIdValue).FirstOrDefault();
                    GeneralKpi.KpiPeriodId = KpiPeriod == null ? 0 : KpiPeriod.Id;
                    GeneralKpi.KpiPeriod = KpiPeriod;
                    Status Status = Statuses.Where(x => x.Id.ToString() == StatusIdValue).FirstOrDefault();
                    GeneralKpi.StatusId = Status == null ? 0 : Status.Id;
                    GeneralKpi.Status = Status;
                    
                    GeneralKpis.Add(GeneralKpi);
                }
            }
            GeneralKpis = await GeneralKpiService.Import(GeneralKpis);
            if (GeneralKpis.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < GeneralKpis.Count; i++)
                {
                    GeneralKpi GeneralKpi = GeneralKpis[i];
                    if (!GeneralKpi.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.Id)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.Id)];
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.OrganizationId)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.OrganizationId)];
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.EmployeeId)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.EmployeeId)];
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.KpiPeriodId)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.KpiPeriodId)];
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.StatusId)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.StatusId)];
                        if (GeneralKpi.Errors.ContainsKey(nameof(GeneralKpi.CreatorId)))
                            Error += GeneralKpi.Errors[nameof(GeneralKpi.CreatorId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(GeneralKpiRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] GeneralKpi_GeneralKpiFilterDTO GeneralKpi_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region GeneralKpi
                var GeneralKpiFilter = ConvertFilterDTOToFilterEntity(GeneralKpi_GeneralKpiFilterDTO);
                GeneralKpiFilter.Skip = 0;
                GeneralKpiFilter.Take = int.MaxValue;
                GeneralKpiFilter = GeneralKpiService.ToFilter(GeneralKpiFilter);
                List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);

                var GeneralKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "EmployeeId",
                        "KpiPeriodId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> GeneralKpiData = new List<object[]>();
                for (int i = 0; i < GeneralKpis.Count; i++)
                {
                    var GeneralKpi = GeneralKpis[i];
                    GeneralKpiData.Add(new Object[]
                    {
                        GeneralKpi.Id,
                        GeneralKpi.OrganizationId,
                        GeneralKpi.EmployeeId,
                        GeneralKpi.KpiPeriodId,
                        GeneralKpi.StatusId,
                        GeneralKpi.CreatorId,
                    });
                }
                excel.GenerateWorksheet("GeneralKpi", GeneralKpiHeaders, GeneralKpiData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "GeneralKpi.xlsx");
        }

        [Route(GeneralKpiRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] GeneralKpi_GeneralKpiFilterDTO GeneralKpi_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region GeneralKpi
                var GeneralKpiHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "OrganizationId",
                        "EmployeeId",
                        "KpiPeriodId",
                        "StatusId",
                        "CreatorId",
                    }
                };
                List<object[]> GeneralKpiData = new List<object[]>();
                excel.GenerateWorksheet("GeneralKpi", GeneralKpiHeaders, GeneralKpiData);
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
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "GeneralKpi.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter = GeneralKpiService.ToFilter(GeneralKpiFilter);
            if (Id == 0)
            {

            }
            else
            {
                GeneralKpiFilter.Id = new IdFilter { Equal = Id };
                int count = await GeneralKpiService.Count(GeneralKpiFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private GeneralKpi ConvertDTOToEntity(GeneralKpi_GeneralKpiDTO GeneralKpi_GeneralKpiDTO)
        {
            GeneralKpi GeneralKpi = new GeneralKpi();
            GeneralKpi.Id = GeneralKpi_GeneralKpiDTO.Id;
            GeneralKpi.OrganizationId = GeneralKpi_GeneralKpiDTO.OrganizationId;
            GeneralKpi.EmployeeId = GeneralKpi_GeneralKpiDTO.EmployeeId;
            GeneralKpi.KpiPeriodId = GeneralKpi_GeneralKpiDTO.KpiPeriodId;
            GeneralKpi.StatusId = GeneralKpi_GeneralKpiDTO.StatusId;
            GeneralKpi.CreatorId = GeneralKpi_GeneralKpiDTO.CreatorId;
            GeneralKpi.Creator = GeneralKpi_GeneralKpiDTO.Creator == null ? null : new AppUser
            {
                Id = GeneralKpi_GeneralKpiDTO.Creator.Id,
                Username = GeneralKpi_GeneralKpiDTO.Creator.Username,
                DisplayName = GeneralKpi_GeneralKpiDTO.Creator.DisplayName,
                Address = GeneralKpi_GeneralKpiDTO.Creator.Address,
                Email = GeneralKpi_GeneralKpiDTO.Creator.Email,
                Phone = GeneralKpi_GeneralKpiDTO.Creator.Phone,
                PositionId = GeneralKpi_GeneralKpiDTO.Creator.PositionId,
                Department = GeneralKpi_GeneralKpiDTO.Creator.Department,
                OrganizationId = GeneralKpi_GeneralKpiDTO.Creator.OrganizationId,
                StatusId = GeneralKpi_GeneralKpiDTO.Creator.StatusId,
                Avatar = GeneralKpi_GeneralKpiDTO.Creator.Avatar,
                ProvinceId = GeneralKpi_GeneralKpiDTO.Creator.ProvinceId,
                SexId = GeneralKpi_GeneralKpiDTO.Creator.SexId,
                Birthday = GeneralKpi_GeneralKpiDTO.Creator.Birthday,
            };
            GeneralKpi.Employee = GeneralKpi_GeneralKpiDTO.Employee == null ? null : new AppUser
            {
                Id = GeneralKpi_GeneralKpiDTO.Employee.Id,
                Username = GeneralKpi_GeneralKpiDTO.Employee.Username,
                DisplayName = GeneralKpi_GeneralKpiDTO.Employee.DisplayName,
                Address = GeneralKpi_GeneralKpiDTO.Employee.Address,
                Email = GeneralKpi_GeneralKpiDTO.Employee.Email,
                Phone = GeneralKpi_GeneralKpiDTO.Employee.Phone,
                PositionId = GeneralKpi_GeneralKpiDTO.Employee.PositionId,
                Department = GeneralKpi_GeneralKpiDTO.Employee.Department,
                OrganizationId = GeneralKpi_GeneralKpiDTO.Employee.OrganizationId,
                StatusId = GeneralKpi_GeneralKpiDTO.Employee.StatusId,
                Avatar = GeneralKpi_GeneralKpiDTO.Employee.Avatar,
                ProvinceId = GeneralKpi_GeneralKpiDTO.Employee.ProvinceId,
                SexId = GeneralKpi_GeneralKpiDTO.Employee.SexId,
                Birthday = GeneralKpi_GeneralKpiDTO.Employee.Birthday,
            };
            GeneralKpi.KpiPeriod = GeneralKpi_GeneralKpiDTO.KpiPeriod == null ? null : new KpiPeriod
            {
                Id = GeneralKpi_GeneralKpiDTO.KpiPeriod.Id,
                Code = GeneralKpi_GeneralKpiDTO.KpiPeriod.Code,
                Name = GeneralKpi_GeneralKpiDTO.KpiPeriod.Name,
            };
            GeneralKpi.Organization = GeneralKpi_GeneralKpiDTO.Organization == null ? null : new Organization
            {
                Id = GeneralKpi_GeneralKpiDTO.Organization.Id,
                Code = GeneralKpi_GeneralKpiDTO.Organization.Code,
                Name = GeneralKpi_GeneralKpiDTO.Organization.Name,
                ParentId = GeneralKpi_GeneralKpiDTO.Organization.ParentId,
                Path = GeneralKpi_GeneralKpiDTO.Organization.Path,
                Level = GeneralKpi_GeneralKpiDTO.Organization.Level,
                StatusId = GeneralKpi_GeneralKpiDTO.Organization.StatusId,
                Phone = GeneralKpi_GeneralKpiDTO.Organization.Phone,
                Email = GeneralKpi_GeneralKpiDTO.Organization.Email,
                Address = GeneralKpi_GeneralKpiDTO.Organization.Address,
            };
            GeneralKpi.Status = GeneralKpi_GeneralKpiDTO.Status == null ? null : new Status
            {
                Id = GeneralKpi_GeneralKpiDTO.Status.Id,
                Code = GeneralKpi_GeneralKpiDTO.Status.Code,
                Name = GeneralKpi_GeneralKpiDTO.Status.Name,
            };
            GeneralKpi.EmployeeIds = GeneralKpi_GeneralKpiDTO.EmployeeIds;
            GeneralKpi.GeneralKpiCriteriaMappings = GeneralKpi_GeneralKpiDTO.GeneralKpiCriteriaMappings?.Select(x => new GeneralKpiCriteriaMapping
            {
                GeneralKpiId = x.GeneralKpiId,
                GeneralCriteriaId = x.GeneralCriteriaId,
                M01 = x.M01,
                M02 = x.M02,
                M03 = x.M03,
                M04 = x.M04,
                M05 = x.M05,
                M06 = x.M06,
                M07 = x.M07,
                M08 = x.M08,
                M09 = x.M09,
                M10 = x.M10,
                M11 = x.M11,
                M12 = x.M12,
                Q01 = x.Q01,
                Q02 = x.Q02,
                Q03 = x.Q03,
                Q04 = x.Q04,
                Y01 = x.Y01,
                StatusId = x.StatusId
            }).ToList();
            GeneralKpi.BaseLanguage = CurrentContext.Language;
            return GeneralKpi;
        }

        private GeneralKpiFilter ConvertFilterDTOToFilterEntity(GeneralKpi_GeneralKpiFilterDTO GeneralKpi_GeneralKpiFilterDTO)
        {
            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
            GeneralKpiFilter.Skip = GeneralKpi_GeneralKpiFilterDTO.Skip;
            GeneralKpiFilter.Take = GeneralKpi_GeneralKpiFilterDTO.Take;
            GeneralKpiFilter.OrderBy = GeneralKpi_GeneralKpiFilterDTO.OrderBy;
            GeneralKpiFilter.OrderType = GeneralKpi_GeneralKpiFilterDTO.OrderType;

            GeneralKpiFilter.Id = GeneralKpi_GeneralKpiFilterDTO.Id;
            GeneralKpiFilter.OrganizationId = GeneralKpi_GeneralKpiFilterDTO.OrganizationId;
            GeneralKpiFilter.EmployeeId = GeneralKpi_GeneralKpiFilterDTO.EmployeeId;
            GeneralKpiFilter.KpiPeriodId = GeneralKpi_GeneralKpiFilterDTO.KpiPeriodId;
            GeneralKpiFilter.StatusId = GeneralKpi_GeneralKpiFilterDTO.StatusId;
            GeneralKpiFilter.CreatorId = GeneralKpi_GeneralKpiFilterDTO.CreatorId;
            GeneralKpiFilter.CreatedAt = GeneralKpi_GeneralKpiFilterDTO.CreatedAt;
            GeneralKpiFilter.UpdatedAt = GeneralKpi_GeneralKpiFilterDTO.UpdatedAt;
            return GeneralKpiFilter;
        }

        [Route(GeneralKpiRoute.FilterListAppUser), HttpPost]
        public async Task<List<GeneralKpi_AppUserDTO>> FilterListAppUser([FromBody] GeneralKpi_AppUserFilterDTO GeneralKpi_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = GeneralKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = GeneralKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = GeneralKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = GeneralKpi_AppUserFilterDTO.Address;
            AppUserFilter.Email = GeneralKpi_AppUserFilterDTO.Email;
            AppUserFilter.Phone = GeneralKpi_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = GeneralKpi_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = GeneralKpi_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = GeneralKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = GeneralKpi_AppUserFilterDTO.StatusId;
            AppUserFilter.ProvinceId = GeneralKpi_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = GeneralKpi_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = GeneralKpi_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<GeneralKpi_AppUserDTO> GeneralKpi_AppUserDTOs = AppUsers
                .Select(x => new GeneralKpi_AppUserDTO(x)).ToList();
            return GeneralKpi_AppUserDTOs;
        }
        [Route(GeneralKpiRoute.FilterListKpiPeriod), HttpPost]
        public async Task<List<GeneralKpi_KpiPeriodDTO>> FilterListKpiPeriod([FromBody] GeneralKpi_KpiPeriodFilterDTO GeneralKpi_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = GeneralKpi_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = GeneralKpi_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = GeneralKpi_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<GeneralKpi_KpiPeriodDTO> GeneralKpi_KpiPeriodDTOs = KpiPeriods
                .Select(x => new GeneralKpi_KpiPeriodDTO(x)).ToList();
            return GeneralKpi_KpiPeriodDTOs;
        }
        [Route(GeneralKpiRoute.FilterListOrganization), HttpPost]
        public async Task<List<GeneralKpi_OrganizationDTO>> FilterListOrganization([FromBody] GeneralKpi_OrganizationFilterDTO GeneralKpi_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = GeneralKpi_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = GeneralKpi_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = GeneralKpi_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = GeneralKpi_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = GeneralKpi_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = GeneralKpi_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = GeneralKpi_OrganizationFilterDTO.StatusId;
            OrganizationFilter.Phone = GeneralKpi_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = GeneralKpi_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = GeneralKpi_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<GeneralKpi_OrganizationDTO> GeneralKpi_OrganizationDTOs = Organizations
                .Select(x => new GeneralKpi_OrganizationDTO(x)).ToList();
            return GeneralKpi_OrganizationDTOs;
        }
        [Route(GeneralKpiRoute.FilterListStatus), HttpPost]
        public async Task<List<GeneralKpi_StatusDTO>> FilterListStatus([FromBody] GeneralKpi_StatusFilterDTO GeneralKpi_StatusFilterDTO)
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
            List<GeneralKpi_StatusDTO> GeneralKpi_StatusDTOs = Statuses
                .Select(x => new GeneralKpi_StatusDTO(x)).ToList();
            return GeneralKpi_StatusDTOs;
        }

        [Route(GeneralKpiRoute.SingleListAppUser), HttpPost]
        public async Task<List<GeneralKpi_AppUserDTO>> SingleListAppUser([FromBody] GeneralKpi_AppUserFilterDTO GeneralKpi_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = GeneralKpi_AppUserFilterDTO.Id;
            AppUserFilter.Username = GeneralKpi_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = GeneralKpi_AppUserFilterDTO.DisplayName;
            AppUserFilter.Address = GeneralKpi_AppUserFilterDTO.Address;
            AppUserFilter.Email = GeneralKpi_AppUserFilterDTO.Email;
            AppUserFilter.Phone = GeneralKpi_AppUserFilterDTO.Phone;
            AppUserFilter.PositionId = GeneralKpi_AppUserFilterDTO.PositionId;
            AppUserFilter.Department = GeneralKpi_AppUserFilterDTO.Department;
            AppUserFilter.OrganizationId = GeneralKpi_AppUserFilterDTO.OrganizationId;
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            AppUserFilter.ProvinceId = GeneralKpi_AppUserFilterDTO.ProvinceId;
            AppUserFilter.SexId = GeneralKpi_AppUserFilterDTO.SexId;
            AppUserFilter.Birthday = GeneralKpi_AppUserFilterDTO.Birthday;

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<GeneralKpi_AppUserDTO> GeneralKpi_AppUserDTOs = AppUsers
                .Select(x => new GeneralKpi_AppUserDTO(x)).ToList();
            return GeneralKpi_AppUserDTOs;
        }
        [Route(GeneralKpiRoute.SingleListKpiPeriod), HttpPost]
        public async Task<List<GeneralKpi_KpiPeriodDTO>> SingleListKpiPeriod([FromBody] GeneralKpi_KpiPeriodFilterDTO GeneralKpi_KpiPeriodFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiPeriodFilter KpiPeriodFilter = new KpiPeriodFilter();
            KpiPeriodFilter.Skip = 0;
            KpiPeriodFilter.Take = 20;
            KpiPeriodFilter.OrderBy = KpiPeriodOrder.Id;
            KpiPeriodFilter.OrderType = OrderType.ASC;
            KpiPeriodFilter.Selects = KpiPeriodSelect.ALL;
            KpiPeriodFilter.Id = GeneralKpi_KpiPeriodFilterDTO.Id;
            KpiPeriodFilter.Code = GeneralKpi_KpiPeriodFilterDTO.Code;
            KpiPeriodFilter.Name = GeneralKpi_KpiPeriodFilterDTO.Name;

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(KpiPeriodFilter);
            List<GeneralKpi_KpiPeriodDTO> GeneralKpi_KpiPeriodDTOs = KpiPeriods
                .Select(x => new GeneralKpi_KpiPeriodDTO(x)).ToList();
            return GeneralKpi_KpiPeriodDTOs;
        }
        [Route(GeneralKpiRoute.SingleListOrganization), HttpPost]
        public async Task<List<GeneralKpi_OrganizationDTO>> SingleListOrganization([FromBody] GeneralKpi_OrganizationFilterDTO GeneralKpi_OrganizationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrganizationFilter OrganizationFilter = new OrganizationFilter();
            OrganizationFilter.Skip = 0;
            OrganizationFilter.Take = int.MaxValue;
            OrganizationFilter.OrderBy = OrganizationOrder.Id;
            OrganizationFilter.OrderType = OrderType.ASC;
            OrganizationFilter.Selects = OrganizationSelect.ALL;
            OrganizationFilter.Id = GeneralKpi_OrganizationFilterDTO.Id;
            OrganizationFilter.Code = GeneralKpi_OrganizationFilterDTO.Code;
            OrganizationFilter.Name = GeneralKpi_OrganizationFilterDTO.Name;
            OrganizationFilter.ParentId = GeneralKpi_OrganizationFilterDTO.ParentId;
            OrganizationFilter.Path = GeneralKpi_OrganizationFilterDTO.Path;
            OrganizationFilter.Level = GeneralKpi_OrganizationFilterDTO.Level;
            OrganizationFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };
            OrganizationFilter.Phone = GeneralKpi_OrganizationFilterDTO.Phone;
            OrganizationFilter.Email = GeneralKpi_OrganizationFilterDTO.Email;
            OrganizationFilter.Address = GeneralKpi_OrganizationFilterDTO.Address;

            List<Organization> Organizations = await OrganizationService.List(OrganizationFilter);
            List<GeneralKpi_OrganizationDTO> GeneralKpi_OrganizationDTOs = Organizations
                .Select(x => new GeneralKpi_OrganizationDTO(x)).ToList();
            return GeneralKpi_OrganizationDTOs;
        }
        [Route(GeneralKpiRoute.SingleListStatus), HttpPost]
        public async Task<List<GeneralKpi_StatusDTO>> SingleListStatus([FromBody] GeneralKpi_StatusFilterDTO GeneralKpi_StatusFilterDTO)
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
            List<GeneralKpi_StatusDTO> GeneralKpi_StatusDTOs = Statuses
                .Select(x => new GeneralKpi_StatusDTO(x)).ToList();
            return GeneralKpi_StatusDTOs;
        }

    }
}

