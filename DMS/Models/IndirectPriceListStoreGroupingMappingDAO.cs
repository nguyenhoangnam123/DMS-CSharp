namespace DMS.Models
{
    public partial class IndirectPriceListStoreGroupingMappingDAO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }

        public virtual IndirectPriceListDAO IndirectPriceList { get; set; }
        public virtual StoreGroupingDAO StoreGrouping { get; set; }
    }
}
