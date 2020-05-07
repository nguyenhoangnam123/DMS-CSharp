using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PriceListItemMapping : DataEntity,  IEquatable<PriceListItemMapping>
    {
        public long PriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public Item Item { get; set; }
        public PriceList PriceList { get; set; }

        public bool Equals(PriceListItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class PriceListItemMappingFilter : FilterEntity
    {
        public IdFilter PriceListId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter Price { get; set; }
        public List<PriceListItemMappingFilter> OrFilter { get; set; }
        public PriceListItemMappingOrder OrderBy {get; set;}
        public PriceListItemMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceListItemMappingOrder
    {
        PriceList = 0,
        Item = 1,
        Price = 2,
    }

    [Flags]
    public enum PriceListItemMappingSelect:long
    {
        ALL = E.ALL,
        PriceList = E._0,
        Item = E._1,
        Price = E._2,
    }
}
