using Common;
using DMS.Entities;
using DMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProductDTO : DataDTO
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
        public MobileSync_ProductTypeDTO ProductType { get; set; }
        public MobileSync_SupplierDTO Supplier { get; set; }
        public MobileSync_TaxTypeDTO TaxType { get; set; }
        public MobileSync_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public MobileSync_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping { get; set; }
        public List<MobileSync_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public MobileSync_ProductDTO() { }
        public MobileSync_ProductDTO(ProductDAO ProductDAO)
        {
            this.Id = ProductDAO.Id;
            this.Code = ProductDAO.Code;
            this.SupplierCode = ProductDAO.SupplierCode;
            this.ERPCode = ProductDAO.ERPCode;
            this.Name = ProductDAO.Name;
            this.Description = ProductDAO.Description;
            this.ScanCode = ProductDAO.ScanCode;
            this.ProductTypeId = ProductDAO.ProductTypeId;
            this.SupplierId = ProductDAO.SupplierId;
            this.BrandId = ProductDAO.BrandId;
            this.UnitOfMeasureId = ProductDAO.UnitOfMeasureId;
            this.UnitOfMeasureGroupingId = ProductDAO.UnitOfMeasureGroupingId;
            this.SalePrice = ProductDAO.SalePrice;
            this.RetailPrice = ProductDAO.RetailPrice;
            this.TaxTypeId = ProductDAO.TaxTypeId;
            this.StatusId = ProductDAO.StatusId;
            this.OtherName = ProductDAO.OtherName;
            this.TechnicalName = ProductDAO.TechnicalName;
            this.Note = ProductDAO.Note;
            this.ProductType = ProductDAO.ProductType == null ? null : new MobileSync_ProductTypeDTO(ProductDAO.ProductType);
            this.Supplier = ProductDAO.Supplier == null ? null : new MobileSync_SupplierDTO(ProductDAO.Supplier);
            this.TaxType = ProductDAO.TaxType == null ? null : new MobileSync_TaxTypeDTO(ProductDAO.TaxType);
            this.UnitOfMeasure = ProductDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(ProductDAO.UnitOfMeasure);
            this.UnitOfMeasureGrouping = ProductDAO.UnitOfMeasureGrouping == null ? null : new MobileSync_UnitOfMeasureGroupingDTO(ProductDAO.UnitOfMeasureGrouping);
            this.ProductProductGroupingMappings = ProductDAO.ProductProductGroupingMappings?.Select(x => new MobileSync_ProductProductGroupingMappingDTO(x)).ToList();
        }
    }

    public class MobileSync_ProductFilterDTO : FilterDTO
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
