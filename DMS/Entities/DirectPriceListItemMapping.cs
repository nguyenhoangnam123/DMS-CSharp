using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class DirectPriceListItemMapping : DataEntity, IEquatable<DirectPriceListItemMapping>
    {
        public long DirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public DirectPriceList DirectPriceList { get; set; }
        public Item Item { get; set; }

        public bool Equals(DirectPriceListItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DirectPriceListItemMappingFilter : FilterEntity
    {
        public IdFilter DirectPriceListId { get; set; }
        public IdFilter DirectPriceListTypeId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter StatusId { get; set; }
        public LongFilter Price { get; set; }
        public List<DirectPriceListItemMappingFilter> OrFilter { get; set; }
        public DirectPriceListItemMappingOrder OrderBy { get; set; }
        public DirectPriceListItemMappingSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectPriceListItemMappingOrder
    {
        DirectPriceList = 0,
        Item = 1,
        Price = 2,
    }

    [Flags]
    public enum DirectPriceListItemMappingSelect : long
    {
        ALL = E.ALL,
        DirectPriceList = E._0,
        Item = E._1,
        Price = E._2,
    }
}
