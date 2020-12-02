using System;
using System.Collections.Generic;
using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class LuckyNumber : DataEntity,  IEquatable<LuckyNumber>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public long RewardStatusId { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public RewardStatus RewardStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public bool Equals(LuckyNumber other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class LuckyNumberFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Value { get; set; }
        public IdFilter RewardStatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<LuckyNumberFilter> OrFilter { get; set; }
        public LuckyNumberOrder OrderBy {get; set;}
        public LuckyNumberSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LuckyNumberOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        RewardStatus = 3,
        Value = 4,
        Row = 7,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum LuckyNumberSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        RewardStatus = E._3,
        Value = E._4,
        Row = E._7,
    }
}