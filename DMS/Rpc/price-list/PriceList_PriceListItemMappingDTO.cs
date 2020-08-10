using Common;
using DMS.Entities;

namespace DMS.Rpc.price_list
{
    public class PriceList_PriceListItemMappingDTO : DataDTO
    {
        public long PriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public PriceList_ItemDTO Item { get; set; }

        public PriceList_PriceListItemMappingDTO() { }
        public PriceList_PriceListItemMappingDTO(PriceListItemMapping PriceListItemMapping)
        {
            this.PriceListId = PriceListItemMapping.PriceListId;
            this.ItemId = PriceListItemMapping.ItemId;
            this.Price = PriceListItemMapping.Price;
            this.Item = PriceListItemMapping.Item == null ? null : new PriceList_ItemDTO(PriceListItemMapping.Item);
            this.Errors = PriceListItemMapping.Errors;
        }
    }

    public class PriceList_PriceListItemMappingFilterDTO : FilterDTO
    {

        public IdFilter PriceListId { get; set; }

        public IdFilter ItemId { get; set; }

        public LongFilter Price { get; set; }

        public PriceListItemMappingOrder OrderBy { get; set; }
    }
}