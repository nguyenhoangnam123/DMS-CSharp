using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_ItemDTO : DataDTO
    {

        public long Id { get; set; }

        public long ProductId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ScanCode { get; set; }

        public long SalePrice { get; set; }

        public long? RetailPrice { get; set; }

        public long StatusId { get; set; }


        public IndirectPriceList_ItemDTO() { }
        public IndirectPriceList_ItemDTO(Item Item)
        {

            this.Id = Item.Id;

            this.ProductId = Item.ProductId;

            this.Code = Item.Code;

            this.Name = Item.Name;

            this.ScanCode = Item.ScanCode;

            this.SalePrice = Item.SalePrice;

            this.RetailPrice = Item.RetailPrice;

            this.StatusId = Item.StatusId;

            this.Errors = Item.Errors;
        }
    }

    public class IndirectPriceList_ItemFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter ProductId { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public StringFilter ScanCode { get; set; }

        public LongFilter SalePrice { get; set; }

        public LongFilter RetailPrice { get; set; }

        public IdFilter StatusId { get; set; }

        public ItemOrder OrderBy { get; set; }
    }
}