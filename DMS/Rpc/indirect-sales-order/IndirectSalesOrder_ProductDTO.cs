﻿using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_ProductDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string SupplierCode { get; set; }
        public string ERPCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScanCode { get; set; }
        public long ProductTypeId { get; set; }
        public long? SupplierId { get; set; }
        public long? BrandId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? UnitOfMeasureGroupingId { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long TaxTypeId { get; set; }
        public long StatusId { get; set; }
        public string OtherName { get; set; }
        public string TechnicalName { get; set; }
        public string Note { get; set; }
        public IndirectSalesOrder_ProductTypeDTO ProductType { get; set; }
        public IndirectSalesOrder_SupplierDTO Supplier { get; set; }
        public IndirectSalesOrder_TaxTypeDTO TaxType { get; set; }
        public IndirectSalesOrder_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public IndirectSalesOrder_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping { get; set; }
        public List<IndirectSalesOrder_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public IndirectSalesOrder_ProductDTO() { }
        public IndirectSalesOrder_ProductDTO(Product Product)
        {
            this.Id = Product.Id;
            this.Code = Product.Code;
            this.SupplierCode = Product.SupplierCode;
            this.ERPCode = Product.ERPCode;
            this.Name = Product.Name;
            this.Description = Product.Description;
            this.ScanCode = Product.ScanCode;
            this.ProductTypeId = Product.ProductTypeId;
            this.SupplierId = Product.SupplierId;
            this.BrandId = Product.BrandId;
            this.UnitOfMeasureId = Product.UnitOfMeasureId;
            this.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
            this.SalePrice = Product.SalePrice;
            this.RetailPrice = Product.RetailPrice;
            this.TaxTypeId = Product.TaxTypeId;
            this.StatusId = Product.StatusId;
            this.OtherName = Product.OtherName;
            this.TechnicalName = Product.TechnicalName;
            this.Note = Product.Note;
            this.ProductType = Product.ProductType == null ? null : new IndirectSalesOrder_ProductTypeDTO(Product.ProductType);
            this.Supplier = Product.Supplier == null ? null : new IndirectSalesOrder_SupplierDTO(Product.Supplier);
            this.TaxType = Product.TaxType == null ? null : new IndirectSalesOrder_TaxTypeDTO(Product.TaxType);
            this.UnitOfMeasure = Product.UnitOfMeasure == null ? null : new IndirectSalesOrder_UnitOfMeasureDTO(Product.UnitOfMeasure);
            this.UnitOfMeasureGrouping = Product.UnitOfMeasureGrouping == null ? null : new IndirectSalesOrder_UnitOfMeasureGroupingDTO(Product.UnitOfMeasureGrouping);
            this.ProductProductGroupingMappings = Product.ProductProductGroupingMappings?.Select(x => new IndirectSalesOrder_ProductProductGroupingMappingDTO(x)).ToList();
            this.Errors = Product.Errors;
        }
    }

    public class IndirectSalesOrder_ProductFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter SupplierCode { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ScanCode { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter BrandId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter UnitOfMeasureGroupingId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter TaxTypeId { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter TechnicalName { get; set; }
        public StringFilter Note { get; set; }
        public IdFilter ProductGroupingId { get; set; }

        public string Search { get; set; }
        public ProductOrder OrderBy { get; set; }
    }
}
