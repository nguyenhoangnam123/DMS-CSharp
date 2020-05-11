using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PriceList : DataEntity,  IEquatable<PriceList>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long PriceListTypeId { get; set; }
        public Organization Organization { get; set; }
        public PriceListType PriceListType { get; set; }
        public Status Status { get; set; }
        public List<PriceListItemMapping> PriceListItemMappings { get; set; }
        public List<PriceListStoreMapping> PriceListStoreMappings { get; set; }
        public List<PriceListStoreTypeMapping> PriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(PriceList other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PriceListFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter PriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<PriceListFilter> OrFilter { get; set; }
        public PriceListOrder OrderBy {get; set;}
        public PriceListSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceListOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Status = 3,
        Organization = 4,
        PriceListType = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum PriceListSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Status = E._3,
        Organization = E._4,
        PriceListType = E._5,
    }
}