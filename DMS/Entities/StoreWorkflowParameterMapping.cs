using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class StoreWorkflowParameterMapping : DataEntity,  IEquatable<StoreWorkflowParameterMapping>
    {
        public long WorkflowParameterId { get; set; }
        public long StoreId { get; set; }
        public string Value { get; set; }
        public Store Store { get; set; }
        public WorkflowParameter WorkflowParameter { get; set; }

        public bool Equals(StoreWorkflowParameterMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class StoreWorkflowParameterMappingFilter : FilterEntity
    {
        public IdFilter WorkflowParameterId { get; set; }
        public IdFilter StoreId { get; set; }
        public StringFilter Value { get; set; }
        public List<StoreWorkflowParameterMappingFilter> OrFilter { get; set; }
        public StoreWorkflowParameterMappingOrder OrderBy {get; set;}
        public StoreWorkflowParameterMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreWorkflowParameterMappingOrder
    {
        WorkflowParameter = 0,
        Store = 1,
        Value = 2,
    }

    [Flags]
    public enum StoreWorkflowParameterMappingSelect:long
    {
        ALL = E.ALL,
        WorkflowParameter = E._0,
        Store = E._1,
        Value = E._2,
    }
}
