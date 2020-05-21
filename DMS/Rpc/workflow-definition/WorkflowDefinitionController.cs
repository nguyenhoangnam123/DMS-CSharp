using Common;
using DMS.Entities;
using DMS.Services.MRole;
using DMS.Services.MWorkflow;
using DMS.Services.MWorkflowDirection;
using DMS.Services.MWorkflowParameter;
using DMS.Services.MWorkflowStep;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinitionRoute : Root
    {
        public const string Master = Module + "/workflow-definition/workflow-definition-master";
        public const string Detail = Module + "/workflow-definition/workflow-definition-detail";
        private const string Default = Rpc + Module + "/workflow-definition";
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
        
        public const string FilterListWorkflowType = Default + "/filter-list-workflow-type";
        public const string FilterListWorkflowDirection = Default + "/filter-list-workflow-direction";
        public const string FilterListWorkflowStep = Default + "/filter-list-workflow-step";
        public const string FilterListWorkflowParameter = Default + "/filter-list-workflow-parameter";
        public const string FilterListRole = Default + "/filter-list-role";
        
        public const string SingleListWorkflowType = Default + "/single-list-workflow-type";
        public const string SingleListWorkflowDirection = Default + "/single-list-workflow-direction";
        public const string SingleListWorkflowStep = Default + "/single-list-workflow-step";
        public const string SingleListWorkflowParameter = Default + "/single-list-workflow-parameter";
        public const string SingleListRole = Default + "/single-list-role";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WorkflowDefinitionFilter.Id), FieldType.ID },
            { nameof(WorkflowDefinitionFilter.Code), FieldType.STRING },
            { nameof(WorkflowDefinitionFilter.Name), FieldType.STRING },
            { nameof(WorkflowDefinitionFilter.WorkflowTypeId), FieldType.ID },
            { nameof(WorkflowDefinitionFilter.StartDate), FieldType.DATE },
            { nameof(WorkflowDefinitionFilter.EndDate), FieldType.DATE },
            { nameof(WorkflowDefinitionFilter.StatusId), FieldType.ID },
            { nameof(WorkflowDefinitionFilter.UpdatedAt), FieldType.DATE },
        };
    }

    public class WorkflowDefinitionController : RpcController
    {
        private IWorkflowTypeService WorkflowTypeService;
        private IWorkflowDirectionService WorkflowDirectionService;
        private IWorkflowStepService WorkflowStepService;
        private IWorkflowParameterService WorkflowParameterService;
        private IRoleService RoleService;
        private IWorkflowDefinitionService WorkflowDefinitionService;
        private ICurrentContext CurrentContext;
        public WorkflowDefinitionController(
            IWorkflowTypeService WorkflowTypeService,
            IWorkflowDirectionService WorkflowDirectionService,
            IWorkflowStepService WorkflowStepService,
            IWorkflowParameterService WorkflowParameterService,
            IRoleService RoleService,
            IWorkflowDefinitionService WorkflowDefinitionService,
            ICurrentContext CurrentContext
        )
        {
            this.WorkflowTypeService = WorkflowTypeService;
            this.WorkflowDirectionService = WorkflowDirectionService;
            this.WorkflowStepService = WorkflowStepService;
            this.WorkflowParameterService = WorkflowParameterService;
            this.RoleService = RoleService;
            this.WorkflowDefinitionService = WorkflowDefinitionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WorkflowDefinitionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] WorkflowDefinition_WorkflowDefinitionFilterDTO WorkflowDefinition_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = ConvertFilterDTOToFilterEntity(WorkflowDefinition_WorkflowDefinitionFilterDTO);
            WorkflowDefinitionFilter = WorkflowDefinitionService.ToFilter(WorkflowDefinitionFilter);
            int count = await WorkflowDefinitionService.Count(WorkflowDefinitionFilter);
            return count;
        }

        [Route(WorkflowDefinitionRoute.List), HttpPost]
        public async Task<ActionResult<List<WorkflowDefinition_WorkflowDefinitionDTO>>> List([FromBody] WorkflowDefinition_WorkflowDefinitionFilterDTO WorkflowDefinition_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = ConvertFilterDTOToFilterEntity(WorkflowDefinition_WorkflowDefinitionFilterDTO);
            WorkflowDefinitionFilter = WorkflowDefinitionService.ToFilter(WorkflowDefinitionFilter);
            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowDefinition_WorkflowDefinitionDTO> WorkflowDefinition_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(c => new WorkflowDefinition_WorkflowDefinitionDTO(c)).ToList();
            return WorkflowDefinition_WorkflowDefinitionDTOs;
        }

        [Route(WorkflowDefinitionRoute.Get), HttpPost]
        public async Task<ActionResult<WorkflowDefinition_WorkflowDefinitionDTO>> Get([FromBody]WorkflowDefinition_WorkflowDefinitionDTO WorkflowDefinition_WorkflowDefinitionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowDefinition_WorkflowDefinitionDTO.Id))
                return Forbid();

            WorkflowDefinition WorkflowDefinition = await WorkflowDefinitionService.Get(WorkflowDefinition_WorkflowDefinitionDTO.Id);
            return new WorkflowDefinition_WorkflowDefinitionDTO(WorkflowDefinition);
        }

        [Route(WorkflowDefinitionRoute.Create), HttpPost]
        public async Task<ActionResult<WorkflowDefinition_WorkflowDefinitionDTO>> Create([FromBody] WorkflowDefinition_WorkflowDefinitionDTO WorkflowDefinition_WorkflowDefinitionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowDefinition_WorkflowDefinitionDTO.Id))
                return Forbid();

            WorkflowDefinition WorkflowDefinition = ConvertDTOToEntity(WorkflowDefinition_WorkflowDefinitionDTO);
            WorkflowDefinition = await WorkflowDefinitionService.Create(WorkflowDefinition);
            WorkflowDefinition_WorkflowDefinitionDTO = new WorkflowDefinition_WorkflowDefinitionDTO(WorkflowDefinition);
            if (WorkflowDefinition.IsValidated)
                return WorkflowDefinition_WorkflowDefinitionDTO;
            else
                return BadRequest(WorkflowDefinition_WorkflowDefinitionDTO);
        }

        [Route(WorkflowDefinitionRoute.Update), HttpPost]
        public async Task<ActionResult<WorkflowDefinition_WorkflowDefinitionDTO>> Update([FromBody] WorkflowDefinition_WorkflowDefinitionDTO WorkflowDefinition_WorkflowDefinitionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowDefinition_WorkflowDefinitionDTO.Id))
                return Forbid();

            WorkflowDefinition WorkflowDefinition = ConvertDTOToEntity(WorkflowDefinition_WorkflowDefinitionDTO);
            WorkflowDefinition = await WorkflowDefinitionService.Update(WorkflowDefinition);
            WorkflowDefinition_WorkflowDefinitionDTO = new WorkflowDefinition_WorkflowDefinitionDTO(WorkflowDefinition);
            if (WorkflowDefinition.IsValidated)
                return WorkflowDefinition_WorkflowDefinitionDTO;
            else
                return BadRequest(WorkflowDefinition_WorkflowDefinitionDTO);
        }

        [Route(WorkflowDefinitionRoute.Delete), HttpPost]
        public async Task<ActionResult<WorkflowDefinition_WorkflowDefinitionDTO>> Delete([FromBody] WorkflowDefinition_WorkflowDefinitionDTO WorkflowDefinition_WorkflowDefinitionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowDefinition_WorkflowDefinitionDTO.Id))
                return Forbid();

            WorkflowDefinition WorkflowDefinition = ConvertDTOToEntity(WorkflowDefinition_WorkflowDefinitionDTO);
            WorkflowDefinition = await WorkflowDefinitionService.Delete(WorkflowDefinition);
            WorkflowDefinition_WorkflowDefinitionDTO = new WorkflowDefinition_WorkflowDefinitionDTO(WorkflowDefinition);
            if (WorkflowDefinition.IsValidated)
                return WorkflowDefinition_WorkflowDefinitionDTO;
            else
                return BadRequest(WorkflowDefinition_WorkflowDefinitionDTO);
        }
        
        [Route(WorkflowDefinitionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter = WorkflowDefinitionService.ToFilter(WorkflowDefinitionFilter);
            WorkflowDefinitionFilter.Id = new IdFilter { In = Ids };
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.Id;
            WorkflowDefinitionFilter.Skip = 0;
            WorkflowDefinitionFilter.Take = int.MaxValue;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            WorkflowDefinitions = await WorkflowDefinitionService.BulkDelete(WorkflowDefinitions);
            return true;
        }
        
        [Route(WorkflowDefinitionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            WorkflowTypeFilter WorkflowTypeFilter = new WorkflowTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WorkflowTypeSelect.ALL
            };
            List<WorkflowType> WorkflowTypes = await WorkflowTypeService.List(WorkflowTypeFilter);
            List<WorkflowDefinition> WorkflowDefinitions = new List<WorkflowDefinition>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(WorkflowDefinitions);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int WorkflowTypeIdColumn = 2 + StartColumn;
                int StartDateColumn = 3 + StartColumn;
                int EndDateColumn = 4 + StartColumn;
                int StatusIdColumn = 5 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string WorkflowTypeIdValue = worksheet.Cells[i + StartRow, WorkflowTypeIdColumn].Value?.ToString();
                    string StartDateValue = worksheet.Cells[i + StartRow, StartDateColumn].Value?.ToString();
                    string EndDateValue = worksheet.Cells[i + StartRow, EndDateColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    
                    WorkflowDefinition WorkflowDefinition = new WorkflowDefinition();
                    WorkflowDefinition.Name = NameValue;
                    WorkflowDefinition.StartDate = DateTime.TryParse(StartDateValue, out DateTime StartDate) ? StartDate : DateTime.Now;
                    WorkflowDefinition.EndDate = DateTime.TryParse(EndDateValue, out DateTime EndDate) ? EndDate : DateTime.Now;
                    WorkflowType WorkflowType = WorkflowTypes.Where(x => x.Id.ToString() == WorkflowTypeIdValue).FirstOrDefault();
                    WorkflowDefinition.WorkflowTypeId = WorkflowType == null ? 0 : WorkflowType.Id;
                    WorkflowDefinition.WorkflowType = WorkflowType;
                    
                    WorkflowDefinitions.Add(WorkflowDefinition);
                }
            }
            WorkflowDefinitions = await WorkflowDefinitionService.Import(WorkflowDefinitions);
            if (WorkflowDefinitions.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < WorkflowDefinitions.Count; i++)
                {
                    WorkflowDefinition WorkflowDefinition = WorkflowDefinitions[i];
                    if (!WorkflowDefinition.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.Id)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.Id)];
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.Name)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.Name)];
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.WorkflowTypeId)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.WorkflowTypeId)];
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.StartDate)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.StartDate)];
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.EndDate)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.EndDate)];
                        if (WorkflowDefinition.Errors.ContainsKey(nameof(WorkflowDefinition.StatusId)))
                            Error += WorkflowDefinition.Errors[nameof(WorkflowDefinition.StatusId)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(WorkflowDefinitionRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] WorkflowDefinition_WorkflowDefinitionFilterDTO WorkflowDefinition_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowDefinition
                var WorkflowDefinitionFilter = ConvertFilterDTOToFilterEntity(WorkflowDefinition_WorkflowDefinitionFilterDTO);
                WorkflowDefinitionFilter.Skip = 0;
                WorkflowDefinitionFilter.Take = int.MaxValue;
                WorkflowDefinitionFilter = WorkflowDefinitionService.ToFilter(WorkflowDefinitionFilter);
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
                
                #region WorkflowType
                var WorkflowTypeFilter = new WorkflowTypeFilter();
                WorkflowTypeFilter.Selects = WorkflowTypeSelect.ALL;
                WorkflowTypeFilter.OrderBy = WorkflowTypeOrder.Id;
                WorkflowTypeFilter.OrderType = OrderType.ASC;
                WorkflowTypeFilter.Skip = 0;
                WorkflowTypeFilter.Take = int.MaxValue;
                List<WorkflowType> WorkflowTypes = await WorkflowTypeService.List(WorkflowTypeFilter);

                var WorkflowTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> WorkflowTypeData = new List<object[]>();
                for (int i = 0; i < WorkflowTypes.Count; i++)
                {
                    var WorkflowType = WorkflowTypes[i];
                    WorkflowTypeData.Add(new Object[]
                    {
                        WorkflowType.Id,
                        WorkflowType.Code,
                        WorkflowType.Name,
                    });
                }
                excel.GenerateWorksheet("WorkflowType", WorkflowTypeHeaders, WorkflowTypeData);
                #endregion
                #region WorkflowDirection
                var WorkflowDirectionFilter = new WorkflowDirectionFilter();
                WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.ALL;
                WorkflowDirectionFilter.OrderBy = WorkflowDirectionOrder.Id;
                WorkflowDirectionFilter.OrderType = OrderType.ASC;
                WorkflowDirectionFilter.Skip = 0;
                WorkflowDirectionFilter.Take = int.MaxValue;
                List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);

                var WorkflowDirectionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "FromStepId",
                        "ToStepId",
                        "SubjectMailForCreator",
                        "SubjectMailForNextStep",
                        "BodyMailForCreator",
                        "BodyMailForNextStep",
                    }
                };
                List<object[]> WorkflowDirectionData = new List<object[]>();
                for (int i = 0; i < WorkflowDirections.Count; i++)
                {
                    var WorkflowDirection = WorkflowDirections[i];
                    WorkflowDirectionData.Add(new Object[]
                    {
                        WorkflowDirection.Id,
                        WorkflowDirection.WorkflowDefinitionId,
                        WorkflowDirection.FromStepId,
                        WorkflowDirection.ToStepId,
                        WorkflowDirection.SubjectMailForCreator,
                        WorkflowDirection.SubjectMailForNextStep,
                        WorkflowDirection.BodyMailForCreator,
                        WorkflowDirection.BodyMailForNextStep,
                    });
                }
                excel.GenerateWorksheet("WorkflowDirection", WorkflowDirectionHeaders, WorkflowDirectionData);
                #endregion
                #region WorkflowStep
                var WorkflowStepFilter = new WorkflowStepFilter();
                WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
                WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
                WorkflowStepFilter.OrderType = OrderType.ASC;
                WorkflowStepFilter.Skip = 0;
                WorkflowStepFilter.Take = int.MaxValue;
                List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);

                var WorkflowStepHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "Name",
                        "RoleId",
                        "SubjectMailForReject",
                        "BodyMailForReject",
                    }
                };
                List<object[]> WorkflowStepData = new List<object[]>();
                for (int i = 0; i < WorkflowSteps.Count; i++)
                {
                    var WorkflowStep = WorkflowSteps[i];
                    WorkflowStepData.Add(new Object[]
                    {
                        WorkflowStep.Id,
                        WorkflowStep.WorkflowDefinitionId,
                        WorkflowStep.Name,
                        WorkflowStep.RoleId,
                        WorkflowStep.SubjectMailForReject,
                        WorkflowStep.BodyMailForReject,
                    });
                }
                excel.GenerateWorksheet("WorkflowStep", WorkflowStepHeaders, WorkflowStepData);
                #endregion
                #region WorkflowParameter
                var WorkflowParameterFilter = new WorkflowParameterFilter();
                WorkflowParameterFilter.Selects = WorkflowParameterSelect.ALL;
                WorkflowParameterFilter.OrderBy = WorkflowParameterOrder.Id;
                WorkflowParameterFilter.OrderType = OrderType.ASC;
                WorkflowParameterFilter.Skip = 0;
                WorkflowParameterFilter.Take = int.MaxValue;
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
                #region Role
                var RoleFilter = new RoleFilter();
                RoleFilter.Selects = RoleSelect.ALL;
                RoleFilter.OrderBy = RoleOrder.Id;
                RoleFilter.OrderType = OrderType.ASC;
                RoleFilter.Skip = 0;
                RoleFilter.Take = int.MaxValue;
                List<Role> Roles = await RoleService.List(RoleFilter);

                var RoleHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                    }
                };
                List<object[]> RoleData = new List<object[]>();
                for (int i = 0; i < Roles.Count; i++)
                {
                    var Role = Roles[i];
                    RoleData.Add(new Object[]
                    {
                        Role.Id,
                        Role.Code,
                        Role.Name,
                        Role.StatusId,
                    });
                }
                excel.GenerateWorksheet("Role", RoleHeaders, RoleData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowDefinition.xlsx");
        }

        [Route(WorkflowDefinitionRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] WorkflowDefinition_WorkflowDefinitionFilterDTO WorkflowDefinition_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowDefinition
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
                excel.GenerateWorksheet("WorkflowDefinition", WorkflowDefinitionHeaders, WorkflowDefinitionData);
                #endregion
                
                #region WorkflowType
                var WorkflowTypeFilter = new WorkflowTypeFilter();
                WorkflowTypeFilter.Selects = WorkflowTypeSelect.ALL;
                WorkflowTypeFilter.OrderBy = WorkflowTypeOrder.Id;
                WorkflowTypeFilter.OrderType = OrderType.ASC;
                WorkflowTypeFilter.Skip = 0;
                WorkflowTypeFilter.Take = int.MaxValue;
                List<WorkflowType> WorkflowTypes = await WorkflowTypeService.List(WorkflowTypeFilter);

                var WorkflowTypeHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                    }
                };
                List<object[]> WorkflowTypeData = new List<object[]>();
                for (int i = 0; i < WorkflowTypes.Count; i++)
                {
                    var WorkflowType = WorkflowTypes[i];
                    WorkflowTypeData.Add(new Object[]
                    {
                        WorkflowType.Id,
                        WorkflowType.Code,
                        WorkflowType.Name,
                    });
                }
                excel.GenerateWorksheet("WorkflowType", WorkflowTypeHeaders, WorkflowTypeData);
                #endregion
                #region WorkflowDirection
                var WorkflowDirectionFilter = new WorkflowDirectionFilter();
                WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.ALL;
                WorkflowDirectionFilter.OrderBy = WorkflowDirectionOrder.Id;
                WorkflowDirectionFilter.OrderType = OrderType.ASC;
                WorkflowDirectionFilter.Skip = 0;
                WorkflowDirectionFilter.Take = int.MaxValue;
                List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);

                var WorkflowDirectionHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "FromStepId",
                        "ToStepId",
                        "SubjectMailForCreator",
                        "SubjectMailForNextStep",
                        "BodyMailForCreator",
                        "BodyMailForNextStep",
                    }
                };
                List<object[]> WorkflowDirectionData = new List<object[]>();
                for (int i = 0; i < WorkflowDirections.Count; i++)
                {
                    var WorkflowDirection = WorkflowDirections[i];
                    WorkflowDirectionData.Add(new Object[]
                    {
                        WorkflowDirection.Id,
                        WorkflowDirection.WorkflowDefinitionId,
                        WorkflowDirection.FromStepId,
                        WorkflowDirection.ToStepId,
                        WorkflowDirection.SubjectMailForCreator,
                        WorkflowDirection.SubjectMailForNextStep,
                        WorkflowDirection.BodyMailForCreator,
                        WorkflowDirection.BodyMailForNextStep,
                    });
                }
                excel.GenerateWorksheet("WorkflowDirection", WorkflowDirectionHeaders, WorkflowDirectionData);
                #endregion
                #region WorkflowStep
                var WorkflowStepFilter = new WorkflowStepFilter();
                WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
                WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
                WorkflowStepFilter.OrderType = OrderType.ASC;
                WorkflowStepFilter.Skip = 0;
                WorkflowStepFilter.Take = int.MaxValue;
                List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);

                var WorkflowStepHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "WorkflowDefinitionId",
                        "Name",
                        "RoleId",
                        "SubjectMailForReject",
                        "BodyMailForReject",
                    }
                };
                List<object[]> WorkflowStepData = new List<object[]>();
                for (int i = 0; i < WorkflowSteps.Count; i++)
                {
                    var WorkflowStep = WorkflowSteps[i];
                    WorkflowStepData.Add(new Object[]
                    {
                        WorkflowStep.Id,
                        WorkflowStep.WorkflowDefinitionId,
                        WorkflowStep.Name,
                        WorkflowStep.RoleId,
                        WorkflowStep.SubjectMailForReject,
                        WorkflowStep.BodyMailForReject,
                    });
                }
                excel.GenerateWorksheet("WorkflowStep", WorkflowStepHeaders, WorkflowStepData);
                #endregion
                #region WorkflowParameter
                var WorkflowParameterFilter = new WorkflowParameterFilter();
                WorkflowParameterFilter.Selects = WorkflowParameterSelect.ALL;
                WorkflowParameterFilter.OrderBy = WorkflowParameterOrder.Id;
                WorkflowParameterFilter.OrderType = OrderType.ASC;
                WorkflowParameterFilter.Skip = 0;
                WorkflowParameterFilter.Take = int.MaxValue;
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
                #region Role
                var RoleFilter = new RoleFilter();
                RoleFilter.Selects = RoleSelect.ALL;
                RoleFilter.OrderBy = RoleOrder.Id;
                RoleFilter.OrderType = OrderType.ASC;
                RoleFilter.Skip = 0;
                RoleFilter.Take = int.MaxValue;
                List<Role> Roles = await RoleService.List(RoleFilter);

                var RoleHeaders = new List<string[]>()
                {
                    new string[] { 
                        "Id",
                        "Code",
                        "Name",
                        "StatusId",
                    }
                };
                List<object[]> RoleData = new List<object[]>();
                for (int i = 0; i < Roles.Count; i++)
                {
                    var Role = Roles[i];
                    RoleData.Add(new Object[]
                    {
                        Role.Id,
                        Role.Code,
                        Role.Name,
                        Role.StatusId,
                    });
                }
                excel.GenerateWorksheet("Role", RoleHeaders, RoleData);
                #endregion
                excel.Save();
            }
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowDefinition.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter = WorkflowDefinitionService.ToFilter(WorkflowDefinitionFilter);
            if (Id == 0)
            {

            }
            else
            {
                WorkflowDefinitionFilter.Id = new IdFilter { Equal = Id };
                int count = await WorkflowDefinitionService.Count(WorkflowDefinitionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private WorkflowDefinition ConvertDTOToEntity(WorkflowDefinition_WorkflowDefinitionDTO WorkflowDefinition_WorkflowDefinitionDTO)
        {
            WorkflowDefinition WorkflowDefinition = new WorkflowDefinition();
            WorkflowDefinition.Id = WorkflowDefinition_WorkflowDefinitionDTO.Id;
            WorkflowDefinition.Code = WorkflowDefinition_WorkflowDefinitionDTO.Code;
            WorkflowDefinition.Name = WorkflowDefinition_WorkflowDefinitionDTO.Name;
            WorkflowDefinition.WorkflowTypeId = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowTypeId;
            WorkflowDefinition.StartDate = WorkflowDefinition_WorkflowDefinitionDTO.StartDate;
            WorkflowDefinition.EndDate = WorkflowDefinition_WorkflowDefinitionDTO.EndDate;
            WorkflowDefinition.StatusId = WorkflowDefinition_WorkflowDefinitionDTO.StatusId;
            WorkflowDefinition.UpdatedAt = WorkflowDefinition_WorkflowDefinitionDTO.UpdatedAt;
            WorkflowDefinition.WorkflowType = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowType == null ? null : new WorkflowType
            {
                Id = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowType.Id,
                Code = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowType.Code,
                Name = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowType.Name,
            };
            WorkflowDefinition.WorkflowDirections = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowDirections?
                .Select(x => new WorkflowDirection
                {
                    Id = x.Id,
                    FromStepId = x.FromStepId,
                    ToStepId = x.ToStepId,
                    SubjectMailForCreator = x.SubjectMailForCreator,
                    SubjectMailForNextStep = x.SubjectMailForNextStep,
                    BodyMailForCreator = x.BodyMailForCreator,
                    BodyMailForNextStep = x.BodyMailForNextStep,
                    UpdatedAt = x.UpdatedAt,
                    FromStep = x.FromStep == null ? null : new WorkflowStep
                    {
                        Id = x.FromStep.Id,
                        WorkflowDefinitionId = x.FromStep.WorkflowDefinitionId,
                        Code = x.FromStep.Code,
                        Name = x.FromStep.Name,
                        RoleId = x.FromStep.RoleId,
                        SubjectMailForReject = x.FromStep.SubjectMailForReject,
                        BodyMailForReject = x.FromStep.BodyMailForReject,
                    },
                    ToStep = x.ToStep == null ? null : new WorkflowStep
                    {
                        Id = x.ToStep.Id,
                        WorkflowDefinitionId = x.ToStep.WorkflowDefinitionId,
                        Code = x.ToStep.Code,
                        Name = x.ToStep.Name,
                        RoleId = x.ToStep.RoleId,
                        SubjectMailForReject = x.ToStep.SubjectMailForReject,
                        BodyMailForReject = x.ToStep.BodyMailForReject,
                    },
                }).ToList();
            WorkflowDefinition.WorkflowParameters = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowParameters?
                .Select(x => new WorkflowParameter
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                }).ToList();
            WorkflowDefinition.WorkflowSteps = WorkflowDefinition_WorkflowDefinitionDTO.WorkflowSteps?
                .Select(x => new WorkflowStep
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    RoleId = x.RoleId,
                    SubjectMailForReject = x.SubjectMailForReject,
                    BodyMailForReject = x.BodyMailForReject,
                    Role = x.Role == null ? null : new Role
                    {
                        Id = x.Role.Id,
                        Code = x.Role.Code,
                        Name = x.Role.Name,
                        StatusId = x.Role.StatusId,
                    },
                }).ToList();
            WorkflowDefinition.BaseLanguage = CurrentContext.Language;
            return WorkflowDefinition;
        }

        private WorkflowDefinitionFilter ConvertFilterDTOToFilterEntity(WorkflowDefinition_WorkflowDefinitionFilterDTO WorkflowDefinition_WorkflowDefinitionFilterDTO)
        {
            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
            WorkflowDefinitionFilter.Skip = WorkflowDefinition_WorkflowDefinitionFilterDTO.Skip;
            WorkflowDefinitionFilter.Take = WorkflowDefinition_WorkflowDefinitionFilterDTO.Take;
            WorkflowDefinitionFilter.OrderBy = WorkflowDefinition_WorkflowDefinitionFilterDTO.OrderBy;
            WorkflowDefinitionFilter.OrderType = WorkflowDefinition_WorkflowDefinitionFilterDTO.OrderType;

            WorkflowDefinitionFilter.Id = WorkflowDefinition_WorkflowDefinitionFilterDTO.Id;
            WorkflowDefinitionFilter.Code = WorkflowDefinition_WorkflowDefinitionFilterDTO.Code;
            WorkflowDefinitionFilter.Name = WorkflowDefinition_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowDefinition_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowDefinition_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowDefinition_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowDefinition_WorkflowDefinitionFilterDTO.StatusId;
            WorkflowDefinitionFilter.UpdatedAt = WorkflowDefinition_WorkflowDefinitionFilterDTO.UpdatedAt;
            return WorkflowDefinitionFilter;
        }

        [Route(WorkflowDefinitionRoute.FilterListWorkflowType), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowTypeDTO>> FilterListWorkflowType([FromBody] WorkflowDefinition_WorkflowTypeFilterDTO WorkflowDefinition_WorkflowTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowTypeFilter WorkflowTypeFilter = new WorkflowTypeFilter();
            WorkflowTypeFilter.Skip = 0;
            WorkflowTypeFilter.Take = int.MaxValue;
            WorkflowTypeFilter.Take = 20;
            WorkflowTypeFilter.OrderBy = WorkflowTypeOrder.Id;
            WorkflowTypeFilter.OrderType = OrderType.ASC;
            WorkflowTypeFilter.Selects = WorkflowTypeSelect.ALL;

            List<WorkflowType> WorkflowTypes = await WorkflowTypeService.List(WorkflowTypeFilter);
            List<WorkflowDefinition_WorkflowTypeDTO> WorkflowDefinition_WorkflowTypeDTOs = WorkflowTypes
                .Select(x => new WorkflowDefinition_WorkflowTypeDTO(x)).ToList();
            return WorkflowDefinition_WorkflowTypeDTOs;
        }
        [Route(WorkflowDefinitionRoute.FilterListWorkflowDirection), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowDirectionDTO>> FilterListWorkflowDirection([FromBody] WorkflowDefinition_WorkflowDirectionFilterDTO WorkflowDefinition_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter();
            WorkflowDirectionFilter.Skip = 0;
            WorkflowDirectionFilter.Take = 20;
            WorkflowDirectionFilter.OrderBy = WorkflowDirectionOrder.Id;
            WorkflowDirectionFilter.OrderType = OrderType.ASC;
            WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.ALL;
            WorkflowDirectionFilter.Id = WorkflowDefinition_WorkflowDirectionFilterDTO.Id;
            WorkflowDirectionFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowDirectionFilterDTO.WorkflowDefinitionId;
            WorkflowDirectionFilter.FromStepId = WorkflowDefinition_WorkflowDirectionFilterDTO.FromStepId;
            WorkflowDirectionFilter.ToStepId = WorkflowDefinition_WorkflowDirectionFilterDTO.ToStepId;

            List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);
            List<WorkflowDefinition_WorkflowDirectionDTO> WorkflowDefinition_WorkflowDirectionDTOs = WorkflowDirections
                .Select(x => new WorkflowDefinition_WorkflowDirectionDTO(x)).ToList();
            return WorkflowDefinition_WorkflowDirectionDTOs;
        }
        [Route(WorkflowDefinitionRoute.FilterListWorkflowStep), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowStepDTO>> FilterListWorkflowStep([FromBody] WorkflowDefinition_WorkflowStepFilterDTO WorkflowDefinition_WorkflowStepFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter();
            WorkflowStepFilter.Skip = 0;
            WorkflowStepFilter.Take = 20;
            WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
            WorkflowStepFilter.OrderType = OrderType.ASC;
            WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
            WorkflowStepFilter.Id = WorkflowDefinition_WorkflowStepFilterDTO.Id;
            WorkflowStepFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowStepFilterDTO.WorkflowDefinitionId;
            WorkflowStepFilter.Code = WorkflowDefinition_WorkflowStepFilterDTO.Code;
            WorkflowStepFilter.Name = WorkflowDefinition_WorkflowStepFilterDTO.Name;
            WorkflowStepFilter.RoleId = WorkflowDefinition_WorkflowStepFilterDTO.RoleId;

            List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);
            List<WorkflowDefinition_WorkflowStepDTO> WorkflowDefinition_WorkflowStepDTOs = WorkflowSteps
                .Select(x => new WorkflowDefinition_WorkflowStepDTO(x)).ToList();
            return WorkflowDefinition_WorkflowStepDTOs;
        }
        [Route(WorkflowDefinitionRoute.FilterListWorkflowParameter), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowParameterDTO>> FilterListWorkflowParameter([FromBody] WorkflowDefinition_WorkflowParameterFilterDTO WorkflowDefinition_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterFilter WorkflowParameterFilter = new WorkflowParameterFilter();
            WorkflowParameterFilter.Skip = 0;
            WorkflowParameterFilter.Take = 20;
            WorkflowParameterFilter.OrderBy = WorkflowParameterOrder.Id;
            WorkflowParameterFilter.OrderType = OrderType.ASC;
            WorkflowParameterFilter.Selects = WorkflowParameterSelect.ALL;
            WorkflowParameterFilter.Id = WorkflowDefinition_WorkflowParameterFilterDTO.Id;
            WorkflowParameterFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowParameterFilterDTO.WorkflowDefinitionId;
            WorkflowParameterFilter.Code = WorkflowDefinition_WorkflowParameterFilterDTO.Code;
            WorkflowParameterFilter.Name = WorkflowDefinition_WorkflowParameterFilterDTO.Name;

            List<WorkflowParameter> WorkflowParameters = await WorkflowParameterService.List(WorkflowParameterFilter);
            List<WorkflowDefinition_WorkflowParameterDTO> WorkflowDefinition_WorkflowParameterDTOs = WorkflowParameters
                .Select(x => new WorkflowDefinition_WorkflowParameterDTO(x)).ToList();
            return WorkflowDefinition_WorkflowParameterDTOs;
        }
        [Route(WorkflowDefinitionRoute.FilterListRole), HttpPost]
        public async Task<List<WorkflowDefinition_RoleDTO>> FilterListRole([FromBody] WorkflowDefinition_RoleFilterDTO WorkflowDefinition_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = WorkflowDefinition_RoleFilterDTO.Id;
            RoleFilter.Code = WorkflowDefinition_RoleFilterDTO.Code;
            RoleFilter.Name = WorkflowDefinition_RoleFilterDTO.Name;
            RoleFilter.StatusId = WorkflowDefinition_RoleFilterDTO.StatusId;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<WorkflowDefinition_RoleDTO> WorkflowDefinition_RoleDTOs = Roles
                .Select(x => new WorkflowDefinition_RoleDTO(x)).ToList();
            return WorkflowDefinition_RoleDTOs;
        }

        [Route(WorkflowDefinitionRoute.SingleListWorkflowType), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowTypeDTO>> SingleListWorkflowType([FromBody] WorkflowDefinition_WorkflowTypeFilterDTO WorkflowDefinition_WorkflowTypeFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowTypeFilter WorkflowTypeFilter = new WorkflowTypeFilter();
            WorkflowTypeFilter.Skip = 0;
            WorkflowTypeFilter.Take = int.MaxValue;
            WorkflowTypeFilter.Take = 20;
            WorkflowTypeFilter.OrderBy = WorkflowTypeOrder.Id;
            WorkflowTypeFilter.OrderType = OrderType.ASC;
            WorkflowTypeFilter.Selects = WorkflowTypeSelect.ALL;

            List<WorkflowType> WorkflowTypes = await WorkflowTypeService.List(WorkflowTypeFilter);
            List<WorkflowDefinition_WorkflowTypeDTO> WorkflowDefinition_WorkflowTypeDTOs = WorkflowTypes
                .Select(x => new WorkflowDefinition_WorkflowTypeDTO(x)).ToList();
            return WorkflowDefinition_WorkflowTypeDTOs;
        }
        [Route(WorkflowDefinitionRoute.SingleListWorkflowDirection), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowDirectionDTO>> SingleListWorkflowDirection([FromBody] WorkflowDefinition_WorkflowDirectionFilterDTO WorkflowDefinition_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter();
            WorkflowDirectionFilter.Skip = 0;
            WorkflowDirectionFilter.Take = 20;
            WorkflowDirectionFilter.OrderBy = WorkflowDirectionOrder.Id;
            WorkflowDirectionFilter.OrderType = OrderType.ASC;
            WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.ALL;
            WorkflowDirectionFilter.Id = WorkflowDefinition_WorkflowDirectionFilterDTO.Id;
            WorkflowDirectionFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowDirectionFilterDTO.WorkflowDefinitionId;
            WorkflowDirectionFilter.FromStepId = WorkflowDefinition_WorkflowDirectionFilterDTO.FromStepId;
            WorkflowDirectionFilter.ToStepId = WorkflowDefinition_WorkflowDirectionFilterDTO.ToStepId;

            List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);
            List<WorkflowDefinition_WorkflowDirectionDTO> WorkflowDefinition_WorkflowDirectionDTOs = WorkflowDirections
                .Select(x => new WorkflowDefinition_WorkflowDirectionDTO(x)).ToList();
            return WorkflowDefinition_WorkflowDirectionDTOs;
        }
        [Route(WorkflowDefinitionRoute.SingleListWorkflowStep), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowStepDTO>> SingleListWorkflowStep([FromBody] WorkflowDefinition_WorkflowStepFilterDTO WorkflowDefinition_WorkflowStepFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter();
            WorkflowStepFilter.Skip = 0;
            WorkflowStepFilter.Take = 20;
            WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
            WorkflowStepFilter.OrderType = OrderType.ASC;
            WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
            WorkflowStepFilter.Id = WorkflowDefinition_WorkflowStepFilterDTO.Id;
            WorkflowStepFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowStepFilterDTO.WorkflowDefinitionId;
            WorkflowStepFilter.Code = WorkflowDefinition_WorkflowStepFilterDTO.Code;
            WorkflowStepFilter.Name = WorkflowDefinition_WorkflowStepFilterDTO.Name;
            WorkflowStepFilter.RoleId = WorkflowDefinition_WorkflowStepFilterDTO.RoleId;

            List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);
            List<WorkflowDefinition_WorkflowStepDTO> WorkflowDefinition_WorkflowStepDTOs = WorkflowSteps
                .Select(x => new WorkflowDefinition_WorkflowStepDTO(x)).ToList();
            return WorkflowDefinition_WorkflowStepDTOs;
        }
        [Route(WorkflowDefinitionRoute.SingleListWorkflowParameter), HttpPost]
        public async Task<List<WorkflowDefinition_WorkflowParameterDTO>> SingleListWorkflowParameter([FromBody] WorkflowDefinition_WorkflowParameterFilterDTO WorkflowDefinition_WorkflowParameterFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowParameterFilter WorkflowParameterFilter = new WorkflowParameterFilter();
            WorkflowParameterFilter.Skip = 0;
            WorkflowParameterFilter.Take = 20;
            WorkflowParameterFilter.OrderBy = WorkflowParameterOrder.Id;
            WorkflowParameterFilter.OrderType = OrderType.ASC;
            WorkflowParameterFilter.Selects = WorkflowParameterSelect.ALL;
            WorkflowParameterFilter.Id = WorkflowDefinition_WorkflowParameterFilterDTO.Id;
            WorkflowParameterFilter.WorkflowDefinitionId = WorkflowDefinition_WorkflowParameterFilterDTO.WorkflowDefinitionId;
            WorkflowParameterFilter.Code = WorkflowDefinition_WorkflowParameterFilterDTO.Code;
            WorkflowParameterFilter.Name = WorkflowDefinition_WorkflowParameterFilterDTO.Name;

            List<WorkflowParameter> WorkflowParameters = await WorkflowParameterService.List(WorkflowParameterFilter);
            List<WorkflowDefinition_WorkflowParameterDTO> WorkflowDefinition_WorkflowParameterDTOs = WorkflowParameters
                .Select(x => new WorkflowDefinition_WorkflowParameterDTO(x)).ToList();
            return WorkflowDefinition_WorkflowParameterDTOs;
        }
        [Route(WorkflowDefinitionRoute.SingleListRole), HttpPost]
        public async Task<List<WorkflowDefinition_RoleDTO>> SingleListRole([FromBody] WorkflowDefinition_RoleFilterDTO WorkflowDefinition_RoleFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            RoleFilter RoleFilter = new RoleFilter();
            RoleFilter.Skip = 0;
            RoleFilter.Take = 20;
            RoleFilter.OrderBy = RoleOrder.Id;
            RoleFilter.OrderType = OrderType.ASC;
            RoleFilter.Selects = RoleSelect.ALL;
            RoleFilter.Id = WorkflowDefinition_RoleFilterDTO.Id;
            RoleFilter.Code = WorkflowDefinition_RoleFilterDTO.Code;
            RoleFilter.Name = WorkflowDefinition_RoleFilterDTO.Name;
            RoleFilter.StatusId = WorkflowDefinition_RoleFilterDTO.StatusId;

            List<Role> Roles = await RoleService.List(RoleFilter);
            List<WorkflowDefinition_RoleDTO> WorkflowDefinition_RoleDTOs = Roles
                .Select(x => new WorkflowDefinition_RoleDTO(x)).ToList();
            return WorkflowDefinition_RoleDTOs;
        }

    }
}

