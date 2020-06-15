﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StatusDAO
    {
        public StatusDAO()
        {
            Albums = new HashSet<AlbumDAO>();
            AppUsers = new HashSet<AppUserDAO>();
            Banners = new HashSet<BannerDAO>();
            Brands = new HashSet<BrandDAO>();
            DirectPriceLists = new HashSet<DirectPriceListDAO>();
            Districts = new HashSet<DistrictDAO>();
            ERoutes = new HashSet<ERouteDAO>();
            IndirectPriceLists = new HashSet<IndirectPriceListDAO>();
            Items = new HashSet<ItemDAO>();
            KpiGeneralContents = new HashSet<KpiGeneralContentDAO>();
            KpiGenerals = new HashSet<KpiGeneralDAO>();
            KpiItems = new HashSet<KpiItemDAO>();
            Organizations = new HashSet<OrganizationDAO>();
            Permissions = new HashSet<PermissionDAO>();
            Positions = new HashSet<PositionDAO>();
            ProductTypes = new HashSet<ProductTypeDAO>();
            Products = new HashSet<ProductDAO>();
            Provinces = new HashSet<ProvinceDAO>();
            Resellers = new HashSet<ResellerDAO>();
            Roles = new HashSet<RoleDAO>();
            StoreGroupings = new HashSet<StoreGroupingDAO>();
            StoreTypes = new HashSet<StoreTypeDAO>();
            Stores = new HashSet<StoreDAO>();
            Suppliers = new HashSet<SupplierDAO>();
            Surveys = new HashSet<SurveyDAO>();
            TaxTypes = new HashSet<TaxTypeDAO>();
            UnitOfMeasureGroupings = new HashSet<UnitOfMeasureGroupingDAO>();
            UnitOfMeasures = new HashSet<UnitOfMeasureDAO>();
            Wards = new HashSet<WardDAO>();
            Warehouses = new HashSet<WarehouseDAO>();
            WorkflowDefinitions = new HashSet<WorkflowDefinitionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AlbumDAO> Albums { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
        public virtual ICollection<BannerDAO> Banners { get; set; }
        public virtual ICollection<BrandDAO> Brands { get; set; }
        public virtual ICollection<DirectPriceListDAO> DirectPriceLists { get; set; }
        public virtual ICollection<DistrictDAO> Districts { get; set; }
        public virtual ICollection<ERouteDAO> ERoutes { get; set; }
        public virtual ICollection<IndirectPriceListDAO> IndirectPriceLists { get; set; }
        public virtual ICollection<ItemDAO> Items { get; set; }
        public virtual ICollection<KpiGeneralContentDAO> KpiGeneralContents { get; set; }
        public virtual ICollection<KpiGeneralDAO> KpiGenerals { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItems { get; set; }
        public virtual ICollection<OrganizationDAO> Organizations { get; set; }
        public virtual ICollection<PermissionDAO> Permissions { get; set; }
        public virtual ICollection<PositionDAO> Positions { get; set; }
        public virtual ICollection<ProductTypeDAO> ProductTypes { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
        public virtual ICollection<ProvinceDAO> Provinces { get; set; }
        public virtual ICollection<ResellerDAO> Resellers { get; set; }
        public virtual ICollection<RoleDAO> Roles { get; set; }
        public virtual ICollection<StoreGroupingDAO> StoreGroupings { get; set; }
        public virtual ICollection<StoreTypeDAO> StoreTypes { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<SupplierDAO> Suppliers { get; set; }
        public virtual ICollection<SurveyDAO> Surveys { get; set; }
        public virtual ICollection<TaxTypeDAO> TaxTypes { get; set; }
        public virtual ICollection<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupings { get; set; }
        public virtual ICollection<UnitOfMeasureDAO> UnitOfMeasures { get; set; }
        public virtual ICollection<WardDAO> Wards { get; set; }
        public virtual ICollection<WarehouseDAO> Warehouses { get; set; }
        public virtual ICollection<WorkflowDefinitionDAO> WorkflowDefinitions { get; set; }
    }
}
