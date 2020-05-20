using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinition_WorkflowDirectionDTO : DataDTO
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public long FromStepId { get; set; }
        public long ToStepId { get; set; }
        public string SubjectMailForCreator { get; set; }
        public string SubjectMailForNextStep { get; set; }
        public string BodyMailForCreator { get; set; }
        public string BodyMailForNextStep { get; set; }
        public WorkflowDefinition_WorkflowStepDTO FromStep { get; set; }   
        public WorkflowDefinition_WorkflowStepDTO ToStep { get; set; }   
        
        public WorkflowDefinition_WorkflowDirectionDTO() {}
        public WorkflowDefinition_WorkflowDirectionDTO(WorkflowDirection WorkflowDirection)
        {
            this.Id = WorkflowDirection.Id;
            this.WorkflowDefinitionId = WorkflowDirection.WorkflowDefinitionId;
            this.FromStepId = WorkflowDirection.FromStepId;
            this.ToStepId = WorkflowDirection.ToStepId;
            this.SubjectMailForCreator = WorkflowDirection.SubjectMailForCreator;
            this.SubjectMailForNextStep = WorkflowDirection.SubjectMailForNextStep;
            this.BodyMailForCreator = WorkflowDirection.BodyMailForCreator;
            this.BodyMailForNextStep = WorkflowDirection.BodyMailForNextStep;
            this.FromStep = WorkflowDirection.FromStep == null ? null : new WorkflowDefinition_WorkflowStepDTO(WorkflowDirection.FromStep);
            this.ToStep = WorkflowDirection.ToStep == null ? null : new WorkflowDefinition_WorkflowStepDTO(WorkflowDirection.ToStep);
            this.Errors = WorkflowDirection.Errors;
        }
    }

    public class WorkflowDefinition_WorkflowDirectionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter WorkflowDefinitionId { get; set; }
        
        public IdFilter FromStepId { get; set; }
        
        public IdFilter ToStepId { get; set; }
        
        public StringFilter SubjectMailForCreator { get; set; }
        
        public StringFilter SubjectMailForNextStep { get; set; }
        
        public StringFilter BodyMailForCreator { get; set; }
        
        public StringFilter BodyMailForNextStep { get; set; }
        
        public WorkflowDirectionOrder OrderBy { get; set; }
    }
}