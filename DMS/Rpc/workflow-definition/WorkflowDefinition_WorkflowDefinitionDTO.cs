using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinition_WorkflowDefinitionDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long WorkflowTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public WorkflowDefinition_WorkflowTypeDTO WorkflowType { get; set; }
        public List<WorkflowDefinition_WorkflowDirectionDTO> WorkflowDirections { get; set; }
        public List<WorkflowDefinition_WorkflowParameterDTO> WorkflowParameters { get; set; }
        public List<WorkflowDefinition_WorkflowStepDTO> WorkflowSteps { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public WorkflowDefinition_WorkflowDefinitionDTO() {}
        public WorkflowDefinition_WorkflowDefinitionDTO(WorkflowDefinition WorkflowDefinition)
        {
            this.Id = WorkflowDefinition.Id;
            this.Name = WorkflowDefinition.Name;
            this.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            this.StartDate = WorkflowDefinition.StartDate;
            this.EndDate = WorkflowDefinition.EndDate;
            this.StatusId = WorkflowDefinition.StatusId;
            this.WorkflowType = WorkflowDefinition.WorkflowType == null ? null : new WorkflowDefinition_WorkflowTypeDTO(WorkflowDefinition.WorkflowType);
            this.WorkflowDirections = WorkflowDefinition.WorkflowDirections?.Select(x => new WorkflowDefinition_WorkflowDirectionDTO(x)).ToList();
            this.WorkflowParameters = WorkflowDefinition.WorkflowParameters?.Select(x => new WorkflowDefinition_WorkflowParameterDTO(x)).ToList();
            this.WorkflowSteps = WorkflowDefinition.WorkflowSteps?.Select(x => new WorkflowDefinition_WorkflowStepDTO(x)).ToList();
            this.Errors = WorkflowDefinition.Errors;
        }
    }

    public class WorkflowDefinition_WorkflowDefinitionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter WorkflowTypeId { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public WorkflowDefinitionOrder OrderBy { get; set; }
    }
}
