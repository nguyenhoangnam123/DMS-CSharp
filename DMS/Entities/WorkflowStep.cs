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
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public string SubjectMailForReject { get; set; }
        public string BodyMailForReject { get; set; }
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
        public StringFilter Code { get; set; }
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
        Code = 2,
        Name = 3,
        Role = 4,
    }

    [Flags]
    public enum WorkflowStepSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        WorkflowDefinition = E._1,
        Code = E._2,
        Name = E._3,
        Role = E._4,
    }
}
