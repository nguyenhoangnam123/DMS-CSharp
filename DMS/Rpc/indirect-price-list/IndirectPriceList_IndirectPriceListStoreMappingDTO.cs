using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListStoreMappingDTO : DataDTO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreId { get; set; }
        public IndirectPriceList_StoreDTO Store { get; set; }

        public IndirectPriceList_IndirectPriceListStoreMappingDTO() { }
        public IndirectPriceList_IndirectPriceListStoreMappingDTO(IndirectPriceListStoreMapping IndirectPriceListStoreMapping)
        {
            this.IndirectPriceListId = IndirectPriceListStoreMapping.IndirectPriceListId;
            this.StoreId = IndirectPriceListStoreMapping.StoreId;
            this.Store = IndirectPriceListStoreMapping.Store == null ? null : new IndirectPriceList_StoreDTO(IndirectPriceListStoreMapping.Store);
            this.Errors = IndirectPriceListStoreMapping.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListStoreMappingFilterDTO : FilterDTO
    {

        public IdFilter IndirectPriceListId { get; set; }

        public IdFilter StoreId { get; set; }

        public IndirectPriceListStoreMappingOrder OrderBy { get; set; }
    }
}