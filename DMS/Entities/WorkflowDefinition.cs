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
        public string Code { get; set; }
        public string Name { get; set; }
        public long WorkflowTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public DateTime UpdatedAt { get; set; }
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
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter WorkflowTypeId { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<WorkflowDefinitionFilter> OrFilter { get; set; }
        public WorkflowDefinitionOrder OrderBy {get; set;}
        public WorkflowDefinitionSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkflowDefinitionOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        WorkflowType = 3,
        StartDate = 4,
        EndDate = 5,
        Status = 6,
        WorkflowDirection = 7,
        UpdatedAt = 8,
    }

    [Flags]
    public enum WorkflowDefinitionSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        WorkflowType = E._3,
        StartDate = E._4,
        EndDate = E._5,
        Status = E._6,
        WorkflowDirection = E._7,
        UpdatedAt = E._8,
    }
}
