using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class DirectPriceListStoreGroupingMapping : DataEntity,  IEquatable<DirectPriceListStoreGroupingMapping>
    {
        public long DirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }
        public DirectPriceList DirectPriceList { get; set; }
        public StoreGrouping StoreGrouping { get; set; }

        public bool Equals(DirectPriceListStoreGroupingMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DirectPriceListStoreGroupingMappingFilter : FilterEntity
    {
        public IdFilter DirectPriceListId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public List<DirectPriceListStoreGroupingMappingFilter> OrFilter { get; set; }
        public DirectPriceListStoreGroupingMappingOrder OrderBy {get; set;}
        public DirectPriceListStoreGroupingMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectPriceListStoreGroupingMappingOrder
    {
        DirectPriceList = 0,
        StoreGrouping = 1,
    }

    [Flags]
    public enum DirectPriceListStoreGroupingMappingSelect:long
    {
        ALL = E.ALL,
        DirectPriceList = E._0,
        StoreGrouping = E._1,
    }
}
