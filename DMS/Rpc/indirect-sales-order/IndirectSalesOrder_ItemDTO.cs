using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_ItemDTO : DataDTO
    {
        
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long SaleStock { get; set; }
        public long StatusId { get; set; }
        public IndirectSalesOrder_ProductDTO Product { get; set; }
        public IndirectSalesOrder_ItemDTO() {}
        public IndirectSalesOrder_ItemDTO(Item Item)
        {
            this.Id = Item.Id;
            this.ProductId = Item.ProductId;
            this.Code = Item.Code;
            this.Name = Item.Name;
            this.ScanCode = Item.ScanCode;
            this.SalePrice = Item.SalePrice;
            this.RetailPrice = Item.RetailPrice;
            this.SaleStock = Item.SaleStock;
            this.StatusId = Item.StatusId;
            this.Product = Item.Product == null ? null : new IndirectSalesOrder_ProductDTO(Item.Product);
            this.Errors = Item.Errors;
        }
    }

    public class IndirectSalesOrder_ItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ProductId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter ScanCode { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter StatusId { get; set; }
        public ItemOrder OrderBy { get; set; }
    }
}