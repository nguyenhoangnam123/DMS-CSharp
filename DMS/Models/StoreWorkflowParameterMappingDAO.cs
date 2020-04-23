using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreWorkflowParameterMappingDAO
    {
        public long WorkflowParameterId { get; set; }
        public long StoreId { get; set; }
        public string Value { get; set; }

        public virtual StoreDAO Store { get; set; }
        public virtual WorkflowParameterDAO WorkflowParameter { get; set; }
    }
}
