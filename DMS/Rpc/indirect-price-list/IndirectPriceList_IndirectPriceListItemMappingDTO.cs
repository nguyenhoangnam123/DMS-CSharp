using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListItemMappingDTO : DataDTO
    {
        public long IndirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public IndirectPriceList_ItemDTO Item { get; set; }

        public IndirectPriceList_IndirectPriceListItemMappingDTO() { }
        public IndirectPriceList_IndirectPriceListItemMappingDTO(IndirectPriceListItemMapping IndirectPriceListItemMapping)
        {
            this.IndirectPriceListId = IndirectPriceListItemMapping.IndirectPriceListId;
            this.ItemId = IndirectPriceListItemMapping.ItemId;
            this.Price = IndirectPriceListItemMapping.Price;
            this.Item = IndirectPriceListItemMapping.Item == null ? null : new IndirectPriceList_ItemDTO(IndirectPriceListItemMapping.Item);
            this.Errors = IndirectPriceListItemMapping.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListItemMappingFilterDTO : FilterDTO
    {

        public IdFilter IndirectPriceListId { get; set; }

        public IdFilter ItemId { get; set; }

        public LongFilter Price { get; set; }

        public IndirectPriceListItemMappingOrder OrderBy { get; set; }
    }
}