﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProvinceDAO
    {
        public ProvinceDAO()
        {
            Districts = new HashSet<DistrictDAO>();
            Stores = new HashSet<StoreDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            Warehouses = new HashSet<WarehouseDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<DistrictDAO> Districts { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<WarehouseDAO> Warehouses { get; set; }
    }
}
