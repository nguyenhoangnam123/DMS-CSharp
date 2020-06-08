using Common;
using DMS.Entities;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceList_DirectPriceListStoreTypeMappingDTO : DataDTO
    {
        public long DirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }
        public DirectPriceList_StoreTypeDTO StoreType { get; set; }

        public DirectPriceList_DirectPriceListStoreTypeMappingDTO() { }
        public DirectPriceList_DirectPriceListStoreTypeMappingDTO(DirectPriceListStoreTypeMapping DirectPriceListStoreTypeMapping)
        {
            this.DirectPriceListId = DirectPriceListStoreTypeMapping.DirectPriceListId;
            this.StoreTypeId = DirectPriceListStoreTypeMapping.StoreTypeId;
            this.StoreType = DirectPriceListStoreTypeMapping.StoreType == null ? null : new DirectPriceList_StoreTypeDTO(DirectPriceListStoreTypeMapping.StoreType);
            this.Errors = DirectPriceListStoreTypeMapping.Errors;
        }
    }

    public class DirectPriceList_DirectPriceListStoreTypeMappingFilterDTO : FilterDTO
    {

        public IdFilter DirectPriceListId { get; set; }

        public IdFilter StoreTypeId { get; set; }

        public DirectPriceListStoreTypeMappingOrder OrderBy { get; set; }
    }
}