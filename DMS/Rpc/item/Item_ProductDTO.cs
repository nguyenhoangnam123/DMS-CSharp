using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item
{
    public class Item_ProductDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string SupplierCode { get; set; }
        
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
        
        public long? TaxTypeId { get; set; }
        
        public long StatusId { get; set; }
        

        public Item_ProductDTO() {}
        public Item_ProductDTO(Product Product)
        {
            
            this.Id = Product.Id;
            
            this.Code = Product.Code;
            
            this.SupplierCode = Product.SupplierCode;
            
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
            
        }
    }

    public class Item_ProductFilterDTO : FilterDTO
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
        
        public ProductOrder OrderBy { get; set; }
    }
}