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
        public decimal? DeliveryLatitude { get; set; }
        public decimal? DeliveryLongitude { get; set; }
        public string OwnerName { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerEmail { get; set; }
        public string TaxCode { get; set; }
        public string LegalEntity { get; set; }
        public long StatusId { get; set; }
        public Guid RowId { get; set; }
        public long? RequestStateId { get; set; }
        public District District { get; set; }
        public Organization Organization { get; set; }
        public Store ParentStore { get; set; }
        public Reseller Reseller { get; set; }
        public Province Province { get; set; }
        public Status Status { get; set; }
        public StoreGrouping StoreGrouping { get; set; }
        public StoreType StoreType { get; set; }
        public Ward Ward { get; set; }
        public RequestState RequestState { get; set; }
        public List<StoreImageMapping> StoreImageMappings { get; set; }
        public List<RequestWorkflowStepMapping> StoreWorkflows { get; set; }
        public List<RequestWorkflowStepMapping> RequestWorkflowStepMappings { get; set; }

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
        public DecimalFilter DeliveryLatitude { get; set; }
        public DecimalFilter DeliveryLongitude { get; set; }
        public StringFilter OwnerName { get; set; }
        public StringFilter OwnerPhone { get; set; }
        public StringFilter OwnerEmail { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter WorkflowDefinitionId { get; set; }
        public IdFilter RequestStateId { get; set; }
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
        Reseller = 7,
        Telephone = 8,
        Province = 9,
        District = 10,
        Ward = 11,
        Address = 12,
        DeliveryAddress = 13,
        Latitude = 14,
        Longitude = 15,
        DeliveryLatitude = 16,
        DeliveryLongitude = 17,
        OwnerName = 18,
        OwnerPhone = 19,
        OwnerEmail = 20,
        Status = 21,
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
        Reseller = E._6,
        Telephone = E._8,
        Province = E._9,
        District = E._10,
        Ward = E._11,
        Address = E._12,
        DeliveryAddress = E._13,
        Latitude = E._14,
        Longitude = E._15,
        DeliveryLatitude = E._16,
        DeliveryLongitude = E._17,
        OwnerName = E._18,
        OwnerPhone = E._19,
        OwnerEmail = E._20,
        Status = E._21,
        StoreImageMappings = E._23,
        TaxCode = E._24,
        LegalEntity = E._25
    }
}
