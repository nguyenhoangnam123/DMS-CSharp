using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.workflow_step
{
    public class WorkflowStep_WorkflowDefinitionDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        public string Name { get; set; }
        
        public long WorkflowTypeId { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public long StatusId { get; set; }
        

        public WorkflowStep_WorkflowDefinitionDTO() {}
        public WorkflowStep_WorkflowDefinitionDTO(WorkflowDefinition WorkflowDefinition)
        {
            
            this.Id = WorkflowDefinition.Id;
            
            this.Code = WorkflowDefinition.Code;
            this.Name = WorkflowDefinition.Name;
            
            this.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            
            this.StartDate = WorkflowDefinition.StartDate;
            
            this.EndDate = WorkflowDefinition.EndDate;
            
            this.StatusId = WorkflowDefinition.StatusId;
            this.UpdatedAt = WorkflowDefinition.UpdatedAt;
            
            this.Errors = WorkflowDefinition.Errors;
        }
    }

    public class WorkflowStep_WorkflowDefinitionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        
        public IdFilter WorkflowTypeId { get; set; }
        
        public DateFilter StartDate { get; set; }
        
        public DateFilter EndDate { get; set; }
        
        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedAt { get; set; }
        
        public WorkflowDefinitionOrder OrderBy { get; set; }
    }
}