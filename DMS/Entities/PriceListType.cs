using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class PriceListType : DataEntity, IEquatable<PriceListType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(PriceListType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PriceListTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<PriceListTypeFilter> OrFilter { get; set; }
        public PriceListTypeOrder OrderBy { get; set; }
        public PriceListTypeSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceListTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum PriceListTypeSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
