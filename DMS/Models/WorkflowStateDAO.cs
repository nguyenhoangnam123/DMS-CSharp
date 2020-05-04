using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowStateDAO
    {
        public WorkflowStateDAO()
        {
            RequestWorkflows = new HashSet<RequestWorkflowDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RequestWorkflowDAO> RequestWorkflows { get; set; }
    }
}
