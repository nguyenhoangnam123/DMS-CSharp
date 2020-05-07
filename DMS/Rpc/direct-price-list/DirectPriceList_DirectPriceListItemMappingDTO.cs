using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListItemMappingDTO : DataDTO
    {
        public long DirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }
        public DirectPriceList_ItemDTO Item { get; set; }   
        
        public DirectPriceList_DirectPriceListItemMappingDTO() {}
        public DirectPriceList_DirectPriceListItemMappingDTO(DirectPriceListItemMapping DirectPriceListItemMapping)
        {
            this.DirectPriceListId = DirectPriceListItemMapping.DirectPriceListId;
            this.ItemId = DirectPriceListItemMapping.ItemId;
            this.Price = DirectPriceListItemMapping.Price;
            this.Item = DirectPriceListItemMapping.Item == null ? null : new DirectPriceList_ItemDTO(DirectPriceListItemMapping.Item);
            this.Errors = DirectPriceListItemMapping.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListItemMappingFilterDTO : FilterDTO
    {
        
        public IdFilter DirectPriceListId { get; set; }
        
        public IdFilter ItemId { get; set; }
        
        public LongFilter Price { get; set; }
        
        public DirectPriceListItemMappingOrder OrderBy { get; set; }
    }
}