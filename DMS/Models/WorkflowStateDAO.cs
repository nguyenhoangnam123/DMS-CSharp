using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowStateDAO
    {
        public WorkflowStateDAO()
        {
            StoreWorkflows = new HashSet<StoreWorkflowDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StoreWorkflowDAO> StoreWorkflows { get; set; }
    }
}
