using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectPriceListStoreTypeMapping : DataEntity,  IEquatable<IndirectPriceListStoreTypeMapping>
    {
        public long IndirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }
        public IndirectPriceList IndirectPriceList { get; set; }
        public StoreType StoreType { get; set; }

        public bool Equals(IndirectPriceListStoreTypeMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IndirectPriceListStoreTypeMappingFilter : FilterEntity
    {
        public IdFilter IndirectPriceListId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public List<IndirectPriceListStoreTypeMappingFilter> OrFilter { get; set; }
        public IndirectPriceListStoreTypeMappingOrder OrderBy {get; set;}
        public IndirectPriceListStoreTypeMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectPriceListStoreTypeMappingOrder
    {
        IndirectPriceList = 0,
        StoreType = 1,
    }

    [Flags]
    public enum IndirectPriceListStoreTypeMappingSelect:long
    {
        ALL = E.ALL,
        IndirectPriceList = E._0,
        StoreType = E._1,
    }
}
