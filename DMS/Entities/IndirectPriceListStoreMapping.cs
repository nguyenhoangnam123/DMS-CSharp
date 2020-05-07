using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectPriceListStoreMapping : DataEntity,  IEquatable<IndirectPriceListStoreMapping>
    {
        public long IndirectPriceListId { get; set; }
        public long StoreId { get; set; }
        public IndirectPriceList IndirectPriceList { get; set; }
        public Store Store { get; set; }

        public bool Equals(IndirectPriceListStoreMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class IndirectPriceListStoreMappingFilter : FilterEntity
    {
        public IdFilter IndirectPriceListId { get; set; }
        public IdFilter StoreId { get; set; }
        public List<IndirectPriceListStoreMappingFilter> OrFilter { get; set; }
        public IndirectPriceListStoreMappingOrder OrderBy {get; set;}
        public IndirectPriceListStoreMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectPriceListStoreMappingOrder
    {
        IndirectPriceList = 0,
        Store = 1,
    }

    [Flags]
    public enum IndirectPriceListStoreMappingSelect:long
    {
        ALL = E.ALL,
        IndirectPriceList = E._0,
        Store = E._1,
    }
}
