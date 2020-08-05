using Common;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class WorkflowOperator : DataEntity
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long WorkflowParameterTypeId { get; set; }

        public virtual WorkflowParameterType WorkflowParameterType { get; set; }
    }
}
