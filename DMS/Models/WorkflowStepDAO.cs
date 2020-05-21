using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowStepDAO
    {
        public WorkflowStepDAO()
        {
            RequestWorkflowStepMappings = new HashSet<RequestWorkflowStepMappingDAO>();
            WorkflowDirectionFromSteps = new HashSet<WorkflowDirectionDAO>();
            WorkflowDirectionToSteps = new HashSet<WorkflowDirectionDAO>();
        }

        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public string SubjectMailForReject { get; set; }
        public string BodyMailForReject { get; set; }

        public virtual RoleDAO Role { get; set; }
        public virtual WorkflowDefinitionDAO WorkflowDefinition { get; set; }
        public virtual ICollection<RequestWorkflowStepMappingDAO> RequestWorkflowStepMappings { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirectionFromSteps { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirectionToSteps { get; set; }
    }
}
