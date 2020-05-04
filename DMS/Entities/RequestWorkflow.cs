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
        public Guid RequestId { get; set; }
        public long WorkflowStepId { get; set; }
        public long WorkflowStateId { get; set; }
        public long? AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public WorkflowState WorkflowState { get; set; }
        public WorkflowStep WorkflowStep { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool Equals(RequestWorkflow other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class RequestWorkflowFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public GuidFilter RequestId { get; set; }
        public IdFilter WorkflowStepId { get; set; }
        public IdFilter WorkflowStateId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<RequestWorkflowFilter> OrFilter { get; set; }
        public RequestWorkflowOrder OrderBy {get; set;}
        public RequestWorkflowSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestWorkflowOrder
    {
        Id = 0,
        WorkflowStep = 2,
        WorkflowState = 3,
        RequestId = 4,
        AppUser = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum RequestWorkflowSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        WorkflowStep = E._2,
        WorkflowState = E._3,
        RequestId = E._4,
        AppUser = E._5,
    }
}
