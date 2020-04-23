using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowParameterDAO
    {
        public WorkflowParameterDAO()
        {
            StoreWorkflowParameterMappings = new HashSet<StoreWorkflowParameterMappingDAO>();
        }

        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual WorkflowDefinitionDAO WorkflowDefinition { get; set; }
        public virtual ICollection<StoreWorkflowParameterMappingDAO> StoreWorkflowParameterMappings { get; set; }
    }
}
