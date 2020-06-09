using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListStoreTypeMappingDTO : DataDTO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }
        public IndirectPriceList_StoreTypeDTO StoreType { get; set; }

        public IndirectPriceList_IndirectPriceListStoreTypeMappingDTO() { }
        public IndirectPriceList_IndirectPriceListStoreTypeMappingDTO(IndirectPriceListStoreTypeMapping IndirectPriceListStoreTypeMapping)
        {
            this.IndirectPriceListId = IndirectPriceListStoreTypeMapping.IndirectPriceListId;
            this.StoreTypeId = IndirectPriceListStoreTypeMapping.StoreTypeId;
            this.StoreType = IndirectPriceListStoreTypeMapping.StoreType == null ? null : new IndirectPriceList_StoreTypeDTO(IndirectPriceListStoreTypeMapping.StoreType);
            this.Errors = IndirectPriceListStoreTypeMapping.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListStoreTypeMappingFilterDTO : FilterDTO
    {

        public IdFilter IndirectPriceListId { get; set; }

        public IdFilter StoreTypeId { get; set; }

        public IndirectPriceListStoreTypeMappingOrder OrderBy { get; set; }
    }
}