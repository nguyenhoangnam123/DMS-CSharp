﻿using Common;
using DMS.Entities;


namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ItemDTO : DataDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public long SalePrice { get; set; }
        public long? RetailPrice { get; set; }
        public long SaleStock { get; set; }
        public long StatusId { get; set; }
        public Mobile_ProductDTO Product { get; set; }
        public StoreChecking_ItemDTO() { }
        public StoreChecking_ItemDTO(Item Item)
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
            this.Product = Item.Product == null ? null : new Mobile_ProductDTO(Item.Product);
            this.Errors = Item.Errors;
        }
    }

    public class StoreChecking_ItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ProductId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter ScanCode { get; set; }
        public LongFilter SalePrice { get; set; }
        public LongFilter RetailPrice { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter StoreId { get; set; }
        public ItemOrder OrderBy { get; set; }
    }
}
