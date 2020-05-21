using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinition_WorkflowParameterDTO : DataDTO
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public WorkflowDefinition_WorkflowParameterDTO() {}
        public WorkflowDefinition_WorkflowParameterDTO(WorkflowParameter WorkflowParameter)
        {
            this.Id = WorkflowParameter.Id;
            this.WorkflowDefinitionId = WorkflowParameter.WorkflowDefinitionId;
            this.Code = WorkflowParameter.Code;
            this.Name = WorkflowParameter.Name;
            this.Errors = WorkflowParameter.Errors;
        }
    }

    public class WorkflowDefinition_WorkflowParameterFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter WorkflowDefinitionId { get; set; }
        
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        
        public WorkflowParameterOrder OrderBy { get; set; }
    }
}