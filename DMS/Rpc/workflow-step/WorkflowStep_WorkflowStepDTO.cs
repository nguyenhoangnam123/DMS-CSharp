using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_step
{
    public class WorkflowStep_WorkflowStepDTO : DataDTO
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public string SubjectMailForReject { get; set; }
        public string BodyMailForReject { get; set; }
        public WorkflowStep_RoleDTO Role { get; set; }
        public WorkflowStep_WorkflowDefinitionDTO WorkflowDefinition { get; set; }
        public WorkflowStep_WorkflowStepDTO() {}
        public WorkflowStep_WorkflowStepDTO(WorkflowStep WorkflowStep)
        {
            this.Id = WorkflowStep.Id;
            this.WorkflowDefinitionId = WorkflowStep.WorkflowDefinitionId;
            this.Code = WorkflowStep.Code;
            this.Name = WorkflowStep.Name;
            this.RoleId = WorkflowStep.RoleId;
            this.SubjectMailForReject = WorkflowStep.SubjectMailForReject;
            this.BodyMailForReject = WorkflowStep.BodyMailForReject;
            this.Role = WorkflowStep.Role == null ? null : new WorkflowStep_RoleDTO(WorkflowStep.Role);
            this.WorkflowDefinition = WorkflowStep.WorkflowDefinition == null ? null : new WorkflowStep_WorkflowDefinitionDTO(WorkflowStep.WorkflowDefinition);
            this.Errors = WorkflowStep.Errors;
        }
    }

    public class WorkflowStep_WorkflowStepFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter WorkflowDefinitionId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter RoleId { get; set; }
        public StringFilter SubjectMailForReject { get; set; }
        public StringFilter BodyMailForReject { get; set; }
        public WorkflowStepOrder OrderBy { get; set; }
    }
}
