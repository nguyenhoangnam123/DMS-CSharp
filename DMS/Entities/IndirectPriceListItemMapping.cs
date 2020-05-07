using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectPriceListItemMapping : DataEntity,  IEquatable<IndirectPriceListItemMapping>
    {
        public long IndirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public IndirectPriceList IndirectPriceList { get; set; }
        public Item Item { get; set; }

        public bool Equals(IndirectPriceListItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IndirectPriceListItemMappingFilter : FilterEntity
    {
        public IdFilter IndirectPriceListId { get; set; }
        public IdFilter IndirectPriceListTypeId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter Price { get; set; }
        public List<IndirectPriceListItemMappingFilter> OrFilter { get; set; }
        public IndirectPriceListItemMappingOrder OrderBy {get; set;}
        public IndirectPriceListItemMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectPriceListItemMappingOrder
    {
        IndirectPriceList = 0,
        Item = 1,
        Price = 2,
    }

    [Flags]
    public enum IndirectPriceListItemMappingSelect:long
    {
        ALL = E.ALL,
        IndirectPriceList = E._0,
        Item = E._1,
        Price = E._2,
    }
}
