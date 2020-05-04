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

    public class RequestWorkflowParameterMappingFilter : FilterEntity
    {
        public IdFilter WorkflowParameterId { get; set; }
        public IdFilter RequestId { get; set; }
        public StringFilter Value { get; set; }
        public List<RequestWorkflowParameterMappingFilter> OrFilter { get; set; }
        public RequestWorkflowParameterMappingOrder OrderBy {get; set;}
        public RequestWorkflowParameterMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestWorkflowParameterMappingOrder
    {
        WorkflowParameter = 0,
        Value = 2,
    }

    [Flags]
    public enum RequestWorkflowParameterMappingSelect:long
    {
        ALL = E.ALL,
        WorkflowParameter = E._0,
        Value = E._2,
    }
}
