using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowDirectionDAO
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public long FromStepId { get; set; }
        public long ToStepId { get; set; }

        public virtual WorkflowStepDAO FromStep { get; set; }
        public virtual WorkflowStepDAO ToStep { get; set; }
        public virtual WorkflowDefinitionDAO WorkflowDefinition { get; set; }
    }
}
