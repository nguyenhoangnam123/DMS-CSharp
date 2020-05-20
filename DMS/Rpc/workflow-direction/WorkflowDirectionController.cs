using Common;
using DMS.Entities;
using DMS.Services.MWorkflow;
using DMS.Services.MWorkflowDirection;
using DMS.Services.MWorkflowStep;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.workflow_direction
{
    public class WorkflowDirectionRoute : Root
    {
        public const string Master = Module + "/workflow-direction/workflow-direction-master";
        public const string Detail = Module + "/workflow-direction/workflow-direction-detail";
        private const string Default = Rpc + Module + "/workflow-direction";
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
        
        
        public const string FilterListWorkflowStep = Default + "/filter-list-workflow-step";
        
        public const string FilterListWorkflowDefinition = Default + "/filter-list-workflow-definition";
        

        
        public const string SingleListWorkflowStep = Default + "/single-list-workflow-step";
        
        public const string SingleListWorkflowDefinition = Default + "/single-list-workflow-definition";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WorkflowDirectionFilter.Id), FieldType.ID },
            { nameof(WorkflowDirectionFilter.WorkflowDefinitionId), FieldType.ID },
            { nameof(WorkflowDirectionFilter.FromStepId), FieldType.ID },
            { nameof(WorkflowDirectionFilter.ToStepId), FieldType.ID },
        };
    }

    public class WorkflowDirectionController : RpcController
    {
        private IWorkflowStepService WorkflowStepService;
        private IWorkflowDefinitionService WorkflowDefinitionService;
        private IWorkflowDirectionService WorkflowDirectionService;
        private ICurrentContext CurrentContext;
        public WorkflowDirectionController(
            IWorkflowStepService WorkflowStepService,
            IWorkflowDefinitionService WorkflowDefinitionService,
            IWorkflowDirectionService WorkflowDirectionService,
            ICurrentContext CurrentContext
        )
        {
            this.WorkflowStepService = WorkflowStepService;
            this.WorkflowDefinitionService = WorkflowDefinitionService;
            this.WorkflowDirectionService = WorkflowDirectionService;
            this.CurrentContext = CurrentContext;
        }

        [Route(WorkflowDirectionRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] WorkflowDirection_WorkflowDirectionFilterDTO WorkflowDirection_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDirectionFilter WorkflowDirectionFilter = ConvertFilterDTOToFilterEntity(WorkflowDirection_WorkflowDirectionFilterDTO);
            WorkflowDirectionFilter = WorkflowDirectionService.ToFilter(WorkflowDirectionFilter);
            int count = await WorkflowDirectionService.Count(WorkflowDirectionFilter);
            return count;
        }

        [Route(WorkflowDirectionRoute.List), HttpPost]
        public async Task<ActionResult<List<WorkflowDirection_WorkflowDirectionDTO>>> List([FromBody] WorkflowDirection_WorkflowDirectionFilterDTO WorkflowDirection_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDirectionFilter WorkflowDirectionFilter = ConvertFilterDTOToFilterEntity(WorkflowDirection_WorkflowDirectionFilterDTO);
            WorkflowDirectionFilter = WorkflowDirectionService.ToFilter(WorkflowDirectionFilter);
            List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);
            List<WorkflowDirection_WorkflowDirectionDTO> WorkflowDirection_WorkflowDirectionDTOs = WorkflowDirections
                .Select(c => new WorkflowDirection_WorkflowDirectionDTO(c)).ToList();
            return WorkflowDirection_WorkflowDirectionDTOs;
        }

        [Route(WorkflowDirectionRoute.Get), HttpPost]
        public async Task<ActionResult<WorkflowDirection_WorkflowDirectionDTO>> Get([FromBody]WorkflowDirection_WorkflowDirectionDTO WorkflowDirection_WorkflowDirectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowDirection_WorkflowDirectionDTO.Id))
                return Forbid();

            WorkflowDirection WorkflowDirection = await WorkflowDirectionService.Get(WorkflowDirection_WorkflowDirectionDTO.Id);
            return new WorkflowDirection_WorkflowDirectionDTO(WorkflowDirection);
        }

        [Route(WorkflowDirectionRoute.Create), HttpPost]
        public async Task<ActionResult<WorkflowDirection_WorkflowDirectionDTO>> Create([FromBody] WorkflowDirection_WorkflowDirectionDTO WorkflowDirection_WorkflowDirectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowDirection_WorkflowDirectionDTO.Id))
                return Forbid();

            WorkflowDirection WorkflowDirection = ConvertDTOToEntity(WorkflowDirection_WorkflowDirectionDTO);
            WorkflowDirection = await WorkflowDirectionService.Create(WorkflowDirection);
            WorkflowDirection_WorkflowDirectionDTO = new WorkflowDirection_WorkflowDirectionDTO(WorkflowDirection);
            if (WorkflowDirection.IsValidated)
                return WorkflowDirection_WorkflowDirectionDTO;
            else
                return BadRequest(WorkflowDirection_WorkflowDirectionDTO);
        }

        [Route(WorkflowDirectionRoute.Update), HttpPost]
        public async Task<ActionResult<WorkflowDirection_WorkflowDirectionDTO>> Update([FromBody] WorkflowDirection_WorkflowDirectionDTO WorkflowDirection_WorkflowDirectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            if (!await HasPermission(WorkflowDirection_WorkflowDirectionDTO.Id))
                return Forbid();

            WorkflowDirection WorkflowDirection = ConvertDTOToEntity(WorkflowDirection_WorkflowDirectionDTO);
            WorkflowDirection = await WorkflowDirectionService.Update(WorkflowDirection);
            WorkflowDirection_WorkflowDirectionDTO = new WorkflowDirection_WorkflowDirectionDTO(WorkflowDirection);
            if (WorkflowDirection.IsValidated)
                return WorkflowDirection_WorkflowDirectionDTO;
            else
                return BadRequest(WorkflowDirection_WorkflowDirectionDTO);
        }

        [Route(WorkflowDirectionRoute.Delete), HttpPost]
        public async Task<ActionResult<WorkflowDirection_WorkflowDirectionDTO>> Delete([FromBody] WorkflowDirection_WorkflowDirectionDTO WorkflowDirection_WorkflowDirectionDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(WorkflowDirection_WorkflowDirectionDTO.Id))
                return Forbid();

            WorkflowDirection WorkflowDirection = ConvertDTOToEntity(WorkflowDirection_WorkflowDirectionDTO);
            WorkflowDirection = await WorkflowDirectionService.Delete(WorkflowDirection);
            WorkflowDirection_WorkflowDirectionDTO = new WorkflowDirection_WorkflowDirectionDTO(WorkflowDirection);
            if (WorkflowDirection.IsValidated)
                return WorkflowDirection_WorkflowDirectionDTO;
            else
                return BadRequest(WorkflowDirection_WorkflowDirectionDTO);
        }
        
        [Route(WorkflowDirectionRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter();
            WorkflowDirectionFilter = WorkflowDirectionService.ToFilter(WorkflowDirectionFilter);
            WorkflowDirectionFilter.Id = new IdFilter { In = Ids };
            WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.Id;
            WorkflowDirectionFilter.Skip = 0;
            WorkflowDirectionFilter.Take = int.MaxValue;

            List<WorkflowDirection> WorkflowDirections = await WorkflowDirectionService.List(WorkflowDirectionFilter);
            WorkflowDirections = await WorkflowDirectionService.BulkDelete(WorkflowDirections);
            return true;
        }
        
        [Route(WorkflowDirectionRoute.Import), HttpPost]
        public async Task<ActionResult> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            WorkflowStepFilter FromStepFilter = new WorkflowStepFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WorkflowStepSelect.ALL
            };
            List<WorkflowStep> FromSteps = await WorkflowStepService.List(FromStepFilter);
            WorkflowStepFilter ToStepFilter = new WorkflowStepFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WorkflowStepSelect.ALL
            };
            List<WorkflowStep> ToSteps = await WorkflowStepService.List(ToStepFilter);
            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WorkflowDefinitionSelect.ALL
            };
            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowDirection> WorkflowDirections = new List<WorkflowDirection>();
            using (ExcelPackage excelPackage = new ExcelPackage(file.OpenReadStream()))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Ok(WorkflowDirections);
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int WorkflowDefinitionIdColumn = 1 + StartColumn;
                int FromStepIdColumn = 2 + StartColumn;
                int ToStepIdColumn = 3 + StartColumn;
                int SubjectMailForCreatorColumn = 4 + StartColumn;
                int SubjectMailForNextStepColumn = 5 + StartColumn;
                int BodyMailForCreatorColumn = 6 + StartColumn;
                int BodyMailForNextStepColumn = 7 + StartColumn;

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, StartColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string WorkflowDefinitionIdValue = worksheet.Cells[i + StartRow, WorkflowDefinitionIdColumn].Value?.ToString();
                    string FromStepIdValue = worksheet.Cells[i + StartRow, FromStepIdColumn].Value?.ToString();
                    string ToStepIdValue = worksheet.Cells[i + StartRow, ToStepIdColumn].Value?.ToString();
                    string SubjectMailForCreatorValue = worksheet.Cells[i + StartRow, SubjectMailForCreatorColumn].Value?.ToString();
                    string SubjectMailForNextStepValue = worksheet.Cells[i + StartRow, SubjectMailForNextStepColumn].Value?.ToString();
                    string BodyMailForCreatorValue = worksheet.Cells[i + StartRow, BodyMailForCreatorColumn].Value?.ToString();
                    string BodyMailForNextStepValue = worksheet.Cells[i + StartRow, BodyMailForNextStepColumn].Value?.ToString();
                    
                    WorkflowDirection WorkflowDirection = new WorkflowDirection();
                    WorkflowDirection.SubjectMailForCreator = SubjectMailForCreatorValue;
                    WorkflowDirection.SubjectMailForNextStep = SubjectMailForNextStepValue;
                    WorkflowDirection.BodyMailForCreator = BodyMailForCreatorValue;
                    WorkflowDirection.BodyMailForNextStep = BodyMailForNextStepValue;
                    WorkflowStep FromStep = FromSteps.Where(x => x.Id.ToString() == FromStepIdValue).FirstOrDefault();
                    WorkflowDirection.FromStepId = FromStep == null ? 0 : FromStep.Id;
                    WorkflowDirection.FromStep = FromStep;
                    WorkflowStep ToStep = ToSteps.Where(x => x.Id.ToString() == ToStepIdValue).FirstOrDefault();
                    WorkflowDirection.ToStepId = ToStep == null ? 0 : ToStep.Id;
                    WorkflowDirection.ToStep = ToStep;
                    WorkflowDefinition WorkflowDefinition = WorkflowDefinitions.Where(x => x.Id.ToString() == WorkflowDefinitionIdValue).FirstOrDefault();
                    WorkflowDirection.WorkflowDefinitionId = WorkflowDefinition == null ? 0 : WorkflowDefinition.Id;
                    WorkflowDirection.WorkflowDefinition = WorkflowDefinition;
                    
                    WorkflowDirections.Add(WorkflowDirection);
                }
            }
            WorkflowDirections = await WorkflowDirectionService.Import(WorkflowDirections);
            if (WorkflowDirections.All(x => x.IsValidated))
                return Ok(true);
            else
            {
                List<string> Errors = new List<string>();
                for (int i = 0; i < WorkflowDirections.Count; i++)
                {
                    WorkflowDirection WorkflowDirection = WorkflowDirections[i];
                    if (!WorkflowDirection.IsValidated)
                    {
                        string Error = $"Dòng {i + 2} có lỗi:";
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.Id)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.Id)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.WorkflowDefinitionId)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.WorkflowDefinitionId)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.FromStepId)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.FromStepId)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.ToStepId)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.ToStepId)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.SubjectMailForCreator)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.SubjectMailForCreator)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.SubjectMailForNextStep)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.SubjectMailForNextStep)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.BodyMailForCreator)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.BodyMailForCreator)];
                        if (WorkflowDirection.Errors.ContainsKey(nameof(WorkflowDirection.BodyMailForNextStep)))
                            Error += WorkflowDirection.Errors[nameof(WorkflowDirection.BodyMailForNextStep)];
                        Errors.Add(Error);
                    }
                }
                return BadRequest(Errors);
            }
        }
        
        [Route(WorkflowDirectionRoute.Export), HttpPost]
        public async Task<FileResult> Export([FromBody] WorkflowDirection_WorkflowDirectionFilterDTO WorkflowDirection_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowDirection
                var WorkflowDirectionFilter = ConvertFilterDTOToFilterEntity(WorkflowDirection_WorkflowDirectionFilterDTO);
                WorkflowDirectionFilter.Skip = 0;
                WorkflowDirectionFilter.Take = int.MaxValue;
                WorkflowDirectionFilter = WorkflowDirectionService.ToFilter(WorkflowDirectionFilter);
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
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowDirection.xlsx");
        }

        [Route(WorkflowDirectionRoute.ExportTemplate), HttpPost]
        public async Task<FileResult> ExportTemplate([FromBody] WorkflowDirection_WorkflowDirectionFilterDTO WorkflowDirection_WorkflowDirectionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(memoryStream))
            {
                #region WorkflowDirection
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
            return File(memoryStream.ToArray(), "application/octet-stream", "WorkflowDirection.xlsx");
        }

        private async Task<bool> HasPermission(long Id)
        {
            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter();
            WorkflowDirectionFilter = WorkflowDirectionService.ToFilter(WorkflowDirectionFilter);
            if (Id == 0)
            {

            }
            else
            {
                WorkflowDirectionFilter.Id = new IdFilter { Equal = Id };
                int count = await WorkflowDirectionService.Count(WorkflowDirectionFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private WorkflowDirection ConvertDTOToEntity(WorkflowDirection_WorkflowDirectionDTO WorkflowDirection_WorkflowDirectionDTO)
        {
            WorkflowDirection WorkflowDirection = new WorkflowDirection();
            WorkflowDirection.Id = WorkflowDirection_WorkflowDirectionDTO.Id;
            WorkflowDirection.WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinitionId;
            WorkflowDirection.FromStepId = WorkflowDirection_WorkflowDirectionDTO.FromStepId;
            WorkflowDirection.ToStepId = WorkflowDirection_WorkflowDirectionDTO.ToStepId;
            WorkflowDirection.SubjectMailForCreator = WorkflowDirection_WorkflowDirectionDTO.SubjectMailForCreator;
            WorkflowDirection.SubjectMailForNextStep = WorkflowDirection_WorkflowDirectionDTO.SubjectMailForNextStep;
            WorkflowDirection.BodyMailForCreator = WorkflowDirection_WorkflowDirectionDTO.BodyMailForCreator;
            WorkflowDirection.BodyMailForNextStep = WorkflowDirection_WorkflowDirectionDTO.BodyMailForNextStep;
            WorkflowDirection.FromStep = WorkflowDirection_WorkflowDirectionDTO.FromStep == null ? null : new WorkflowStep
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.FromStep.Id,
                WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionDTO.FromStep.WorkflowDefinitionId,
                Name = WorkflowDirection_WorkflowDirectionDTO.FromStep.Name,
                RoleId = WorkflowDirection_WorkflowDirectionDTO.FromStep.RoleId,
                SubjectMailForReject = WorkflowDirection_WorkflowDirectionDTO.FromStep.SubjectMailForReject,
                BodyMailForReject = WorkflowDirection_WorkflowDirectionDTO.FromStep.BodyMailForReject,
            };
            WorkflowDirection.ToStep = WorkflowDirection_WorkflowDirectionDTO.ToStep == null ? null : new WorkflowStep
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.ToStep.Id,
                WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionDTO.ToStep.WorkflowDefinitionId,
                Name = WorkflowDirection_WorkflowDirectionDTO.ToStep.Name,
                RoleId = WorkflowDirection_WorkflowDirectionDTO.ToStep.RoleId,
                SubjectMailForReject = WorkflowDirection_WorkflowDirectionDTO.ToStep.SubjectMailForReject,
                BodyMailForReject = WorkflowDirection_WorkflowDirectionDTO.ToStep.BodyMailForReject,
            };
            WorkflowDirection.WorkflowDefinition = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition == null ? null : new WorkflowDefinition
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.Id,
                Name = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.Name,
                WorkflowTypeId = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.WorkflowTypeId,
                StartDate = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.StartDate,
                EndDate = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.EndDate,
                StatusId = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.StatusId,
            };
            WorkflowDirection.BaseLanguage = CurrentContext.Language;
            return WorkflowDirection;
        }

        private WorkflowDirectionFilter ConvertFilterDTOToFilterEntity(WorkflowDirection_WorkflowDirectionFilterDTO WorkflowDirection_WorkflowDirectionFilterDTO)
        {
            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter();
            WorkflowDirectionFilter.Selects = WorkflowDirectionSelect.ALL;
            WorkflowDirectionFilter.Skip = WorkflowDirection_WorkflowDirectionFilterDTO.Skip;
            WorkflowDirectionFilter.Take = WorkflowDirection_WorkflowDirectionFilterDTO.Take;
            WorkflowDirectionFilter.OrderBy = WorkflowDirection_WorkflowDirectionFilterDTO.OrderBy;
            WorkflowDirectionFilter.OrderType = WorkflowDirection_WorkflowDirectionFilterDTO.OrderType;

            WorkflowDirectionFilter.Id = WorkflowDirection_WorkflowDirectionFilterDTO.Id;
            WorkflowDirectionFilter.WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionFilterDTO.WorkflowDefinitionId;
            WorkflowDirectionFilter.FromStepId = WorkflowDirection_WorkflowDirectionFilterDTO.FromStepId;
            WorkflowDirectionFilter.ToStepId = WorkflowDirection_WorkflowDirectionFilterDTO.ToStepId;
            return WorkflowDirectionFilter;
        }

        [Route(WorkflowDirectionRoute.FilterListWorkflowStep), HttpPost]
        public async Task<List<WorkflowDirection_WorkflowStepDTO>> FilterListWorkflowStep([FromBody] WorkflowDirection_WorkflowStepFilterDTO WorkflowDirection_WorkflowStepFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter();
            WorkflowStepFilter.Skip = 0;
            WorkflowStepFilter.Take = 20;
            WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
            WorkflowStepFilter.OrderType = OrderType.ASC;
            WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
            WorkflowStepFilter.Id = WorkflowDirection_WorkflowStepFilterDTO.Id;
            WorkflowStepFilter.WorkflowDefinitionId = WorkflowDirection_WorkflowStepFilterDTO.WorkflowDefinitionId;
            WorkflowStepFilter.Name = WorkflowDirection_WorkflowStepFilterDTO.Name;
            WorkflowStepFilter.RoleId = WorkflowDirection_WorkflowStepFilterDTO.RoleId;

            List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);
            List<WorkflowDirection_WorkflowStepDTO> WorkflowDirection_WorkflowStepDTOs = WorkflowSteps
                .Select(x => new WorkflowDirection_WorkflowStepDTO(x)).ToList();
            return WorkflowDirection_WorkflowStepDTOs;
        }
        [Route(WorkflowDirectionRoute.FilterListWorkflowDefinition), HttpPost]
        public async Task<List<WorkflowDirection_WorkflowDefinitionDTO>> FilterListWorkflowDefinition([FromBody] WorkflowDirection_WorkflowDefinitionFilterDTO WorkflowDirection_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter.Skip = 0;
            WorkflowDefinitionFilter.Take = 20;
            WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
            WorkflowDefinitionFilter.OrderType = OrderType.ASC;
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
            WorkflowDefinitionFilter.Id = WorkflowDirection_WorkflowDefinitionFilterDTO.Id;
            WorkflowDefinitionFilter.Name = WorkflowDirection_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowDirection_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowDirection_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowDirection_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowDirection_WorkflowDefinitionFilterDTO.StatusId;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowDirection_WorkflowDefinitionDTO> WorkflowDirection_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(x => new WorkflowDirection_WorkflowDefinitionDTO(x)).ToList();
            return WorkflowDirection_WorkflowDefinitionDTOs;
        }

        [Route(WorkflowDirectionRoute.SingleListWorkflowStep), HttpPost]
        public async Task<List<WorkflowDirection_WorkflowStepDTO>> SingleListWorkflowStep([FromBody] WorkflowDirection_WorkflowStepFilterDTO WorkflowDirection_WorkflowStepFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter();
            WorkflowStepFilter.Skip = 0;
            WorkflowStepFilter.Take = 20;
            WorkflowStepFilter.OrderBy = WorkflowStepOrder.Id;
            WorkflowStepFilter.OrderType = OrderType.ASC;
            WorkflowStepFilter.Selects = WorkflowStepSelect.ALL;
            WorkflowStepFilter.Id = WorkflowDirection_WorkflowStepFilterDTO.Id;
            WorkflowStepFilter.WorkflowDefinitionId = WorkflowDirection_WorkflowStepFilterDTO.WorkflowDefinitionId;
            WorkflowStepFilter.Name = WorkflowDirection_WorkflowStepFilterDTO.Name;
            WorkflowStepFilter.RoleId = WorkflowDirection_WorkflowStepFilterDTO.RoleId;

            List<WorkflowStep> WorkflowSteps = await WorkflowStepService.List(WorkflowStepFilter);
            List<WorkflowDirection_WorkflowStepDTO> WorkflowDirection_WorkflowStepDTOs = WorkflowSteps
                .Select(x => new WorkflowDirection_WorkflowStepDTO(x)).ToList();
            return WorkflowDirection_WorkflowStepDTOs;
        }
        [Route(WorkflowDirectionRoute.SingleListWorkflowDefinition), HttpPost]
        public async Task<List<WorkflowDirection_WorkflowDefinitionDTO>> SingleListWorkflowDefinition([FromBody] WorkflowDirection_WorkflowDefinitionFilterDTO WorkflowDirection_WorkflowDefinitionFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter();
            WorkflowDefinitionFilter.Skip = 0;
            WorkflowDefinitionFilter.Take = 20;
            WorkflowDefinitionFilter.OrderBy = WorkflowDefinitionOrder.Id;
            WorkflowDefinitionFilter.OrderType = OrderType.ASC;
            WorkflowDefinitionFilter.Selects = WorkflowDefinitionSelect.ALL;
            WorkflowDefinitionFilter.Id = WorkflowDirection_WorkflowDefinitionFilterDTO.Id;
            WorkflowDefinitionFilter.Name = WorkflowDirection_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowDirection_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowDirection_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowDirection_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowDirection_WorkflowDefinitionFilterDTO.StatusId;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowDirection_WorkflowDefinitionDTO> WorkflowDirection_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(x => new WorkflowDirection_WorkflowDefinitionDTO(x)).ToList();
            return WorkflowDirection_WorkflowDefinitionDTOs;
        }

    }
}

