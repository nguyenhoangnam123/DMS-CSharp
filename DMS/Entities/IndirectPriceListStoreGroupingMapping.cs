using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectPriceListStoreGroupingMapping : DataEntity,  IEquatable<IndirectPriceListStoreGroupingMapping>
    {
        public long IndirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }
        public IndirectPriceList IndirectPriceList { get; set; }
        public StoreGrouping StoreGrouping { get; set; }

        public bool Equals(IndirectPriceListStoreGroupingMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IndirectPriceListStoreGroupingMappingFilter : FilterEntity
    {
        public IdFilter IndirectPriceListId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public List<IndirectPriceListStoreGroupingMappingFilter> OrFilter { get; set; }
        public IndirectPriceListStoreGroupingMappingOrder OrderBy {get; set;}
        public IndirectPriceListStoreGroupingMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectPriceListStoreGroupingMappingOrder
    {
        IndirectPriceList = 0,
        StoreGrouping = 1,
    }

    [Flags]
    public enum IndirectPriceListStoreGroupingMappingSelect:long
    {
        ALL = E.ALL,
        IndirectPriceList = E._0,
        StoreGrouping = E._1,
    }
}
