using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class RequestWorkflowParameterMapping : DataEntity,  IEquatable<RequestWorkflowParameterMapping>
    {
        public long WorkflowParameterId { get; set; }
        public Guid RequestId { get; set; }
        public string Value { get; set; }
        public WorkflowParameter WorkflowParameter { get; set; }

        public bool Equals(RequestWorkflowParameterMapping other)
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
        public StringFilter Value { get; set; }
        public List<StoreWorkflowParameterMappingFilter> OrFilter { get; set; }
        public StoreWorkflowParameterMappingOrder OrderBy {get; set;}
        public StoreWorkflowParameterMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreWorkflowParameterMappingOrder
    {
        WorkflowParameter = 0,
        Value = 1,
    }

    [Flags]
    public enum StoreWorkflowParameterMappingSelect:long
    {
        ALL = E.ALL,
        WorkflowParameter = E._0,
        Value = E._1,
    }
}
