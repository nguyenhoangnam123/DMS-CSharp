using Common;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListStoreGroupingMappingDTO : DataDTO
    {
        public long DirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }
        public DirectPriceList_StoreGroupingDTO StoreGrouping { get; set; }

        public DirectPriceList_DirectPriceListStoreGroupingMappingDTO() { }
        public DirectPriceList_DirectPriceListStoreGroupingMappingDTO(DirectPriceListStoreGroupingMapping DirectPriceListStoreGroupingMapping)
        {
            this.DirectPriceListId = DirectPriceListStoreGroupingMapping.DirectPriceListId;
            this.StoreGroupingId = DirectPriceListStoreGroupingMapping.StoreGroupingId;
            this.StoreGrouping = DirectPriceListStoreGroupingMapping.StoreGrouping == null ? null : new DirectPriceList_StoreGroupingDTO(DirectPriceListStoreGroupingMapping.StoreGrouping);
            this.Errors = DirectPriceListStoreGroupingMapping.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListStoreGroupingMappingFilterDTO : FilterDTO
    {

        public IdFilter DirectPriceListId { get; set; }

        public IdFilter StoreGroupingId { get; set; }

        public DirectPriceListStoreGroupingMappingOrder OrderBy { get; set; }
    }
}