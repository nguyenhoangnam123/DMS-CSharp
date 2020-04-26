using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreWorkflowDAO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long WorkflowStepId { get; set; }
        public long WorkflowStateId { get; set; }
        public long? UpdatedAt { get; set; }
        public long? AppUserId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual WorkflowStateDAO WorkflowState { get; set; }
        public virtual WorkflowStepDAO WorkflowStep { get; set; }
    }
}
