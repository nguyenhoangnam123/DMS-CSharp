using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class DirectPriceListStoreMapping : DataEntity,  IEquatable<DirectPriceListStoreMapping>
    {
        public long DirectPriceListId { get; set; }
        public long StoreId { get; set; }
        public DirectPriceList DirectPriceList { get; set; }
        public Store Store { get; set; }

        public bool Equals(DirectPriceListStoreMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class DirectPriceListStoreMappingFilter : FilterEntity
    {
        public IdFilter DirectPriceListId { get; set; }
        public IdFilter StoreId { get; set; }
        public List<DirectPriceListStoreMappingFilter> OrFilter { get; set; }
        public DirectPriceListStoreMappingOrder OrderBy {get; set;}
        public DirectPriceListStoreMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectPriceListStoreMappingOrder
    {
        DirectPriceList = 0,
        Store = 1,
    }

    [Flags]
    public enum DirectPriceListStoreMappingSelect:long
    {
        ALL = E.ALL,
        DirectPriceList = E._0,
        Store = E._1,
    }
}
