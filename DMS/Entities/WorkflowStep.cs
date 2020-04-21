using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class WorkflowStep : DataEntity,  IEquatable<WorkflowStep>
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
        public WorkflowDefinition WorkflowDefinition { get; set; }

        public bool Equals(WorkflowStep other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class WorkflowStepFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter WorkflowDefinitionId { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter RoleId { get; set; }
        public List<WorkflowStepFilter> OrFilter { get; set; }
        public WorkflowStepOrder OrderBy {get; set;}
        public WorkflowStepSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkflowStepOrder
    {
        Id = 0,
        WorkflowDefinition = 1,
        Name = 2,
        Role = 3,
    }

    [Flags]
    public enum WorkflowStepSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        WorkflowDefinition = E._1,
        Name = E._2,
        Role = E._3,
    }
}
