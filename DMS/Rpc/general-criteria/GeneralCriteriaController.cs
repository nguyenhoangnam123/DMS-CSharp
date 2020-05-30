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
using DMS.Services.MGeneralCriteria;
using DMS.Services.MGeneralKpi;
using DMS.Services.MStatus;

namespace DMS.Rpc.general_criteria
{
    public class GeneralCriteriaController : RpcController
    {
        private IGeneralKpiService GeneralKpiService;
        private IStatusService StatusService;
        private IGeneralCriteriaService GeneralCriteriaService;
        private ICurrentContext CurrentContext;
        public GeneralCriteriaController(
            IGeneralKpiService GeneralKpiService,
            IStatusService StatusService,
            IGeneralCriteriaService GeneralCriteriaService,
            ICurrentContext CurrentContext
        )
        {
            this.GeneralKpiService = GeneralKpiService;
            this.StatusService = StatusService;
            this.GeneralCriteriaService = GeneralCriteriaService;
            this.CurrentContext = CurrentContext;
        }

        [Route(GeneralCriteriaRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] GeneralCriteria_GeneralCriteriaFilterDTO GeneralCriteria_GeneralCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralCriteriaFilter GeneralCriteriaFilter = ConvertFilterDTOToFilterEntity(GeneralCriteria_GeneralCriteriaFilterDTO);
            GeneralCriteriaFilter = GeneralCriteriaService.ToFilter(GeneralCriteriaFilter);
            int count = await GeneralCriteriaService.Count(GeneralCriteriaFilter);
            return count;
        }

        [Route(GeneralCriteriaRoute.List), HttpPost]
        public async Task<ActionResult<List<GeneralCriteria_GeneralCriteriaDTO>>> List([FromBody] GeneralCriteria_GeneralCriteriaFilterDTO GeneralCriteria_GeneralCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralCriteriaFilter GeneralCriteriaFilter = ConvertFilterDTOToFilterEntity(GeneralCriteria_GeneralCriteriaFilterDTO);
            GeneralCriteriaFilter = GeneralCriteriaService.ToFilter(GeneralCriteriaFilter);
            List<GeneralCriteria> GeneralCriterias = await GeneralCriteriaService.List(GeneralCriteriaFilter);
            List<GeneralCriteria_GeneralCriteriaDTO> GeneralCriteria_GeneralCriteriaDTOs = GeneralCriterias
                .Select(c => new GeneralCriteria_GeneralCriteriaDTO(c)).ToList();
            return GeneralCriteria_GeneralCriteriaDTOs;
        }

        [Route(GeneralCriteriaRoute.Get), HttpPost]
        public async Task<ActionResult<GeneralCriteria_GeneralCriteriaDTO>> Get([FromBody]GeneralCriteria_GeneralCriteriaDTO GeneralCriteria_GeneralCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(GeneralCriteria_GeneralCriteriaDTO.Id))
                return Forbid();

            GeneralCriteria GeneralCriteria = await GeneralCriteriaService.Get(GeneralCriteria_GeneralCriteriaDTO.Id);
            return new GeneralCriteria_GeneralCriteriaDTO(GeneralCriteria);
        }

        [Route(GeneralCriteriaRoute.Create), HttpPost]
        public async Task<ActionResult<GeneralCriteria_GeneralCriteriaDTO>> Create([FromBody] GeneralCriteria_GeneralCriteriaDTO GeneralCriteria_GeneralCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(GeneralCriteria_GeneralCriteriaDTO.Id))
                return Forbid();

            GeneralCriteria GeneralCriteria = ConvertDTOToEntity(GeneralCriteria_GeneralCriteriaDTO);
            GeneralCriteria = await GeneralCriteriaService.Create(GeneralCriteria);
            GeneralCriteria_GeneralCriteriaDTO = new GeneralCriteria_GeneralCriteriaDTO(GeneralCriteria);
            if (GeneralCriteria.IsValidated)
                return GeneralCriteria_GeneralCriteriaDTO;
            else
                return BadRequest(GeneralCriteria_GeneralCriteriaDTO);
        }

        [Route(GeneralCriteriaRoute.Update), HttpPost]
        public async Task<ActionResult<GeneralCriteria_GeneralCriteriaDTO>> Update([FromBody] GeneralCriteria_GeneralCriteriaDTO GeneralCriteria_GeneralCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(GeneralCriteria_GeneralCriteriaDTO.Id))
                return Forbid();

            GeneralCriteria GeneralCriteria = ConvertDTOToEntity(GeneralCriteria_GeneralCriteriaDTO);
            GeneralCriteria = await GeneralCriteriaService.Update(GeneralCriteria);
            GeneralCriteria_GeneralCriteriaDTO = new GeneralCriteria_GeneralCriteriaDTO(GeneralCriteria);
            if (GeneralCriteria.IsValidated)
                return GeneralCriteria_GeneralCriteriaDTO;
            else
                return BadRequest(GeneralCriteria_GeneralCriteriaDTO);
        }

        [Route(GeneralCriteriaRoute.Delete), HttpPost]
        public async Task<ActionResult<GeneralCriteria_GeneralCriteriaDTO>> Delete([FromBody] GeneralCriteria_GeneralCriteriaDTO GeneralCriteria_GeneralCriteriaDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(GeneralCriteria_GeneralCriteriaDTO.Id))
                return Forbid();

            GeneralCriteria GeneralCriteria = ConvertDTOToEntity(GeneralCriteria_GeneralCriteriaDTO);
            GeneralCriteria = await GeneralCriteriaService.Delete(GeneralCriteria);
            GeneralCriteria_GeneralCriteriaDTO = new GeneralCriteria_GeneralCriteriaDTO(GeneralCriteria);
            if (GeneralCriteria.IsValidated)
                return GeneralCriteria_GeneralCriteriaDTO;
            else
                return BadRequest(GeneralCriteria_GeneralCriteriaDTO);
        }
        
        [Route(GeneralCriteriaRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralCriteriaFilter GeneralCriteriaFilter = new GeneralCriteriaFilter();
            GeneralCriteriaFilter = GeneralCriteriaService.ToFilter(GeneralCriteriaFilter);
            GeneralCriteriaFilter.Id = new IdFilter { In = Ids };
            GeneralCriteriaFilter.Selects = GeneralCriteriaSelect.Id;
            GeneralCriteriaFilter.Skip = 0;
            GeneralCriteriaFilter.Take = int.MaxValue;

            List<GeneralCriteria> GeneralCriterias = await GeneralCriteriaService.List(GeneralCriteriaFilter);
            GeneralCriterias = await GeneralCriteriaService.BulkDelete(GeneralCriterias);
            return true;
        }
        
        [Route(GeneralCriteriaRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            List<GeneralCriteria> GeneralCriterias = new List<GeneralCriteria>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(GeneralCriterias);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    
                    GeneralCriteria GeneralCriteria = new GeneralCriteria();
                    GeneralCriteria.Code = CodeValue;
                    GeneralCriteria.Name = NameValue;
                    
                    GeneralCriterias.Add(GeneralCriteria);
                }
            }
            GeneralCriterias = await GeneralCriteriaService.Import(GeneralCriterias);
            if (GeneralCriterias.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < GeneralCriterias.Count; i++)
                {
                    GeneralCriteria GeneralCriteria = GeneralCriterias[i];
                    if (!GeneralCriteria.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (GeneralCriteria.Errors.ContainsKey(nameof(GeneralCriteria.Id)))
                            Error += GeneralCriteria.Errors[nameof(GeneralCriteria.Id)];
                        if (GeneralCriteria.Errors.ContainsKey(nameof(GeneralCriteria.Code)))
                            Error += GeneralCriteria.Errors[nameof(GeneralCriteria.Code)];
                        if (GeneralCriteria.Errors.ContainsKey(nameof(GeneralCriteria.Name)))
                            Error += GeneralCriteria.Errors[nameof(GeneralCriteria.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(GeneralCriteriaRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] GeneralCriteria_GeneralCriteriaFilterDTO GeneralCriteria_GeneralCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region GeneralCriteria
                var GeneralCriteriaFilter = ConvertFilterDTOToFilterEntity(GeneralCriteria_GeneralCriteriaFilterDTO);
                GeneralCriteriaFilter.Skip = 0;
                GeneralCriteriaFilter.Take = int.MaxValue;
                GeneralCriteriaFilter = GeneralCriteriaService.ToFilter(GeneralCriteriaFilter);
                List<GeneralCriteria> GeneralCriterias = await GeneralCriteriaService.List(GeneralCriteriaFilter);

                var GeneralCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> GeneralCriteriaData = new List<object[]>();
                for (int i = 0; i < GeneralCriterias.Count; i++)
                {
                    var GeneralCriteria = GeneralCriterias[i];
                    GeneralCriteriaData.Add(new Object[]
                    {
                        GeneralCriteria.Id,
                        GeneralCriteria.Code,
                        GeneralCriteria.Name,
                    });
                }
                excel.GenerateWorksheet("GeneralCriteria", GeneralCriteriaHeaders, GeneralCriteriaData);
                #endregion
                
                #region GeneralKpi
                var GeneralKpiFilter = new GeneralKpiFilter();
                GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
                GeneralKpiFilter.OrderBy = GeneralKpiOrder.Id;
                GeneralKpiFilter.OrderType = OrderType.ASC;
                GeneralKpiFilter.Skip = 0;
                GeneralKpiFilter.Take = int.MaxValue;
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
            return File(memoryStream.ToArray(), "application/octet-stream", "GeneralCriteria.xlsx");
        }

        [Route(GeneralCriteriaRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] GeneralCriteria_GeneralCriteriaFilterDTO GeneralCriteria_GeneralCriteriaFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region GeneralCriteria
                var GeneralCriteriaHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> GeneralCriteriaData = new List<object[]>();
                excel.GenerateWorksheet("GeneralCriteria", GeneralCriteriaHeaders, GeneralCriteriaData);
                #endregion
                
                #region GeneralKpi
                var GeneralKpiFilter = new GeneralKpiFilter();
                GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
                GeneralKpiFilter.OrderBy = GeneralKpiOrder.Id;
                GeneralKpiFilter.OrderType = OrderType.ASC;
                GeneralKpiFilter.Skip = 0;
                GeneralKpiFilter.Take = int.MaxValue;
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
            return File(memoryStream.ToArray(), "application/octet-stream", "GeneralCriteria.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            GeneralCriteriaFilter GeneralCriteriaFilter = new GeneralCriteriaFilter();
            GeneralCriteriaFilter = GeneralCriteriaService.ToFilter(GeneralCriteriaFilter);
            if (Id == 0)
            {

            }
            else
            {
                GeneralCriteriaFilter.Id = new IdFilter { Equal = Id };
                int count = await GeneralCriteriaService.Count(GeneralCriteriaFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private GeneralCriteria ConvertDTOToEntity(GeneralCriteria_GeneralCriteriaDTO GeneralCriteria_GeneralCriteriaDTO)
        {
            GeneralCriteria GeneralCriteria = new GeneralCriteria();
            GeneralCriteria.Id = GeneralCriteria_GeneralCriteriaDTO.Id;
            GeneralCriteria.Code = GeneralCriteria_GeneralCriteriaDTO.Code;
            GeneralCriteria.Name = GeneralCriteria_GeneralCriteriaDTO.Name;
            GeneralCriteria.GeneralKpiCriteriaMappings = GeneralCriteria_GeneralCriteriaDTO.GeneralKpiCriteriaMappings?
                .Select(x => new GeneralKpiCriteriaMapping
                {
                    GeneralKpiId = x.GeneralKpiId,
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
                    StatusId = x.StatusId,
                    GeneralKpi = x.GeneralKpi == null ? null : new GeneralKpi
                    {
                        Id = x.GeneralKpi.Id,
                        OrganizationId = x.GeneralKpi.OrganizationId,
                        EmployeeId = x.GeneralKpi.EmployeeId,
                        KpiPeriodId = x.GeneralKpi.KpiPeriodId,
                        StatusId = x.GeneralKpi.StatusId,
                        CreatorId = x.GeneralKpi.CreatorId,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToList();
            GeneralCriteria.BaseLanguage = CurrentContext.Language;
            return GeneralCriteria;
        }

        private GeneralCriteriaFilter ConvertFilterDTOToFilterEntity(GeneralCriteria_GeneralCriteriaFilterDTO GeneralCriteria_GeneralCriteriaFilterDTO)
        {
            GeneralCriteriaFilter GeneralCriteriaFilter = new GeneralCriteriaFilter();
            GeneralCriteriaFilter.Selects = GeneralCriteriaSelect.ALL;
            GeneralCriteriaFilter.Skip = GeneralCriteria_GeneralCriteriaFilterDTO.Skip;
            GeneralCriteriaFilter.Take = GeneralCriteria_GeneralCriteriaFilterDTO.Take;
            GeneralCriteriaFilter.OrderBy = GeneralCriteria_GeneralCriteriaFilterDTO.OrderBy;
            GeneralCriteriaFilter.OrderType = GeneralCriteria_GeneralCriteriaFilterDTO.OrderType;

            GeneralCriteriaFilter.Id = GeneralCriteria_GeneralCriteriaFilterDTO.Id;
            GeneralCriteriaFilter.Code = GeneralCriteria_GeneralCriteriaFilterDTO.Code;
            GeneralCriteriaFilter.Name = GeneralCriteria_GeneralCriteriaFilterDTO.Name;
            return GeneralCriteriaFilter;
        }

        [Route(GeneralCriteriaRoute.FilterListGeneralKpi), HttpPost]
        public async Task<List<GeneralCriteria_GeneralKpiDTO>> FilterListGeneralKpi([FromBody] GeneralCriteria_GeneralKpiFilterDTO GeneralCriteria_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter.Skip = 0;
            GeneralKpiFilter.Take = 20;
            GeneralKpiFilter.OrderBy = GeneralKpiOrder.Id;
            GeneralKpiFilter.OrderType = OrderType.ASC;
            GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
            GeneralKpiFilter.Id = GeneralCriteria_GeneralKpiFilterDTO.Id;
            GeneralKpiFilter.OrganizationId = GeneralCriteria_GeneralKpiFilterDTO.OrganizationId;
            GeneralKpiFilter.EmployeeId = GeneralCriteria_GeneralKpiFilterDTO.EmployeeId;
            GeneralKpiFilter.KpiPeriodId = GeneralCriteria_GeneralKpiFilterDTO.KpiPeriodId;
            GeneralKpiFilter.StatusId = GeneralCriteria_GeneralKpiFilterDTO.StatusId;
            GeneralKpiFilter.CreatorId = GeneralCriteria_GeneralKpiFilterDTO.CreatorId;

            List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);
            List<GeneralCriteria_GeneralKpiDTO> GeneralCriteria_GeneralKpiDTOs = GeneralKpis
                .Select(x => new GeneralCriteria_GeneralKpiDTO(x)).ToList();
            return GeneralCriteria_GeneralKpiDTOs;
        }
        [Route(GeneralCriteriaRoute.FilterListStatus), HttpPost]
        public async Task<List<GeneralCriteria_StatusDTO>> FilterListStatus([FromBody] GeneralCriteria_StatusFilterDTO GeneralCriteria_StatusFilterDTO)
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
            List<GeneralCriteria_StatusDTO> GeneralCriteria_StatusDTOs = Statuses
                .Select(x => new GeneralCriteria_StatusDTO(x)).ToList();
            return GeneralCriteria_StatusDTOs;
        }

        [Route(GeneralCriteriaRoute.SingleListGeneralKpi), HttpPost]
        public async Task<List<GeneralCriteria_GeneralKpiDTO>> SingleListGeneralKpi([FromBody] GeneralCriteria_GeneralKpiFilterDTO GeneralCriteria_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter.Skip = 0;
            GeneralKpiFilter.Take = 20;
            GeneralKpiFilter.OrderBy = GeneralKpiOrder.Id;
            GeneralKpiFilter.OrderType = OrderType.ASC;
            GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
            GeneralKpiFilter.Id = GeneralCriteria_GeneralKpiFilterDTO.Id;
            GeneralKpiFilter.OrganizationId = GeneralCriteria_GeneralKpiFilterDTO.OrganizationId;
            GeneralKpiFilter.EmployeeId = GeneralCriteria_GeneralKpiFilterDTO.EmployeeId;
            GeneralKpiFilter.KpiPeriodId = GeneralCriteria_GeneralKpiFilterDTO.KpiPeriodId;
            GeneralKpiFilter.StatusId = GeneralCriteria_GeneralKpiFilterDTO.StatusId;
            GeneralKpiFilter.CreatorId = GeneralCriteria_GeneralKpiFilterDTO.CreatorId;

            List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);
            List<GeneralCriteria_GeneralKpiDTO> GeneralCriteria_GeneralKpiDTOs = GeneralKpis
                .Select(x => new GeneralCriteria_GeneralKpiDTO(x)).ToList();
            return GeneralCriteria_GeneralKpiDTOs;
        }
        [Route(GeneralCriteriaRoute.SingleListStatus), HttpPost]
        public async Task<List<GeneralCriteria_StatusDTO>> SingleListStatus([FromBody] GeneralCriteria_StatusFilterDTO GeneralCriteria_StatusFilterDTO)
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
            List<GeneralCriteria_StatusDTO> GeneralCriteria_StatusDTOs = Statuses
                .Select(x => new GeneralCriteria_StatusDTO(x)).ToList();
            return GeneralCriteria_StatusDTOs;
        }

        [Route(GeneralCriteriaRoute.CountGeneralKpi), HttpPost]
        public async Task<long> CountGeneralKpi([FromBody] GeneralCriteria_GeneralKpiFilterDTO GeneralCriteria_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter.Id = GeneralCriteria_GeneralKpiFilterDTO.Id;
            GeneralKpiFilter.OrganizationId = GeneralCriteria_GeneralKpiFilterDTO.OrganizationId;
            GeneralKpiFilter.EmployeeId = GeneralCriteria_GeneralKpiFilterDTO.EmployeeId;
            GeneralKpiFilter.KpiPeriodId = GeneralCriteria_GeneralKpiFilterDTO.KpiPeriodId;
            GeneralKpiFilter.StatusId = GeneralCriteria_GeneralKpiFilterDTO.StatusId;
            GeneralKpiFilter.CreatorId = GeneralCriteria_GeneralKpiFilterDTO.CreatorId;

            return await GeneralKpiService.Count(GeneralKpiFilter);
        }

        [Route(GeneralCriteriaRoute.ListGeneralKpi), HttpPost]
        public async Task<List<GeneralCriteria_GeneralKpiDTO>> ListGeneralKpi([FromBody] GeneralCriteria_GeneralKpiFilterDTO GeneralCriteria_GeneralKpiFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            GeneralKpiFilter GeneralKpiFilter = new GeneralKpiFilter();
            GeneralKpiFilter.Skip = GeneralCriteria_GeneralKpiFilterDTO.Skip;
            GeneralKpiFilter.Take = GeneralCriteria_GeneralKpiFilterDTO.Take;
            GeneralKpiFilter.OrderBy = GeneralKpiOrder.Id;
            GeneralKpiFilter.OrderType = OrderType.ASC;
            GeneralKpiFilter.Selects = GeneralKpiSelect.ALL;
            GeneralKpiFilter.Id = GeneralCriteria_GeneralKpiFilterDTO.Id;
            GeneralKpiFilter.OrganizationId = GeneralCriteria_GeneralKpiFilterDTO.OrganizationId;
            GeneralKpiFilter.EmployeeId = GeneralCriteria_GeneralKpiFilterDTO.EmployeeId;
            GeneralKpiFilter.KpiPeriodId = GeneralCriteria_GeneralKpiFilterDTO.KpiPeriodId;
            GeneralKpiFilter.StatusId = GeneralCriteria_GeneralKpiFilterDTO.StatusId;
            GeneralKpiFilter.CreatorId = GeneralCriteria_GeneralKpiFilterDTO.CreatorId;

            List<GeneralKpi> GeneralKpis = await GeneralKpiService.List(GeneralKpiFilter);
            List<GeneralCriteria_GeneralKpiDTO> GeneralCriteria_GeneralKpiDTOs = GeneralKpis
                .Select(x => new GeneralCriteria_GeneralKpiDTO(x)).ToList();
            return GeneralCriteria_GeneralKpiDTOs;
        }
    }
}

