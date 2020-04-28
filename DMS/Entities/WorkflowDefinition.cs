using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class WorkflowDefinition : DataEntity,  IEquatable<WorkflowDefinition>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long WorkflowTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public WorkflowType WorkflowType { get; set; }
        public List<WorkflowStep> WorkflowSteps { get; set; }
        public List<WorkflowDirection> WorkflowDirections { get; set; }
        public List<WorkflowParameter> WorkflowParameters { get; set; }

        public bool Equals(WorkflowDefinition other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class WorkflowDefinitionFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter WorkflowTypeId { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter StatusId { get; set; }
        public List<WorkflowDefinitionFilter> OrFilter { get; set; }
        public WorkflowDefinitionOrder OrderBy {get; set;}
        public WorkflowDefinitionSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkflowDefinitionOrder
    {
        Id = 0,
        Name = 1,
        WorkflowType = 2,
        StartDate = 3,
        EndDate = 4,
        Status = 5,
        WorkflowDirection = 6,
    }

    [Flags]
    public enum WorkflowDefinitionSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        WorkflowType = E._2,
        StartDate = E._3,
        EndDate = E._4,
        Status = E._5,
        WorkflowDirection = E._6,
    }
}
