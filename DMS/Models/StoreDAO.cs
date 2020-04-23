using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreDAO
    {
        public StoreDAO()
        {
            InverseParentStore = new HashSet<StoreDAO>();
            StoreImageMappings = new HashSet<StoreImageMappingDAO>();
            StoreWorkflowParameterMappings = new HashSet<StoreWorkflowParameterMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentStoreId { get; set; }
        public long OrganizationId { get; set; }
        public long StoreTypeId { get; set; }
        public long? StoreGroupingId { get; set; }
        public long? ResellerId { get; set; }
        public string Telephone { get; set; }
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
        public long StoreStatusId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long? WorkflowDefinitionId { get; set; }
        public long? RequestStateId { get; set; }

        public virtual DistrictDAO District { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StoreDAO ParentStore { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual RequestStateDAO RequestState { get; set; }
        public virtual ResellerDAO Reseller { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual StoreGroupingDAO StoreGrouping { get; set; }
        public virtual StoreStatusDAO StoreStatus { get; set; }
        public virtual StoreTypeDAO StoreType { get; set; }
        public virtual WardDAO Ward { get; set; }
        public virtual ICollection<StoreDAO> InverseParentStore { get; set; }
        public virtual ICollection<StoreImageMappingDAO> StoreImageMappings { get; set; }
        public virtual ICollection<StoreWorkflowParameterMappingDAO> StoreWorkflowParameterMappings { get; set; }
    }
}
