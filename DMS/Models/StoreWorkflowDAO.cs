using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreWorkflowDAO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long WorkflowStateId { get; set; }
    }
}
