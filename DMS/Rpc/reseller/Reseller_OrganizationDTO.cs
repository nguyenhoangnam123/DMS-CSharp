﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reseller
{
    public class Reseller_OrganizationDTO : DataDTO
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
        public Reseller_OrganizationDTO Parent { get; set; }
        public Reseller_OrganizationDTO() { }
        public Reseller_OrganizationDTO(Organization Organization)
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
            this.Parent = Organization.Parent == null ? null : new Reseller_OrganizationDTO(Organization.Parent);
            this.Errors = Organization.Errors;
        }
    }

    public class Reseller_OrganizationFilterDTO : FilterDTO
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