using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectPriceList : DataEntity,  IEquatable<IndirectPriceList>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long IndirectPriceListTypeId { get; set; }
        public IndirectPriceListType IndirectPriceListType { get; set; }
        public Organization Organization { get; set; }
        public Status Status { get; set; }
        public List<IndirectPriceListItemMapping> IndirectPriceListItemMappings { get; set; }
        public List<IndirectPriceListStoreMapping> IndirectPriceListStoreMappings { get; set; }
        public List<IndirectPriceListStoreTypeMapping> IndirectPriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(IndirectPriceList other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class IndirectPriceListFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter IndirectPriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<IndirectPriceListFilter> OrFilter { get; set; }
        public IndirectPriceListOrder OrderBy {get; set;}
        public IndirectPriceListSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectPriceListOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Status = 3,
        Organization = 4,
        IndirectPriceListType = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum IndirectPriceListSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Status = E._3,
        Organization = E._4,
        IndirectPriceListType = E._5,
    }
}
