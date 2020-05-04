using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class RequestWorkflow : DataEntity,  IEquatable<RequestWorkflow>
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long WorkflowStepId { get; set; }
        public long WorkflowStateId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? AppUserId { get; set; }

        public bool Equals(RequestWorkflow other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class StoreWorkflowFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter WorkflowStateId { get; set; }
        public List<StoreWorkflowFilter> OrFilter { get; set; }
        public StoreWorkflowOrder OrderBy {get; set;}
        public StoreWorkflowSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreWorkflowOrder
    {
        Id = 0,
        Store = 1,
        WorkflowState = 2,
    }

    [Flags]
    public enum StoreWorkflowSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Store = E._1,
        WorkflowState = E._2,
    }
}
