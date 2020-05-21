using Common;
using DMS.Entities;
using DMS.Services.MWorkflow;
using DMS.Services.MWorkflowParameter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.workflow_parameter
{
    public class WorkflowParameterRoute : Root
    {
        public const string Master = Module + "/workflow-parameter/workflow-parameter-master";
        public const string Detail = Module + "/workflow-parameter/workflow-parameter-detail";
        private const string Default = Rpc + Module + "/workflow-parameter";
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
        
        
        public const string FilterListWorkflowDefinition = Default + "/filter-list-workflow-definition";
        

        
        public const string SingleListWorkflowDefinition = Default + "/single-list-workflow-definition";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WorkflowParameterFilter.Id), FieldType.ID },
            { nameof(WorkflowParameterFilter.WorkflowDefinitionId), FieldType.ID },
            { nameof(WorkflowParameterFilter.Code), FieldType.STRING },
            { nameof(WorkflowParameterFilter.Name), FieldType.STRING },
        };
    }

    public class WorkflowParameterController : RpcController
    {
        private IWorkflowDefinitionService WorkflowDefinitionService;
        private IWorkflowParameterService WorkflowParameterService;
        private ICurrentContext CurrentContext;
        public WorkflowParameterController(
            IWorkflowDefinitionService WorkflowDefinitionService,
            IWorkflowParameterService WorkflowParameterService,
            ICurrentContext CurrentContext
        )
        {
            this.WorkflowDefinitionService = WorkflowDefinitionService;
            this.WorkflowParameterService = WorkflowParameterService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WorkflowParameterRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] WorkflowParameter_WorkflowParameterFilterDTO WorkflowParameter_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterFilter WorkflowParameterFilter = ConvertFilterDTOToFilterEntity(WorkflowParameter_WorkflowParameterFilterDTO);
            WorkflowParameterFilter = WorkflowParameterService.ToFilter(WorkflowParameterFilter);
            int count = await WorkflowParameterService.Count(WorkflowParameterFilter);
            return count;
        }

        [Route(WorkflowParameterRoute.List), HttpPost]
        public async Task<ActionResult<List<WorkflowParameter_WorkflowParameterDTO>>> List([FromBody] WorkflowParameter_WorkflowParameterFilterDTO WorkflowParameter_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterFilter WorkflowParameterFilter = ConvertFilterDTOToFilterEntity(WorkflowParameter_WorkflowParameterFilterDTO);
            WorkflowParameterFilter = WorkflowParameterService.ToFilter(WorkflowParameterFilter);
            List<WorkflowParameter> WorkflowParameters = await WorkflowParameterService.List(WorkflowParameterFilter);
            List<WorkflowParameter_WorkflowParameterDTO> WorkflowParameter_WorkflowParameterDTOs = WorkflowParameters
                .Select(c => new WorkflowParameter_WorkflowParameterDTO(c)).ToList();
            return WorkflowParameter_WorkflowParameterDTOs;
        }

        [Route(WorkflowParameterRoute.Get), HttpPost]
        public async Task<ActionResult<WorkflowParameter_WorkflowParameterDTO>> Get([FromBody]WorkflowParameter_WorkflowParameterDTO WorkflowParameter_WorkflowParameterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowParameter_WorkflowParameterDTO.Id))
                return Forbid();

            WorkflowParameter WorkflowParameter = await WorkflowParameterService.Get(WorkflowParameter_WorkflowParameterDTO.Id);
            return new WorkflowParameter_WorkflowParameterDTO(WorkflowParameter);
        }

        [Route(WorkflowParameterRoute.Create), HttpPost]
        public async Task<ActionResult<WorkflowParameter_WorkflowParameterDTO>> Create([FromBody] WorkflowParameter_WorkflowParameterDTO WorkflowParameter_WorkflowParameterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowParameter_WorkflowParameterDTO.Id))
                return Forbid();

            WorkflowParameter WorkflowParameter = ConvertDTOToEntity(WorkflowParameter_WorkflowParameterDTO);
            WorkflowParameter = await WorkflowParameterService.Create(WorkflowParameter);
            WorkflowParameter_WorkflowParameterDTO = new WorkflowParameter_WorkflowParameterDTO(WorkflowParameter);
            if (WorkflowParameter.IsValidated)
                return WorkflowParameter_WorkflowParameterDTO;
            else
                return BadRequest(WorkflowParameter_WorkflowParameterDTO);
        }

        [Route(WorkflowParameterRoute.Update), HttpPost]
        public async Task<ActionResult<WorkflowParameter_WorkflowParameterDTO>> Update([FromBody] WorkflowParameter_WorkflowParameterDTO WorkflowParameter_WorkflowParameterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowParameter_WorkflowParameterDTO.Id))
                return Forbid();

            WorkflowParameter WorkflowParameter = ConvertDTOToEntity(WorkflowParameter_WorkflowParameterDTO);
            WorkflowParameter = await WorkflowParameterService.Update(WorkflowParameter);
            WorkflowParameter_WorkflowParameterDTO = new WorkflowParameter_WorkflowParameterDTO(WorkflowParameter);
            if (WorkflowParameter.IsValidated)
                return WorkflowParameter_WorkflowParameterDTO;
            else
                return BadRequest(WorkflowParameter_WorkflowParameterDTO);
        }

        [Route(WorkflowParameterRoute.Delete), HttpPost]
        public async Task<ActionResult<WorkflowParameter_WorkflowParameterDTO>> Delete([FromBody] WorkflowParameter_WorkflowParameterDTO WorkflowParameter_WorkflowParameterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowParameter_WorkflowParameterDTO.Id))
                return Forbid();

            WorkflowParameter WorkflowParameter = ConvertDTOToEntity(WorkflowParameter_WorkflowParameterDTO);
            WorkflowParameter = await WorkflowParameterService.Delete(WorkflowParameter);
            WorkflowParameter_WorkflowParameterDTO = new WorkflowParameter_WorkflowParameterDTO(WorkflowParameter);
            if (WorkflowParameter.IsValidated)
                return WorkflowParameter_WorkflowParameterDTO;
            else
                return BadRequest(WorkflowParameter_WorkflowParameterDTO);
        }
        
        [Route(WorkflowParameterRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterFilter WorkflowParameterFilter = new WorkflowParameterFilter();
            WorkflowParameterFilter = WorkflowParameterService.ToFilter(WorkflowParameterFilter);
            WorkflowParameterFilter.Id = new IdFilter { In = Ids };
            WorkflowParameterFilter.Selects = WorkflowParameterSelect.Id;
            WorkflowParameterFilter.Skip = 0;
            WorkflowParameterFilter.Take = int.MaxValue;

            List<WorkflowParameter> WorkflowParameters = await WorkflowParameterService.List(WorkflowParameterFilter);
            WorkflowParameters = await WorkflowParameterService.BulkDelete(WorkflowParameters);
            return true;
        }
        
        [Route(WorkflowParameterRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WorkflowDefinitionSelect.ALL
            };
            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowParameter> WorkflowParameters = new List<WorkflowParameter>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(WorkflowParameters);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int WorkflowDefinitionIdColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string WorkflowDefinitionIdValue = worksheet.Cells[i + StartRow, WorkflowDefinitionIdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    
                    WorkflowParameter WorkflowParameter = new WorkflowParameter();
                    WorkflowParameter.Name = NameValue;
                    WorkflowDefinition WorkflowDefinition = WorkflowDefinitions.Where(x => x.Id.ToString() == WorkflowDefinitionIdValue).FirstOrDefault();
                    WorkflowParameter.WorkflowDefinitionId = WorkflowDefinition == null ? 0 : WorkflowDefinition.Id;
                    
                    WorkflowParameters.Add(WorkflowParameter);
                }
            }
            WorkflowParameters = await WorkflowParameterService.Import(WorkflowParameters);
            if (WorkflowParameters.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < WorkflowParameters.Count; i++)
                {
                    WorkflowParameter WorkflowParameter = WorkflowParameters[i];
                    if (!WorkflowParameter.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (WorkflowParameter.Errors.ContainsKey(nameof(WorkflowParameter.Id)))
                            Error += WorkflowParameter.Errors[nameof(WorkflowParameter.Id)];
                        if (WorkflowParameter.Errors.ContainsKey(nameof(WorkflowParameter.WorkflowDefinitionId)))
                            Error += WorkflowParameter.Errors[nameof(WorkflowParameter.WorkflowDefinitionId)];
                        if (WorkflowParameter.Errors.ContainsKey(nameof(WorkflowParameter.Name)))
                            Error += WorkflowParameter.Errors[nameof(WorkflowParameter.Name)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(WorkflowParameterRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] WorkflowParameter_WorkflowParameterFilterDTO WorkflowParameter_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowParameter
                var WorkflowParameterFilter = ConvertFilterDTOToFilterEntity(WorkflowParameter_WorkflowParameterFilterDTO);
                WorkflowParameterFilter.Skip = 0;
                WorkflowParameterFilter.Take = int.MaxValue;
                WorkflowParameterFilter = WorkflowParameterService.ToFilter(WorkflowParameterFilter);
                List<WorkflowParameter> WorkflowParameters = await WorkflowParameterService.List(WorkflowParameterFilter);

                var WorkflowParameterHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "Name",
                    }
                };
                List<object[]> WorkflowParameterData = new List<object[]>();
                for (int i = 0; i < WorkflowParameters.Count; i++)
                {
                    var WorkflowParameter = WorkflowParameters[i];
                    WorkflowParameterData.Add(new Object[]
                    {
                        WorkflowParameter.Id,
                        WorkflowParameter.WorkflowDefinitionId,
                        WorkflowParameter.Name,
                    });
                }
                excel.GenerateWorksheet("WorkflowParameter", WorkflowParameterHeaders, WorkflowParameterData);
                #endregion
                
                #region WorkflowDefinition
                var WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
                WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
                WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
                WorkflowDefinitionFilter.OrderType = OrderType.ASC;
                WorkflowDefinitionFilter.Skip = 0;
                WorkflowDefinitionFilter.Take = int.MaxValue;
                List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);

                var WorkflowDefinitionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "WorkflowTypeId",
                        "StartDate",
                        "EndDate",
                        "StatusId",
                    }
                };
                List<object[]> WorkflowDefinitionData = new List<object[]>();
                for (int i = 0; i < WorkflowDefinitions.Count; i++)
                {
                    var WorkflowDefinition = WorkflowDefinitions[i];
                    WorkflowDefinitionData.Add(new Object[]
                    {
                        WorkflowDefinition.Id,
                        WorkflowDefinition.Name,
                        WorkflowDefinition.WorkflowTypeId,
                        WorkflowDefinition.StartDate,
                        WorkflowDefinition.EndDate,
                        WorkflowDefinition.StatusId,
                    });
                }
                excel.GenerateWorksheet("WorkflowDefinition", WorkflowDefinitionHeaders, WorkflowDefinitionData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowParameter.xlsx");
        }

        [Route(WorkflowParameterRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] WorkflowParameter_WorkflowParameterFilterDTO WorkflowParameter_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowParameter
                var WorkflowParameterHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "Name",
                    }
                };
                List<object[]> WorkflowParameterData = new List<object[]>();
                excel.GenerateWorksheet("WorkflowParameter", WorkflowParameterHeaders, WorkflowParameterData);
                #endregion
                
                #region WorkflowDefinition
                var WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
                WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
                WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
                WorkflowDefinitionFilter.OrderType = OrderType.ASC;
                WorkflowDefinitionFilter.Skip = 0;
                WorkflowDefinitionFilter.Take = int.MaxValue;
                List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);

                var WorkflowDefinitionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Name",
                        "WorkflowTypeId",
                        "StartDate",
                        "EndDate",
                        "StatusId",
                    }
                };
                List<object[]> WorkflowDefinitionData = new List<object[]>();
                for (int i = 0; i < WorkflowDefinitions.Count; i++)
                {
                    var WorkflowDefinition = WorkflowDefinitions[i];
                    WorkflowDefinitionData.Add(new Object[]
                    {
                        WorkflowDefinition.Id,
                        WorkflowDefinition.Name,
                        WorkflowDefinition.WorkflowTypeId,
                        WorkflowDefinition.StartDate,
                        WorkflowDefinition.EndDate,
                        WorkflowDefinition.StatusId,
                    });
                }
                excel.GenerateWorksheet("WorkflowDefinition", WorkflowDefinitionHeaders, WorkflowDefinitionData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowParameter.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            WorkflowParameterFilter WorkflowParameterFilter = new WorkflowParameterFilter();
            WorkflowParameterFilter = WorkflowParameterService.ToFilter(WorkflowParameterFilter);
            if (Id == 0)
            {

            }
            else
            {
                WorkflowParameterFilter.Id = new IdFilter { Equal = Id };
                int count = await WorkflowParameterService.Count(WorkflowParameterFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private WorkflowParameter ConvertDTOToEntity(WorkflowParameter_WorkflowParameterDTO WorkflowParameter_WorkflowParameterDTO)
        {
            WorkflowParameter WorkflowParameter = new WorkflowParameter();
            WorkflowParameter.Id = WorkflowParameter_WorkflowParameterDTO.Id;
            WorkflowParameter.WorkflowDefinitionId = WorkflowParameter_WorkflowParameterDTO.WorkflowDefinitionId;
            WorkflowParameter.Code = WorkflowParameter_WorkflowParameterDTO.Code;
            WorkflowParameter.Name = WorkflowParameter_WorkflowParameterDTO.Name;
            WorkflowParameter.BaseLanguage = CurrentContext.Language;
            return WorkflowParameter;
        }

        private WorkflowParameterFilter ConvertFilterDTOToFilterEntity(WorkflowParameter_WorkflowParameterFilterDTO WorkflowParameter_WorkflowParameterFilterDTO)
        {
            WorkflowParameterFilter WorkflowParameterFilter = new WorkflowParameterFilter();
            WorkflowParameterFilter.Selects = WorkflowParameterSelect.ALL;
            WorkflowParameterFilter.Skip = WorkflowParameter_WorkflowParameterFilterDTO.Skip;
            WorkflowParameterFilter.Take = WorkflowParameter_WorkflowParameterFilterDTO.Take;
            WorkflowParameterFilter.OrderBy = WorkflowParameter_WorkflowParameterFilterDTO.OrderBy;
            WorkflowParameterFilter.OrderType = WorkflowParameter_WorkflowParameterFilterDTO.OrderType;

            WorkflowParameterFilter.Id = WorkflowParameter_WorkflowParameterFilterDTO.Id;
            WorkflowParameterFilter.WorkflowDefinitionId = WorkflowParameter_WorkflowParameterFilterDTO.WorkflowDefinitionId;
            WorkflowParameterFilter.Code = WorkflowParameter_WorkflowParameterFilterDTO.Code;
            WorkflowParameterFilter.Name = WorkflowParameter_WorkflowParameterFilterDTO.Name;
            return WorkflowParameterFilter;
        }

        [Route(WorkflowParameterRoute.FilterListWorkflowDefinition), HttpPost]
        public async Task<List<WorkflowParameter_WorkflowDefinitionDTO>> FilterListWorkflowDefinition([FromBody] WorkflowParameter_WorkflowDefinitionFilterDTO WorkflowParameter_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter.Skip = 0;
            WorkflowDefinitionFilter.Take = 20;
            WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
            WorkflowDefinitionFilter.OrderType = OrderType.ASC;
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
            WorkflowDefinitionFilter.Id = WorkflowParameter_WorkflowDefinitionFilterDTO.Id;
            WorkflowDefinitionFilter.Code = WorkflowParameter_WorkflowDefinitionFilterDTO.Code;
            WorkflowDefinitionFilter.Name = WorkflowParameter_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowParameter_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowParameter_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowParameter_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowParameter_WorkflowDefinitionFilterDTO.StatusId;
            WorkflowDefinitionFilter.UpdatedAt = WorkflowParameter_WorkflowDefinitionFilterDTO.UpdatedAt;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowParameter_WorkflowDefinitionDTO> WorkflowParameter_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(x => new WorkflowParameter_WorkflowDefinitionDTO(x)).ToList();
            return WorkflowParameter_WorkflowDefinitionDTOs;
        }

        [Route(WorkflowParameterRoute.SingleListWorkflowDefinition), HttpPost]
        public async Task<List<WorkflowParameter_WorkflowDefinitionDTO>> SingleListWorkflowDefinition([FromBody] WorkflowParameter_WorkflowDefinitionFilterDTO WorkflowParameter_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter.Skip = 0;
            WorkflowDefinitionFilter.Take = 20;
            WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
            WorkflowDefinitionFilter.OrderType = OrderType.ASC;
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
            WorkflowDefinitionFilter.Id = WorkflowParameter_WorkflowDefinitionFilterDTO.Id;
            WorkflowDefinitionFilter.Code = WorkflowParameter_WorkflowDefinitionFilterDTO.Code;
            WorkflowDefinitionFilter.Name = WorkflowParameter_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowParameter_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowParameter_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowParameter_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowParameter_WorkflowDefinitionFilterDTO.StatusId;
            WorkflowDefinitionFilter.UpdatedAt = WorkflowParameter_WorkflowDefinitionFilterDTO.UpdatedAt;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowParameter_WorkflowDefinitionDTO> WorkflowParameter_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(x => new WorkflowParameter_WorkflowDefinitionDTO(x)).ToList();
            return WorkflowParameter_WorkflowDefinitionDTOs;
        }

    }
}

