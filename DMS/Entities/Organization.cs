using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Organization : DataEntity, IEquatable<Organization>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public long StatusId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid RowId { get; set; }
        public Organization Parent { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public List<AppUser> AppUsers { get; set; }

        public bool Equals(Organization other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class OrganizationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public LongFilter Level { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Address { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longitude { get; set; }
        public List<OrganizationFilter> OrFilter { get; set; }
        public OrganizationOrder OrderBy { get; set; }
        public OrganizationSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganizationOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Parent = 3,
        Path = 4,
        Level = 5,
        Status = 6,
        Phone = 7,
        Address = 8,
        Latitude = 9,
        Longitude = 10,
    }

    [Flags]
    public enum OrganizationSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Parent = E._3,
        Path = E._4,
        Level = E._5,
        Status = E._6,
        Phone = E._7,
        Address = E._8,
        Latitude = E._9,
        Longitude = E._10,
        RowId = E._11,
    }
}
