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
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using DMS.Enums;
using System.Dynamic;
using NGS.Templater;
using DMS.Services.MKpiGeneralContent;
using System.Text;
using DMS.Repositories;

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
        private IKpiGeneralContentService KpiGeneralContentService;
        private IKpiPeriodService KpiPeriodService;
        private ICurrentContext CurrentContext;
        public KpiGeneralController(
            IAppUserService AppUserService,
            IKpiYearService KpiYearService,
            IOrganizationService OrganizationService,
            IStatusService StatusService,
            IKpiCriteriaGeneralService KpiCriteriaGeneralService,
            IKpiGeneralService KpiGeneralService,
            IKpiGeneralContentService KpiGeneralContentService,
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
            this.KpiGeneralContentService = KpiGeneralContentService;
            this.KpiPeriodService = KpiPeriodService;
            this.CurrentContext = CurrentContext;
        }

        [Route(KpiGeneralRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiGeneralFilter KpiGeneralFilter = ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO);
            KpiGeneralFilter = await KpiGeneralService.ToFilter(KpiGeneralFilter);
            int count = await KpiGeneralService.Count(KpiGeneralFilter);
            return count;
        }

        [Route(KpiGeneralRoute.List), HttpPost]
        public async Task<List<KpiGeneral_KpiGeneralDTO>> List([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            KpiGeneralFilter KpiGeneralFilter = ConvertFilterDTOToFilterEntity(KpiGeneral_KpiGeneralFilterDTO);
            KpiGeneralFilter = await KpiGeneralService.ToFilter(KpiGeneralFilter);
            List<KpiGeneral> KpiGenerals = await KpiGeneralService.List(KpiGeneralFilter);
            List<KpiGeneral_KpiGeneralDTO> KpiGeneral_KpiGeneralDTOs = KpiGenerals
                .Select(c => new KpiGeneral_KpiGeneralDTO(c)).ToList();
            return KpiGeneral_KpiGeneralDTOs;
        }

        [Route(KpiGeneralRoute.Get), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Get([FromBody] KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();
            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL,
            });
            KpiGeneral KpiGeneral = await KpiGeneralService.Get(KpiGeneral_KpiGeneralDTO.Id);

            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            (KpiGeneral_KpiGeneralDTO.CurrentMonth, KpiGeneral_KpiGeneralDTO.CurrentQuarter, KpiGeneral_KpiGeneralDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            foreach (var KpiGeneralContent in KpiGeneral_KpiGeneralDTO.KpiGeneralContents)
            {
                KpiGeneralContent.KpiGeneralContentKpiPeriodMappingEnables = GetPeriodEnables(KpiGeneral_KpiGeneralDTO, KpiPeriods);
            }
            return KpiGeneral_KpiGeneralDTO;
        }

        [Route(KpiGeneralRoute.GetDraft), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> GetDraft([FromBody] KpiGeneral_KpiGeneralDTO kpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long KpiYearId = kpiGeneral_KpiGeneralDTO.KpiYearId == 0 ? StaticParams.DateTimeNow.Year : kpiGeneral_KpiGeneralDTO.KpiYearId;

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
            (KpiGeneral_KpiGeneralDTO.CurrentMonth, KpiGeneral_KpiGeneralDTO.CurrentQuarter, KpiGeneral_KpiGeneralDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            KpiGeneral_KpiGeneralDTO.KpiPeriods = KpiPeriods.Select(x => new KpiGeneral_KpiPeriodDTO(x)).ToList();
            KpiGeneral_KpiGeneralDTO.KpiYearId = KpiYearId;
            KpiGeneral_KpiGeneralDTO.KpiYear = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId)
                .Select(x => new KpiGeneral_KpiYearDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .FirstOrDefault();
            KpiGeneral_KpiGeneralDTO.KpiGeneralContents = KpiCriteriaGenerals.Select(x => new KpiGeneral_KpiGeneralContentDTO
            {
                KpiCriteriaGeneralId = x.Id,
                KpiCriteriaGeneral = new KpiGeneral_KpiCriteriaGeneralDTO(x),
                KpiGeneralContentKpiPeriodMappings = KpiPeriods.ToDictionary(x => x.Id, y => (decimal?)null),
                KpiGeneralContentKpiPeriodMappingEnables = GetPeriodEnables(KpiGeneral_KpiGeneralDTO, KpiPeriods),
                Status = new KpiGeneral_StatusDTO
                {
                    Id = Enums.StatusEnum.ACTIVE.Id,
                    Code = Enums.StatusEnum.ACTIVE.Code,
                    Name = Enums.StatusEnum.ACTIVE.Name
                },
                StatusId = Enums.StatusEnum.ACTIVE.Id,
            }).ToList();
            KpiGeneral_KpiGeneralDTO.Status = new KpiGeneral_StatusDTO
            {
                Code = Enums.StatusEnum.ACTIVE.Code,
                Id = Enums.StatusEnum.ACTIVE.Id,
                Name = Enums.StatusEnum.ACTIVE.Name
            };
            KpiGeneral_KpiGeneralDTO.StatusId = Enums.StatusEnum.ACTIVE.Id;
            return KpiGeneral_KpiGeneralDTO;
        }
        [Route(KpiGeneralRoute.Create), HttpPost]
        public async Task<ActionResult<KpiGeneral_KpiGeneralDTO>> Create([FromBody] KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(KpiGeneral_KpiGeneralDTO.Id))
                return Forbid();

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL,
            });
            KpiGeneral KpiGeneral = ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO);
            KpiGeneral = await KpiGeneralService.Create(KpiGeneral);
            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            (KpiGeneral_KpiGeneralDTO.CurrentMonth, KpiGeneral_KpiGeneralDTO.CurrentQuarter, KpiGeneral_KpiGeneralDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            foreach (var KpiGeneralContent in KpiGeneral_KpiGeneralDTO.KpiGeneralContents)
            {
                KpiGeneralContent.KpiGeneralContentKpiPeriodMappingEnables = GetPeriodEnables(KpiGeneral_KpiGeneralDTO, KpiPeriods);
            }
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

            List<KpiPeriod> KpiPeriods = await KpiPeriodService.List(new KpiPeriodFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiPeriodSelect.ALL,
            });

            KpiGeneral KpiGeneral = ConvertDTOToEntity(KpiGeneral_KpiGeneralDTO);
            KpiGeneral = await KpiGeneralService.Update(KpiGeneral);
            KpiGeneral_KpiGeneralDTO = new KpiGeneral_KpiGeneralDTO(KpiGeneral);
            (KpiGeneral_KpiGeneralDTO.CurrentMonth, KpiGeneral_KpiGeneralDTO.CurrentQuarter, KpiGeneral_KpiGeneralDTO.CurrentYear) = ConvertDateTime(StaticParams.DateTimeNow);
            foreach (var KpiGeneralContent in KpiGeneral_KpiGeneralDTO.KpiGeneralContents)
            {
                KpiGeneralContent.KpiGeneralContentKpiPeriodMappingEnables = GetPeriodEnables(KpiGeneral_KpiGeneralDTO, KpiPeriods);
            }
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
            KpiGeneralFilter = await KpiGeneralService.ToFilter(KpiGeneralFilter);
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
            FileInfo FileInfo = new FileInfo(file.FileName);
            StringBuilder errorContent = new StringBuilder();
            if (!FileInfo.Extension.Equals(".xlsx"))
            {
                errorContent.AppendLine("Định dạng file không hợp lệ");
                return BadRequest(errorContent.ToString());
            }

            var AppUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter EmployeeFilter = new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Username | AppUserSelect.DisplayName,
                OrganizationId = new IdFilter { Equal = AppUser.OrganizationId }
            };
            List<AppUser> Employees = await AppUserService.List(EmployeeFilter);

            List<KpiGeneral> KpiGenerals = new List<KpiGeneral>();
            
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["KPI nhan vien"];

                if (worksheet == null)
                {
                    errorContent.AppendLine("File không đúng biểu mẫu import");
                    return BadRequest(errorContent.ToString());
                }
                int StartColumn = 1;
                int StartRow = 6;
                int UsernameColumn = 0 + StartColumn;
                int DisplayNameColumn = 1 + StartColumn;
                int CriterialColumn = 2 + StartColumn;
                int M1Column = 3 + StartColumn;
                int M2Column = 4 + StartColumn;
                int M3Column = 5 + StartColumn;
                int M4Column = 6 + StartColumn;
                int M5Column = 7 + StartColumn;
                int M6Column = 8 + StartColumn;
                int M7Column = 9 + StartColumn;
                int M8Column = 10 + StartColumn;
                int M9Column = 11 + StartColumn;
                int M10Column = 12 + StartColumn;
                int M11Column = 13 + StartColumn;
                int M12Column = 14 + StartColumn;
                int Q1Column = 15 + StartColumn;
                int Q2Column = 16 + StartColumn;
                int Q3Column = 17 + StartColumn;
                int Q4Column = 18 + StartColumn;
                int YColumn = 19 + StartColumn;

                string KpiYearValue = worksheet.Cells[2, 7].Value?.ToString();
                GenericEnum KpiYear;
                if (long.TryParse(KpiYearValue, out long KpiYearId))
                    KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId).FirstOrDefault();
                else
                {
                    errorContent.AppendLine("Chưa chọn năm Kpi hoặc năm không hợp lệ");
                    return BadRequest(errorContent.ToString());
                }

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    string UsernameValue = worksheet.Cells[i, UsernameColumn].Value?.ToString();
                    string CriterialValue = worksheet.Cells[i, CriterialColumn].Value?.ToString();
                    if (UsernameValue != null && UsernameValue.ToLower() == "END".ToLower())
                        break;
                    else if (!string.IsNullOrWhiteSpace(UsernameValue) && string.IsNullOrWhiteSpace(CriterialValue))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(UsernameValue) && i != worksheet.Dimension.End.Row)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Chưa nhập mã nhân viên");
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(UsernameValue) && i == worksheet.Dimension.End.Row)
                        break;

                    GenericEnum KpiGeneralCriterial;
                    if (!string.IsNullOrWhiteSpace(CriterialValue))
                    {
                        KpiGeneralCriterial = KpiCriteriaGeneralEnum.KpiCriteriaGeneralEnumList.Where(x => x.Name == CriterialValue.Trim()).FirstOrDefault();
                        if(KpiGeneralCriterial == null)
                        {
                            errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Tên chỉ tiêu không hợp lệ");
                            continue;
                        }
                    }
                        
                    string M1Value = worksheet.Cells[i, M1Column].Value?.ToString();
                    string M2Value = worksheet.Cells[i, M2Column].Value?.ToString();
                    string M3Value = worksheet.Cells[i, M3Column].Value?.ToString();
                    string M4Value = worksheet.Cells[i, M4Column].Value?.ToString();
                    string M5Value = worksheet.Cells[i, M5Column].Value?.ToString();
                    string M6Value = worksheet.Cells[i, M6Column].Value?.ToString();
                    string M7Value = worksheet.Cells[i, M7Column].Value?.ToString();
                    string M8Value = worksheet.Cells[i, M8Column].Value?.ToString();
                    string M9Value = worksheet.Cells[i, M9Column].Value?.ToString();
                    string M10Value = worksheet.Cells[i, M10Column].Value?.ToString();
                    string M11Value = worksheet.Cells[i, M11Column].Value?.ToString();
                    string M12Value = worksheet.Cells[i, M12Column].Value?.ToString();
                    string Q1Value = worksheet.Cells[i, Q1Column].Value?.ToString();
                    string Q2Value = worksheet.Cells[i, Q2Column].Value?.ToString();
                    string Q3Value = worksheet.Cells[i, Q3Column].Value?.ToString();
                    string Q4Value = worksheet.Cells[i, Q4Column].Value?.ToString();
                    string YValue = worksheet.Cells[i, YColumn].Value?.ToString();
                    
                    AppUser Employee = Employees.Where(x => x.Username == UsernameValue).FirstOrDefault();
                    if(Employee == null)
                    {
                        errorContent.AppendLine($"Lỗi dòng thứ {i + 1}: Nhân viên không tồn tại");
                        continue;
                    }
                    KpiGeneral KpiGeneral = KpiGenerals.Where(x => x.EmployeeId == Employee.Id).FirstOrDefault();
                    if (KpiGeneral == null)
                    {
                        KpiGeneral = new KpiGeneral();
                        KpiGeneral.KpiGeneralContents = new List<KpiGeneralContent>();
                        KpiGeneral.CreatorId = AppUser == null ? 0 : AppUser.Id;
                        KpiGeneral.Creator = AppUser;
                        KpiGeneral.OrganizationId = AppUser.OrganizationId;
                        KpiGeneral.EmployeeId = Employee == null ? 0 : Employee.Id;
                        KpiGeneral.Employee = Employee;
                        KpiGeneral.KpiYearId = KpiYear == null ? 0 : KpiYear.Id;
                        KpiGeneral.StatusId = StatusEnum.ACTIVE.Id;
                        KpiGenerals.Add(KpiGeneral);
                    }
                    KpiGeneralCriterial = KpiCriteriaGeneralEnum.KpiCriteriaGeneralEnumList.Where(x => x.Name == CriterialValue.Trim()).FirstOrDefault();
                    KpiGeneralContent KpiGeneralContent = KpiGeneral.KpiGeneralContents.Where(x => x.KpiCriteriaGeneralId == KpiGeneralCriterial.Id).FirstOrDefault();
                    if(KpiGeneralContent == null)
                    {
                        KpiGeneralContent = new KpiGeneralContent();
                        KpiGeneralContent.StatusId = StatusEnum.ACTIVE.Id;
                        KpiGeneralContent.KpiCriteriaGeneralId = KpiGeneralCriterial.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings = new List<KpiGeneralContentKpiPeriodMapping>();
                        KpiGeneral.KpiGeneralContents.Add(KpiGeneralContent);
                    }
                    KpiGeneralContent.STT = i;

                    #region Tháng 1
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM1 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH01.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM1 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM1 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM1.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH01.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM1);
                    }
                    if (decimal.TryParse(M1Value, out decimal M1))
                        KpiGeneralContentKpiPeriodMappingM1.Value = M1;
                    else
                        KpiGeneralContentKpiPeriodMappingM1.Value = null;
                    #endregion

                    #region Tháng 2
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM2 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH02.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM2 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM2 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM2.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH02.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM2);
                    }
                    if (decimal.TryParse(M2Value, out decimal M2))
                        KpiGeneralContentKpiPeriodMappingM2.Value = M2;
                    else
                        KpiGeneralContentKpiPeriodMappingM2.Value = null;
                    #endregion

                    #region Tháng 3
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM3 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH03.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM3 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM3 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM3.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH03.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM3);
                    }
                    if (decimal.TryParse(M3Value, out decimal M3))
                        KpiGeneralContentKpiPeriodMappingM3.Value = M3;
                    else
                        KpiGeneralContentKpiPeriodMappingM3.Value = null;
                    #endregion

                    #region Tháng 4
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM4 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH04.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM4 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM4 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM4.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH04.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM4);
                    }
                    if (decimal.TryParse(M4Value, out decimal M4))
                        KpiGeneralContentKpiPeriodMappingM4.Value = M4;
                    else
                        KpiGeneralContentKpiPeriodMappingM4.Value = null;
                    #endregion

                    #region Tháng 5
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM5 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH05.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM5 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM5 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM5.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH05.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM5);
                    }
                    if (decimal.TryParse(M5Value, out decimal M5))
                        KpiGeneralContentKpiPeriodMappingM5.Value = M5;
                    else
                        KpiGeneralContentKpiPeriodMappingM5.Value = null;
                    #endregion

                    #region Tháng 6
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM6 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH06.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM6 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM6 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM6.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH06.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM6);
                    }
                    if (decimal.TryParse(M6Value, out decimal M6))
                        KpiGeneralContentKpiPeriodMappingM6.Value = M6;
                    else
                        KpiGeneralContentKpiPeriodMappingM6.Value = null;
                    #endregion

                    #region Tháng 7
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM7 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH07.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM7 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM7 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM7.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH07.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM7);
                    }
                    if (decimal.TryParse(M7Value, out decimal M7))
                        KpiGeneralContentKpiPeriodMappingM7.Value = M7;
                    else
                        KpiGeneralContentKpiPeriodMappingM7.Value = null;
                    #endregion

                    #region Tháng 8
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM8 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH08.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM8 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM8 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM8.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH08.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM8);
                    }
                    if (decimal.TryParse(M8Value, out decimal M8))
                        KpiGeneralContentKpiPeriodMappingM8.Value = M8;
                    else
                        KpiGeneralContentKpiPeriodMappingM8.Value = null;
                    #endregion

                    #region Tháng 9
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM9 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH09.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM9 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM9 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM9.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH09.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM9);
                    }
                    if (decimal.TryParse(M9Value, out decimal M9))
                        KpiGeneralContentKpiPeriodMappingM9.Value = M9;
                    else
                        KpiGeneralContentKpiPeriodMappingM9.Value = null;
                    #endregion

                    #region Tháng 10
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM10 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH10.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM10 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM10 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM10.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH10.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM10);
                    }
                    if (decimal.TryParse(M10Value, out decimal M10))
                        KpiGeneralContentKpiPeriodMappingM10.Value = M10;
                    else
                        KpiGeneralContentKpiPeriodMappingM10.Value = null;
                    #endregion

                    #region Tháng 11
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM11 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH11.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM11 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM11 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM11.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH11.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM11);
                    }
                    if (decimal.TryParse(M11Value, out decimal M11))
                        KpiGeneralContentKpiPeriodMappingM11.Value = M11;
                    else
                        KpiGeneralContentKpiPeriodMappingM11.Value = null;
                    #endregion

                    #region Tháng 12
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingM12 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_MONTH12.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingM12 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingM12 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingM12.KpiPeriodId = KpiPeriodEnum.PERIOD_MONTH12.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingM12);
                    }
                    if (decimal.TryParse(M12Value, out decimal M12))
                        KpiGeneralContentKpiPeriodMappingM12.Value = M12;
                    else
                        KpiGeneralContentKpiPeriodMappingM12.Value = null;
                    #endregion

                    #region Quý 1
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingQ1 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_QUATER01.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingQ1 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingQ1 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingQ1.KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER01.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingQ1);
                    }
                    if (decimal.TryParse(Q1Value, out decimal Q1))
                        KpiGeneralContentKpiPeriodMappingQ1.Value = Q1;
                    else
                        KpiGeneralContentKpiPeriodMappingQ1.Value = null;
                    #endregion

                    #region Quý 2
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingQ2 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_QUATER02.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingQ2 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingQ2 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingQ2.KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER02.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingQ2);
                    }
                    if (decimal.TryParse(Q2Value, out decimal Q2))
                        KpiGeneralContentKpiPeriodMappingQ2.Value = Q2;
                    else
                        KpiGeneralContentKpiPeriodMappingQ2.Value = null;
                    #endregion

                    #region Quý 3
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingQ3 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_QUATER03.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingQ3 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingQ3 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingQ3.KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER03.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingQ3);
                    }
                    if (decimal.TryParse(Q3Value, out decimal Q3))
                        KpiGeneralContentKpiPeriodMappingQ3.Value = Q3;
                    else
                        KpiGeneralContentKpiPeriodMappingQ3.Value = null;
                    #endregion

                    #region Quý 4
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingQ4 = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_QUATER04.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingQ4 == null)
                    {
                        KpiGeneralContentKpiPeriodMappingQ4 = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingQ4.KpiPeriodId = KpiPeriodEnum.PERIOD_QUATER04.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingQ4);
                    }
                    if (decimal.TryParse(Q4Value, out decimal Q4))
                        KpiGeneralContentKpiPeriodMappingQ4.Value = Q4;
                    else
                        KpiGeneralContentKpiPeriodMappingQ4.Value = null;
                    #endregion

                    #region Năm
                    KpiGeneralContentKpiPeriodMapping
                            KpiGeneralContentKpiPeriodMappingY = KpiGeneralContent.KpiGeneralContentKpiPeriodMappings
                            .Where(x => x.KpiPeriodId == KpiPeriodEnum.PERIOD_YEAR01.Id)
                            .FirstOrDefault();
                    if (KpiGeneralContentKpiPeriodMappingY == null)
                    {
                        KpiGeneralContentKpiPeriodMappingY = new KpiGeneralContentKpiPeriodMapping();
                        KpiGeneralContentKpiPeriodMappingY.KpiPeriodId = KpiPeriodEnum.PERIOD_YEAR01.Id;
                        KpiGeneralContent.KpiGeneralContentKpiPeriodMappings.Add(KpiGeneralContentKpiPeriodMappingY);
                    }
                    if (decimal.TryParse(YValue, out decimal Y))
                        KpiGeneralContentKpiPeriodMappingY.Value = Y;
                    else
                        KpiGeneralContentKpiPeriodMappingY.Value = null;
                    #endregion
                }

                KpiGenerals.ForEach(KpiGeneral =>
                {
                    foreach (var KpiGeneralContent in KpiGeneral.KpiGeneralContents)
                    {
                        bool flag = false;
                        foreach (var item in KpiGeneralContent.KpiGeneralContentKpiPeriodMappings)
                        {
                            if (item.Value != null) flag = true;
                        }
                        if (!flag)
                            errorContent.AppendLine($"Lỗi dòng thứ {KpiGeneralContent.STT}: Chưa nhập chỉ tiêu");
                    }
                });

                if (errorContent.Length > 0)
                    return BadRequest(errorContent.ToString());
            }
            KpiGenerals = await KpiGeneralService.Import(KpiGenerals);
            List<KpiGeneral_KpiGeneralDTO> KpiGeneral_KpiGeneralDTOs = KpiGenerals
                .Select(c => new KpiGeneral_KpiGeneralDTO(c)).ToList();
            return Ok(KpiGeneral_KpiGeneralDTOs);
        }

        [Route(KpiGeneralRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] KpiGeneral_KpiGeneralFilterDTO KpiGeneral_KpiGeneralFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (KpiGeneral_KpiGeneralFilterDTO.KpiYearId.Equal.HasValue == false)
                return BadRequest(new { message = "Chưa chọn năm Kpi" });

            long KpiYearId = KpiGeneral_KpiGeneralFilterDTO.KpiYearId.Equal.Value;
            var KpiYear = KpiYearEnum.KpiYearEnumList.Where(x => x.Id == KpiYearId).FirstOrDefault();

            KpiGeneral_KpiGeneralFilterDTO.Skip = 0;
            KpiGeneral_KpiGeneralFilterDTO.Take = int.MaxValue;
            List<KpiGeneral_KpiGeneralDTO> KpiGeneral_KpiGeneralDTOs = await List(KpiGeneral_KpiGeneralFilterDTO);
            var KpiGeneralIds = KpiGeneral_KpiGeneralDTOs.Select(x => x.Id).ToList();
            List<KpiGeneralContent> KpiGeneralContents = await KpiGeneralContentService.List(new KpiGeneralContentFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiGeneralContentSelect.ALL,
                KpiGeneralId = new IdFilter { In = KpiGeneralIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            });
            List<KpiGeneral_KpiGeneralContentDTO> KpiGeneral_KpiGeneralContentDTOs = KpiGeneralContents
                .Select(x => new KpiGeneral_KpiGeneralContentDTO(x)).ToList();
            List<KpiGeneral_ExportDTO> KpiGeneral_ExportDTOs = new List<KpiGeneral_ExportDTO>();
            foreach (var KpiGeneral in KpiGeneral_KpiGeneralDTOs)
            {
                KpiGeneral_ExportDTO KpiGeneral_ExportDTO = new KpiGeneral_ExportDTO();
                KpiGeneral_ExportDTO.Username = KpiGeneral.Employee.Username;
                KpiGeneral_ExportDTO.DisplayName = KpiGeneral.Employee.DisplayName;

                #region Số lần viếng thăm đại lý
                var NumberOfStoreVisitsContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.NumberOfStoreVisits = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id,
                    Name = KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Name
                };
                if (NumberOfStoreVisitsContent != null)
                {
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M1Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M2Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M3Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M4Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M5Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M6Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M7Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M8Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M9Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M10Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M11Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.M12Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.Q1Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.Q2Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.Q3Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.Q4Value = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.NumberOfStoreVisits.YValue = NumberOfStoreVisitsContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Số đại lý tạo mới
                var NewStoresCreatedContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.NewStoresCreated = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id,
                    Name = KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Name
                };
                if (NewStoresCreatedContent != null)
                {
                    KpiGeneral_ExportDTO.NewStoresCreated.M1Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M2Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M3Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M4Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M5Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M6Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M7Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M8Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M9Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M10Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M11Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.M12Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.Q1Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.Q2Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.Q3Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.Q4Value = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.NewStoresCreated.YValue = NewStoresCreatedContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Số đại lý viếng thăm
                var StoresVisitedContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.STORE_VISITED.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.StoresVisited = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.STORE_VISITED.Id,
                    Name = KpiCriteriaGeneralEnum.STORE_VISITED.Name
                };
                if (StoresVisitedContent != null)
                {
                    KpiGeneral_ExportDTO.StoresVisited.M1Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M2Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M3Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M4Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M5Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M6Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M7Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M8Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M9Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M10Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M11Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.StoresVisited.M12Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.StoresVisited.Q1Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.StoresVisited.Q2Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.StoresVisited.Q3Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.StoresVisited.Q4Value = StoresVisitedContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.StoresVisited.YValue = StoresVisitedContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region SKU/ Đơn hàng gián tiếp
                var SKUIndirectOrderContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.SKUIndirectOrder = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Name
                };
                if (SKUIndirectOrderContent != null)
                {
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M1Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M2Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M3Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M4Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M5Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M6Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M7Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M8Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M9Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M10Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M11Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.M12Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.Q1Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.Q2Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.Q3Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.Q4Value = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.SKUIndirectOrder.YValue = SKUIndirectOrderContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Doanh thu đơn hàng gián tiếp
                var TotalIndirectSalesAmountContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalIndirectSalesAmount = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name
                };
                if (TotalIndirectSalesAmountContent != null)
                {
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M1Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M2Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M3Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M4Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M5Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M6Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M7Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M8Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M9Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M10Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M11Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.M12Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.Q1Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.Q2Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.Q3Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.Q4Value = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectSalesAmount.YValue = TotalIndirectSalesAmountContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Tổng sản lượng đơn hàng gián tiếp
                var TotalIndirectQuantityContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalIndirectQuantity = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Name
                };
                if (TotalIndirectQuantityContent != null)
                {
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M1Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M2Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M3Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M4Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M5Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M6Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M7Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M8Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M9Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M10Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M11Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.M12Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.Q1Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.Q2Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.Q3Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.Q4Value = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectQuantity.YValue = TotalIndirectQuantityContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Số đơn hàng gián tiếp
                var TotalIndirectOrdersContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalIndirectOrders = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Name
                };
                if (TotalIndirectOrdersContent != null)
                {
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M1Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M2Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M3Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M4Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M5Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M6Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M7Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M8Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M9Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M10Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M11Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.M12Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.Q1Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.Q2Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.Q3Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.Q4Value = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalIndirectOrders.YValue = TotalIndirectOrdersContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region SKU/ Đơn hàng trực tiếp
                var SKUDirectOrderContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.SKUDirectOrder = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Name
                };
                if (SKUDirectOrderContent != null)
                {
                    KpiGeneral_ExportDTO.SKUDirectOrder.M1Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M2Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M3Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M4Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M5Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M6Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M7Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M8Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M9Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M10Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M11Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.M12Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.Q1Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.Q2Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.Q3Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.Q4Value = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.SKUDirectOrder.YValue = SKUDirectOrderContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Doanh thu đơn hàng trực tiếp
                var TotalDirectSalesAmountContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalDirectSalesAmount = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Name
                };
                if (TotalDirectSalesAmountContent != null)
                {
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M1Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M2Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M3Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M4Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M5Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M6Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M7Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M8Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M9Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M10Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M11Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.M12Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.Q1Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.Q2Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.Q3Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.Q4Value = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalDirectSalesAmount.YValue = TotalDirectSalesAmountContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Tổng sản lượng đơn hàng trực tiếp
                var TotalDirectQuantityContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalDirectQuantity = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Name
                };
                if (TotalDirectQuantityContent != null)
                {
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M1Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M2Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M3Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M4Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M5Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M6Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M7Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M8Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M9Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M10Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M11Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.M12Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.Q1Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.Q2Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.Q3Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.Q4Value = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalDirectQuantity.YValue = TotalDirectQuantityContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion

                #region Số đơn hàng trực tiếp
                var TotalDirectOrdersContent = KpiGeneral_KpiGeneralContentDTOs
                    .Where(x => x.KpiCriteriaGeneralId == KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id)
                    .Where(x => x.KpiGeneralId == KpiGeneral.Id)
                    .Select(x => x.KpiGeneralContentKpiPeriodMappings)
                    .FirstOrDefault();
                KpiGeneral_ExportDTO.TotalDirectOrders = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Name
                };
                if (TotalDirectOrdersContent != null)
                {
                    KpiGeneral_ExportDTO.TotalDirectOrders.M1Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH01.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M2Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH02.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M3Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH03.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M4Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH04.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M5Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH05.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M6Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH06.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M7Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH07.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M8Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH08.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M9Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH09.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M10Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH10.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M11Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH11.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.M12Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_MONTH12.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.Q1Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER01.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.Q2Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER02.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.Q3Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER03.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.Q4Value = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_QUATER04.Id];
                    KpiGeneral_ExportDTO.TotalDirectOrders.YValue = TotalDirectOrdersContent[KpiPeriodEnum.PERIOD_YEAR01.Id];
                }
                #endregion
                KpiGeneral_ExportDTOs.Add(KpiGeneral_ExportDTO);
            }

            string path = "Templates/Kpi_General_Export.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiYear = KpiYear.Name;
            Data.KpiGenerals = KpiGeneral_ExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };

            return File(output.ToArray(), "application/octet-stream", "KpiGenerals.xlsx");
        }

        [Route(KpiGeneralRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var appUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = int.MaxValue;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.Username | AppUserSelect.DisplayName;
            AppUserFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };
            AppUserFilter.StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id };

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }
            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);

            List<KpiCriteriaGeneral> KpiCriteriaGenerals = await KpiCriteriaGeneralService.List(new KpiCriteriaGeneralFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = KpiCriteriaGeneralSelect.Code | KpiCriteriaGeneralSelect.Name,
                OrderBy = KpiCriteriaGeneralOrder.Id,
                OrderType = OrderType.ASC
            });

            List<KpiGeneral_ExportDTO> KpiGeneral_ExportDTOs = new List<KpiGeneral_ExportDTO>();
            foreach (var AppUser in AppUsers)
            {
                KpiGeneral_ExportDTO KpiGeneral_ExportDTO = new KpiGeneral_ExportDTO();
                KpiGeneral_ExportDTO.Username = AppUser.Username;
                KpiGeneral_ExportDTO.DisplayName = AppUser.DisplayName;

                #region Số lần viếng thăm đại lý
                KpiGeneral_ExportDTO.NumberOfStoreVisits = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Id,
                    Name = KpiCriteriaGeneralEnum.NUMBER_OF_STORE_VISIT.Name
                };
                #endregion

                #region Số đại lý tạo mới
                KpiGeneral_ExportDTO.NewStoresCreated = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Id,
                    Name = KpiCriteriaGeneralEnum.NEW_STORE_CREATED.Name
                };
                #endregion

                #region Số đại lý viếng thăm
                KpiGeneral_ExportDTO.StoresVisited = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.STORE_VISITED.Id,
                    Name = KpiCriteriaGeneralEnum.STORE_VISITED.Name
                };
                #endregion

                #region SKU/ Đơn hàng gián tiếp
                KpiGeneral_ExportDTO.SKUIndirectOrder = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.SKU_INDIRECT_SALES_ORDER.Name
                };
                #endregion

                #region Doanh thu đơn hàng gián tiếp
                KpiGeneral_ExportDTO.TotalIndirectSalesAmount = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_AMOUNT.Name
                };
                #endregion

                #region Tổng sản lượng đơn hàng gián tiếp
                KpiGeneral_ExportDTO.TotalIndirectQuantity = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_QUANTITY.Name
                };
                #endregion

                #region Số đơn hàng gián tiếp
                KpiGeneral_ExportDTO.TotalIndirectOrders = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_INDIRECT_SALES_ORDER.Name
                };
                #endregion

                #region SKU/ Đơn hàng trực tiếp
                KpiGeneral_ExportDTO.SKUDirectOrder = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.SKU_DIRECT_SALES_ORDER.Name
                };
                #endregion

                #region Doanh thu đơn hàng trực tiếp
                KpiGeneral_ExportDTO.TotalDirectSalesAmount = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_AMOUNT.Name
                };
                #endregion

                #region Tổng sản lượng đơn hàng trực tiếp
                KpiGeneral_ExportDTO.TotalDirectQuantity = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_QUANTITY.Name
                };
                #endregion

                #region Số đơn hàng trực tiếp
                KpiGeneral_ExportDTO.TotalDirectOrders = new KpiGeneral_ExportCriterialDTO
                {
                    Id = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Id,
                    Name = KpiCriteriaGeneralEnum.TOTAL_DIRECT_SALES_ORDER.Name
                };
                #endregion
                KpiGeneral_ExportDTOs.Add(KpiGeneral_ExportDTO);
            }

            string path = "Templates/Kpi_General.xlsx";
            byte[] arr = System.IO.File.ReadAllBytes(path);
            MemoryStream input = new MemoryStream(arr);
            MemoryStream output = new MemoryStream();
            dynamic Data = new ExpandoObject();
            Data.KpiGenerals = KpiGeneral_ExportDTOs;
            using (var document = StaticParams.DocumentFactory.Open(input, output, "xlsx"))
            {
                document.Process(Data);
            };
            return File(output.ToArray(), "application/octet-stream", "Template_Kpi_General.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            KpiGeneralFilter KpiGeneralFilter = new KpiGeneralFilter();
            KpiGeneralFilter = await KpiGeneralService.ToFilter(KpiGeneralFilter);
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
            KpiGeneralFilter.AppUserId = KpiGeneral_KpiGeneralFilterDTO.AppUserId;
            KpiGeneralFilter.KpiYearId = KpiGeneral_KpiGeneralFilterDTO.KpiYearId;
            KpiGeneralFilter.StatusId = KpiGeneral_KpiGeneralFilterDTO.StatusId;
            KpiGeneralFilter.CreatorId = KpiGeneral_KpiGeneralFilterDTO.CreatorId;
            KpiGeneralFilter.CreatedAt = KpiGeneral_KpiGeneralFilterDTO.CreatedAt;
            KpiGeneralFilter.UpdatedAt = KpiGeneral_KpiGeneralFilterDTO.UpdatedAt;
            return KpiGeneralFilter;
        }

        private Tuple<GenericEnum, GenericEnum, GenericEnum> ConvertDateTime(DateTime date)
        {
            GenericEnum monthName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum quarterName = Enums.KpiPeriodEnum.PERIOD_MONTH01;
            GenericEnum yearName = Enums.KpiYearEnum.KpiYearEnumList.Where(x => x.Id == StaticParams.DateTimeNow.Year).FirstOrDefault();
            switch (date.Month)
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
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH05;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 6:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH06;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER02;
                    break;
                case 7:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH07;
                    quarterName = Enums.KpiPeriodEnum.PERIOD_QUATER03;
                    break;
                case 8:
                    monthName = Enums.KpiPeriodEnum.PERIOD_MONTH08;
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
            return Tuple.Create(monthName, quarterName, yearName);
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
            AppUserFilter.OrganizationId = KpiGeneral_AppUserFilterDTO.OrganizationId;

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

            List<AppUser> AppUsers = await AppUserService.List(AppUserFilter);
            List<KpiGeneral_AppUserDTO> KpiGeneral_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneral_AppUserDTO(x)).ToList();
            return KpiGeneral_AppUserDTOs;
        }

        [Route(KpiGeneralRoute.FilterListCreator), HttpPost]
        public async Task<List<KpiGeneral_AppUserDTO>> FilterListCreator([FromBody] KpiGeneral_AppUserFilterDTO KpiGeneral_AppUserFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var appUser = await AppUserService.Get(CurrentContext.UserId);

            AppUserFilter AppUserFilter = new AppUserFilter();
            AppUserFilter.Skip = 0;
            AppUserFilter.Take = 20;
            AppUserFilter.OrderBy = AppUserOrder.Id;
            AppUserFilter.OrderType = OrderType.ASC;
            AppUserFilter.Selects = AppUserSelect.ALL;
            AppUserFilter.Id = KpiGeneral_AppUserFilterDTO.Id;
            AppUserFilter.Username = KpiGeneral_AppUserFilterDTO.Username;
            AppUserFilter.DisplayName = KpiGeneral_AppUserFilterDTO.DisplayName;
            AppUserFilter.OrganizationId = new IdFilter { Equal = appUser.OrganizationId };

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

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

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

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            AppUserFilter.Id.In = await FilterAppUser(AppUserService, OrganizationService, CurrentContext);

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

            if (OrganizationFilter.Id == null) OrganizationFilter.Id = new IdFilter();
            OrganizationFilter.Id.In = await FilterOrganization(OrganizationService, CurrentContext);

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

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }

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

            if (AppUserFilter.Id == null) AppUserFilter.Id = new IdFilter();
            {
                if (AppUserFilter.Id.In == null) AppUserFilter.Id.In = new List<long>();
                AppUserFilter.Id.In.AddRange(await FilterAppUser(AppUserService, OrganizationService, CurrentContext));
            }

            List<AppUser> AppUsers = await KpiGeneralService.ListAppUser(AppUserFilter, KpiGeneral_AppUserFilterDTO.KpiYearId);
            List<KpiGeneral_AppUserDTO> KpiGeneral_AppUserDTOs = AppUsers
                .Select(x => new KpiGeneral_AppUserDTO(x)).ToList();
            return KpiGeneral_AppUserDTOs;
        }

        private Dictionary<long, bool> GetPeriodEnables(KpiGeneral_KpiGeneralDTO KpiGeneral_KpiGeneralDTO, List<KpiPeriod> KpiPeriods)
        {
            long CurrentMonth = 100 + StaticParams.DateTimeNow.Month;
            long CurrentQuater = 0;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH01.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH03.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER01.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH04.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH06.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER02.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH07.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH09.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER03.Id;
            if (Enums.KpiPeriodEnum.PERIOD_MONTH10.Id <= CurrentMonth && CurrentMonth <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                CurrentQuater = Enums.KpiPeriodEnum.PERIOD_QUATER04.Id;

            Dictionary<long, bool> KpiGeneralContentKpiPeriodMappingEnables = KpiPeriods.ToDictionary(x => x.Id, y => false);
            foreach (KpiPeriod KpiPeriod in KpiPeriods)
            {
                if (KpiGeneral_KpiGeneralDTO.KpiYearId > StaticParams.DateTimeNow.Year)
                {
                    KpiGeneralContentKpiPeriodMappingEnables[KpiPeriod.Id] = true;
                }
                else if (KpiGeneral_KpiGeneralDTO.KpiYearId == StaticParams.DateTimeNow.Year)
                {
                    KpiGeneralContentKpiPeriodMappingEnables[Enums.KpiPeriodEnum.PERIOD_YEAR01.Id] = true;
                    if (CurrentMonth <= KpiPeriod.Id && KpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_MONTH12.Id)
                        KpiGeneralContentKpiPeriodMappingEnables[KpiPeriod.Id] = true;
                    if (CurrentQuater <= KpiPeriod.Id && KpiPeriod.Id <= Enums.KpiPeriodEnum.PERIOD_QUATER04.Id)
                        KpiGeneralContentKpiPeriodMappingEnables[KpiPeriod.Id] = true;
                }
            }


            return KpiGeneralContentKpiPeriodMappingEnables;
        }
    }
}

