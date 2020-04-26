using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowStepDAO
    {
        public WorkflowStepDAO()
        {
            StoreWorkflows = new HashSet<StoreWorkflowDAO>();
            WorkflowDirectionFromSteps = new HashSet<WorkflowDirectionDAO>();
            WorkflowDirectionToSteps = new HashSet<WorkflowDirectionDAO>();
        }

        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }

        public virtual RoleDAO Role { get; set; }
        public virtual WorkflowDefinitionDAO WorkflowDefinition { get; set; }
        public virtual ICollection<StoreWorkflowDAO> StoreWorkflows { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirectionFromSteps { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirectionToSteps { get; set; }
    }
}
