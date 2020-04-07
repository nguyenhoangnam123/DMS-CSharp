using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StatusDAO
    {
        public StatusDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
            Brands = new HashSet<BrandDAO>();
            Districts = new HashSet<DistrictDAO>();
            Organizations = new HashSet<OrganizationDAO>();
            Permissions = new HashSet<PermissionDAO>();
            ProductTypes = new HashSet<ProductTypeDAO>();
            Products = new HashSet<ProductDAO>();
            Provinces = new HashSet<ProvinceDAO>();
            Resellers = new HashSet<ResellerDAO>();
            Roles = new HashSet<RoleDAO>();
            Sexes = new HashSet<SexDAO>();
            StoreGroupings = new HashSet<StoreGroupingDAO>();
            StoreTypes = new HashSet<StoreTypeDAO>();
            Stores = new HashSet<StoreDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            TaxTypes = new HashSet<TaxTypeDAO>();
            UnitOfMeasureGroupings = new HashSet<UnitOfMeasureGroupingDAO>();
            UnitOfMeasures = new HashSet<UnitOfMeasureDAO>();
            Wards = new HashSet<WardDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<BrandDAO> Brands { get; set; }
        public virtual ICollection<DistrictDAO> Districts { get; set; }
        public virtual ICollection<OrganizationDAO> Organizations { get; set; }
        public virtual ICollection<PermissionDAO> Permissions { get; set; }
        public virtual ICollection<ProductTypeDAO> ProductTypes { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<ProvinceDAO> Provinces { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<RoleDAO> Roles { get; set; }
        public virtual ICollection<SexDAO> Sexes { get; set; }
        public virtual ICollection<StoreGroupingDAO> StoreGroupings { get; set; }
        public virtual ICollection<StoreTypeDAO> StoreTypes { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<TaxTypeDAO> TaxTypes { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
        public virtual ICollection<UnitOfMeasureDAO> UnitOfMeasures { get; set; }
        public virtual ICollection<WardDAO> Wards { get; set; }
    }
}
