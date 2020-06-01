using Common;
using DMS.Entities;
using DMS.Services.MWorkflow;
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
            WorkflowDirection.UpdatedAt = WorkflowDirection_WorkflowDirectionDTO.UpdatedAt;
            WorkflowDirection.FromStep = WorkflowDirection_WorkflowDirectionDTO.FromStep == null ? null : new WorkflowStep
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.FromStep.Id,
                WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionDTO.FromStep.WorkflowDefinitionId,
                Name = WorkflowDirection_WorkflowDirectionDTO.FromStep.Name,
                Code = WorkflowDirection_WorkflowDirectionDTO.FromStep.Code,
                RoleId = WorkflowDirection_WorkflowDirectionDTO.FromStep.RoleId,
                SubjectMailForReject = WorkflowDirection_WorkflowDirectionDTO.FromStep.SubjectMailForReject,
                BodyMailForReject = WorkflowDirection_WorkflowDirectionDTO.FromStep.BodyMailForReject,
            };
            WorkflowDirection.ToStep = WorkflowDirection_WorkflowDirectionDTO.ToStep == null ? null : new WorkflowStep
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.ToStep.Id,
                WorkflowDefinitionId = WorkflowDirection_WorkflowDirectionDTO.ToStep.WorkflowDefinitionId,
                Code = WorkflowDirection_WorkflowDirectionDTO.ToStep.Code,
                Name = WorkflowDirection_WorkflowDirectionDTO.ToStep.Name,
                RoleId = WorkflowDirection_WorkflowDirectionDTO.ToStep.RoleId,
                SubjectMailForReject = WorkflowDirection_WorkflowDirectionDTO.ToStep.SubjectMailForReject,
                BodyMailForReject = WorkflowDirection_WorkflowDirectionDTO.ToStep.BodyMailForReject,
            };
            WorkflowDirection.WorkflowDefinition = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition == null ? null : new WorkflowDefinition
            {
                Id = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.Id,
                Code = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.Code,
                Name = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.Name,
                WorkflowTypeId = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.WorkflowTypeId,
                StartDate = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.StartDate,
                EndDate = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.EndDate,
                StatusId = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.StatusId,
                UpdatedAt = WorkflowDirection_WorkflowDirectionDTO.WorkflowDefinition.UpdatedAt,
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
            WorkflowDirectionFilter.UpdatedAt = WorkflowDirection_WorkflowDirectionFilterDTO.UpdatedAt;
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
            WorkflowStepFilter.Code = WorkflowDirection_WorkflowStepFilterDTO.Code;
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
            WorkflowDefinitionFilter.Code = WorkflowDirection_WorkflowDefinitionFilterDTO.Code;
            WorkflowDefinitionFilter.Name = WorkflowDirection_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowDirection_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowDirection_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowDirection_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowDirection_WorkflowDefinitionFilterDTO.StatusId;
            WorkflowDefinitionFilter.UpdatedAt = WorkflowDirection_WorkflowDefinitionFilterDTO.UpdatedAt;

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
            WorkflowStepFilter.Code = WorkflowDirection_WorkflowStepFilterDTO.Code;
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
            WorkflowDefinitionFilter.Code = WorkflowDirection_WorkflowDefinitionFilterDTO.Code;
            WorkflowDefinitionFilter.Name = WorkflowDirection_WorkflowDefinitionFilterDTO.Name;
            WorkflowDefinitionFilter.WorkflowTypeId = WorkflowDirection_WorkflowDefinitionFilterDTO.WorkflowTypeId;
            WorkflowDefinitionFilter.StartDate = WorkflowDirection_WorkflowDefinitionFilterDTO.StartDate;
            WorkflowDefinitionFilter.EndDate = WorkflowDirection_WorkflowDefinitionFilterDTO.EndDate;
            WorkflowDefinitionFilter.StatusId = WorkflowDirection_WorkflowDefinitionFilterDTO.StatusId;
            WorkflowDefinitionFilter.UpdatedAt = WorkflowDirection_WorkflowDefinitionFilterDTO.UpdatedAt;

            List<WorkflowDefinition> WorkflowDefinitions = await WorkflowDefinitionService.List(WorkflowDefinitionFilter);
            List<WorkflowDirection_WorkflowDefinitionDTO> WorkflowDirection_WorkflowDefinitionDTOs = WorkflowDefinitions
                .Select(x => new WorkflowDirection_WorkflowDefinitionDTO(x)).ToList();
            return WorkflowDirection_WorkflowDefinitionDTOs;
        }

    }
}

