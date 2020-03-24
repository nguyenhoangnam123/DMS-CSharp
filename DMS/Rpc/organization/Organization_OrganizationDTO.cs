using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.organization
{
    public class Organization_OrganizationDTO : DataDTO
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
        public Organization_OrganizationDTO Parent { get; set; }
        public Organization_StatusDTO Status { get; set; }
        public List<Organization_StoreDTO> Stores { get; set; }
        public Organization_OrganizationDTO() {}
        public Organization_OrganizationDTO(Organization Organization)
        {
            this.Id = Organization.Id;
            this.Code = Organization.Code;
            this.Name = Organization.Name;
            this.ParentId = Organization.ParentId;
            this.Path = Organization.Path;
            this.Level = Organization.Level;
            this.StatusId = Organization.StatusId;
            this.Phone = Organization.Phone;
            this.Address = Organization.Address;
            this.Latitude = Organization.Latitude;
            this.Longitude = Organization.Longitude;
            this.Parent = Organization.Parent == null ? null : new Organization_OrganizationDTO(Organization.Parent);
            this.Status = Organization.Status == null ? null : new Organization_StatusDTO(Organization.Status);
            this.Stores = Organization.Stores?.Select(x => new Organization_StoreDTO(x)).ToList();
        }
    }

    public class Organization_OrganizationFilterDTO : FilterDTO
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
        public OrganizationOrder OrderBy { get; set; }
    }
}
