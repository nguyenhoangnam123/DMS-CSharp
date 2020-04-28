using Common;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_ItemDTO : DataDTO
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public bool CanDelete { get; set; }
        public bool HasInventory { get; set; }
        public long StatusId { get; set; }

        public Product_ItemDTO() { }
        public Product_ItemDTO(Item Item)
        {
            this.Id = Item.Id;
            this.ProductId = Item.ProductId;
            this.Code = Item.Code;
            this.Name = Item.Name;
            this.ScanCode = Item.ScanCode;
            this.SalePrice = Item.SalePrice;
            this.RetailPrice = Item.RetailPrice;
            this.CanDelete = Item.CanDelete;
            this.HasInventory = Item.HasInventory;
            this.StatusId = Item.StatusId;
            this.Errors = Item.Errors;
        }
    }

    public class Product_ItemFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter ProductId { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter ScanCode { get; set; }

        public DecimalFilter SalePrice { get; set; }

        public DecimalFilter RetailPrice { get; set; }

        public ItemOrder OrderBy { get; set; }
    }
}