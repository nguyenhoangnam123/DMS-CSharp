﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProductDAO
    {
        public ProductDAO()
        {
            Items = new HashSet<ItemDAO>();
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
            ProductProductGroupingMappings = new HashSet<ProductProductGroupingMappingDAO>();
            VariationGroupings = new HashSet<VariationGroupingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string SupplierCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScanCode { get; set; }
        public string ERPCode { get; set; }
        public long ProductTypeId { get; set; }
        public long? SupplierId { get; set; }
        public long? BrandId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? UnitOfMeasureGroupingId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long TaxTypeId { get; set; }
        public long StatusId { get; set; }
        public string OtherName { get; set; }
        public string TechnicalName { get; set; }
        public string Note { get; set; }
        public bool IsNew { get; set; }
        public long UsedVariationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }

        public virtual ICollection<ItemDAO> Items { get; set; }
        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
        public virtual ICollection<ProductProductGroupingMappingDAO> ProductProductGroupingMappings { get; set; }
        public virtual ICollection<VariationGroupingDAO> VariationGroupings { get; set; }
    }
}
