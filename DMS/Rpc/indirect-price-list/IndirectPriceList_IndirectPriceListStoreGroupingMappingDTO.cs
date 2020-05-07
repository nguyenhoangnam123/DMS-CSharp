using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListStoreGroupingMappingDTO : DataDTO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }
        public IndirectPriceList_StoreGroupingDTO StoreGrouping { get; set; }   
        
        public IndirectPriceList_IndirectPriceListStoreGroupingMappingDTO() {}
        public IndirectPriceList_IndirectPriceListStoreGroupingMappingDTO(IndirectPriceListStoreGroupingMapping IndirectPriceListStoreGroupingMapping)
        {
            this.IndirectPriceListId = IndirectPriceListStoreGroupingMapping.IndirectPriceListId;
            this.StoreGroupingId = IndirectPriceListStoreGroupingMapping.StoreGroupingId;
            this.StoreGrouping = IndirectPriceListStoreGroupingMapping.StoreGrouping == null ? null : new IndirectPriceList_StoreGroupingDTO(IndirectPriceListStoreGroupingMapping.StoreGrouping);
            this.Errors = IndirectPriceListStoreGroupingMapping.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListStoreGroupingMappingFilterDTO : FilterDTO
    {
        
        public IdFilter IndirectPriceListId { get; set; }
        
        public IdFilter StoreGroupingId { get; set; }
        
        public IndirectPriceListStoreGroupingMappingOrder OrderBy { get; set; }
    }
}