﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowDefinitionDAO
    {
        public WorkflowDefinitionDAO()
        {
            WorkflowDirections = new HashSet<WorkflowDirectionDAO>();
            WorkflowSteps = new HashSet<WorkflowStepDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long WorkflowTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual WorkflowTypeDAO WorkflowType { get; set; }
        public virtual ICollection<WorkflowDirectionDAO> WorkflowDirections { get; set; }
        public virtual ICollection<WorkflowStepDAO> WorkflowSteps { get; set; }
    }
}