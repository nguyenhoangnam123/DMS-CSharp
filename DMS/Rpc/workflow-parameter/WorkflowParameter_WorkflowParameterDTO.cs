using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_parameter
{
    public class WorkflowParameter_WorkflowParameterDTO : DataDTO
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }
        public WorkflowParameter_WorkflowDefinitionDTO WorkflowDefinition { get; set; }
        public WorkflowParameter_WorkflowParameterDTO() {}
        public WorkflowParameter_WorkflowParameterDTO(WorkflowParameter WorkflowParameter)
        {
            this.Id = WorkflowParameter.Id;
            this.WorkflowDefinitionId = WorkflowParameter.WorkflowDefinitionId;
            this.Name = WorkflowParameter.Name;
            this.WorkflowDefinition = WorkflowParameter.WorkflowDefinition == null ? null : new WorkflowParameter_WorkflowDefinitionDTO(WorkflowParameter.WorkflowDefinition);
            this.Errors = WorkflowParameter.Errors;
        }
    }

    public class WorkflowParameter_WorkflowParameterFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter WorkflowDefinitionId { get; set; }
        public StringFilter Name { get; set; }
        public WorkflowParameterOrder OrderBy { get; set; }
    }
}
