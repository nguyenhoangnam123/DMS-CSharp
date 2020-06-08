using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class DirectPriceList : DataEntity, IEquatable<DirectPriceList>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long StatusId { get; set; }
        public long OrganizationId { get; set; }
        public long DirectPriceListTypeId { get; set; }
        public DirectPriceListType DirectPriceListType { get; set; }
        public Organization Organization { get; set; }
        public Status Status { get; set; }
        public List<DirectPriceListItemMapping> DirectPriceListItemMappings { get; set; }
        public List<DirectPriceListStoreGroupingMapping> DirectPriceListStoreGroupingMappings { get; set; }
        public List<DirectPriceListStoreMapping> DirectPriceListStoreMappings { get; set; }
        public List<DirectPriceListStoreTypeMapping> DirectPriceListStoreTypeMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(DirectPriceList other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class DirectPriceListFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter DirectPriceListTypeId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<DirectPriceListFilter> OrFilter { get; set; }
        public DirectPriceListOrder OrderBy { get; set; }
        public DirectPriceListSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectPriceListOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        StartDate = 3,
        EndDate = 4,
        Status = 5,
        Organization = 6,
        DirectPriceListType = 7,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum DirectPriceListSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        StartDate = E._3,
        EndDate = E._4,
        Status = E._5,
        Organization = E._6,
        DirectPriceListType = E._7,
    }
}
