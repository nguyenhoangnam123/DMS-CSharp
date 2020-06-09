using Common;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListStoreMappingDTO : DataDTO
    {
        public long DirectPriceListId { get; set; }
        public long StoreId { get; set; }
        public DirectPriceList_StoreDTO Store { get; set; }

        public DirectPriceList_DirectPriceListStoreMappingDTO() { }
        public DirectPriceList_DirectPriceListStoreMappingDTO(DirectPriceListStoreMapping DirectPriceListStoreMapping)
        {
            this.DirectPriceListId = DirectPriceListStoreMapping.DirectPriceListId;
            this.StoreId = DirectPriceListStoreMapping.StoreId;
            this.Store = DirectPriceListStoreMapping.Store == null ? null : new DirectPriceList_StoreDTO(DirectPriceListStoreMapping.Store);
            this.Errors = DirectPriceListStoreMapping.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListStoreMappingFilterDTO : FilterDTO
    {

        public IdFilter DirectPriceListId { get; set; }

        public IdFilter StoreId { get; set; }

        public DirectPriceListStoreMappingOrder OrderBy { get; set; }
    }
}