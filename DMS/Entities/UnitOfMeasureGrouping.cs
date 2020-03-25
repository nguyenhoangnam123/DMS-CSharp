using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class UnitOfMeasureGrouping : DataEntity, IEquatable<UnitOfMeasureGrouping>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long StatusId { get; set; }
        public Status Status { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents { get; set; }

        public bool Equals(UnitOfMeasureGrouping other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class UnitOfMeasureGroupingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter StatusId { get; set; }
        public List<UnitOfMeasureGroupingFilter> OrFilter { get; set; }
        public UnitOfMeasureGroupingOrder OrderBy { get; set; }
        public UnitOfMeasureGroupingSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UnitOfMeasureGroupingOrder
    {
        Id = 0,
        Name = 1,
        UnitOfMeasure = 2,
        Status = 3,
    }

    [Flags]
    public enum UnitOfMeasureGroupingSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        UnitOfMeasure = E._2,
        Status = E._3,
    }
}
