using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Store : DataEntity, IEquatable<Store>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentStoreId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreTypeId { get; set; }
        public long? StoreGroupingId { get; set; }
        public string Telephone { get; set; }
        public long? ResellerId { get; set; }
        public long ProvinceId { get; set; }
        public long DistrictId { get; set; }
        public long WardId { get; set; }
        public string Address { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public long StatusId { get; set; }
        public long StoreStatusId { get; set; }
        public District District { get; set; }
        public Organization Organization { get; set; }
        public Store ParentStore { get; set; }
        public Province Province { get; set; }
        public Status Status { get; set; }
        public StoreGrouping StoreGrouping { get; set; }
        public StoreType StoreType { get; set; }
        public Ward Ward { get; set; }
        public List<StoreImageMapping> StoreImageMappings { get; set; }

        public bool Equals(Store other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class StoreFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentStoreId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public StringFilter Telephone { get; set; }
        public IdFilter ResellerId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DecimalFilter Longitude { get; set; }
        public StringFilter OwnerName { get; set; }
        public StringFilter OwnerPhone { get; set; }
        public StringFilter OwnerEmail { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter StoreStatusId { get; set; }
        public List<StoreFilter> OrFilter { get; set; }
        public StoreOrder OrderBy { get; set; }
        public StoreSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        ParentStore = 3,
        Organization = 4,
        StoreType = 5,
        StoreGrouping = 6,
        Telephone = 7,
        Reseller = 8,
        Province = 9,
        District = 10,
        Ward = 11,
        Address = 12,
        DeliveryAddress = 13,
        Latitude = 14,
        Longitude = 15,
        OwnerName = 16,
        OwnerPhone = 17,
        OwnerEmail = 18,
        Status = 19,
        StoreStatus = 20,
    }

    [Flags]
    public enum StoreSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        ParentStore = E._3,
        Organization = E._4,
        StoreType = E._5,
        StoreGrouping = E._6,
        Telephone = E._7,
        Reseller = 8,
        Province = E._9,
        District = E._10,
        Ward = E._11,
        Address = E._12,
        DeliveryAddress = E._13,
        Latitude = E._14,
        Longitude = E._15,
        OwnerName = E._16,
        OwnerPhone = E._17,
        OwnerEmail = E._18,
        Status = E._19,
        StoreStatus = E._20,
    }
}
