using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class WorkflowParameter : DataEntity,  IEquatable<WorkflowParameter>
    {
        public long Id { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public WorkflowDefinition WorkflowDefinition { get; set; }

        public bool Equals(WorkflowParameter other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class WorkflowParameterFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter WorkflowDefinitionId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<WorkflowParameterFilter> OrFilter { get; set; }
        public WorkflowParameterOrder OrderBy {get; set;}
        public WorkflowParameterSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkflowParameterOrder
    {
        Id = 0,
        WorkflowDefinition = 1,
        Name = 2,
        Code = 3,
    }

    [Flags]
    public enum WorkflowParameterSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        WorkflowDefinition = E._1,
        Name = E._2,
        Code = E._3,
    }
}
