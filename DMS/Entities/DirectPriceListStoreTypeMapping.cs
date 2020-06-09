using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class DirectPriceListStoreTypeMapping : DataEntity, IEquatable<DirectPriceListStoreTypeMapping>
    {
        public long DirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }
        public DirectPriceList DirectPriceList { get; set; }
        public StoreType StoreType { get; set; }

        public bool Equals(DirectPriceListStoreTypeMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DirectPriceListStoreTypeMappingFilter : FilterEntity
    {
        public IdFilter DirectPriceListId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public List<DirectPriceListStoreTypeMappingFilter> OrFilter { get; set; }
        public DirectPriceListStoreTypeMappingOrder OrderBy { get; set; }
        public DirectPriceListStoreTypeMappingSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectPriceListStoreTypeMappingOrder
    {
        DirectPriceList = 0,
        StoreType = 1,
    }

    [Flags]
    public enum DirectPriceListStoreTypeMappingSelect : long
    {
        ALL = E.ALL,
        DirectPriceList = E._0,
        StoreType = E._1,
    }
}
