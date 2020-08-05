using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public partial class WorkflowDirectionCondition
    {
        public long Id { get; set; }
        public long WorkflowDirectionId { get; set; }
        public long WorkflowParameterId { get; set; }
        public long WorkflowOperatorId { get; set; }
        public string Value { get; set; }

        public virtual WorkflowDirection WorkflowDirection { get; set; }
        public virtual WorkflowOperator WorkflowOperator { get; set; }
        public virtual WorkflowParameter WorkflowParameter { get; set; }
    }
}
