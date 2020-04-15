﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class OrganizationDAO
    {
        public OrganizationDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            InverseParent = new HashSet<OrganizationDAO>();
            Resellers = new HashSet<ResellerDAO>();
            Stores = new HashSet<StoreDAO>();
            Warehouses = new HashSet<WarehouseDAO>();
        }

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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual OrganizationDAO Parent { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<OrganizationDAO> InverseParent { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<WarehouseDAO> Warehouses { get; set; }
    }
}
