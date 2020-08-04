﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WorkflowParameterDAO
    {
        public WorkflowParameterDAO()
        {
            RequestWorkflowParameterMappings = new HashSet<RequestWorkflowParameterMappingDAO>();
            WorkflowDirectionConditions = new HashSet<WorkflowDirectionConditionDAO>();
        }

        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long WorkflowParameterTypeId { get; set; }

        public virtual WorkflowDefinitionDAO WorkflowDefinition { get; set; }
        public virtual WorkflowParameterTypeDAO WorkflowParameterType { get; set; }
        public virtual ICollection<RequestWorkflowParameterMappingDAO> RequestWorkflowParameterMappings { get; set; }
        public virtual ICollection<WorkflowDirectionConditionDAO> WorkflowDirectionConditions { get; set; }
    }
}
