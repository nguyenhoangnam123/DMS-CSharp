using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.price_list
{
    public class PriceList_PriceListItemHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long PriceListId { get; set; }
        public long ItemId { get; set; }
        public long ModifierId { get; set; }
        public long OldPrice { get; set; }
        public long NewPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Source { get; set; }
        public PriceList_AppUserDTO Modifier { get; set; }
        public PriceList_PriceListItemHistoryDTO() { }
        public PriceList_PriceListItemHistoryDTO(PriceListItemHistory PriceListItemHistory)
        {
            this.Id = PriceListItemHistory.Id;
            this.PriceListId = PriceListItemHistory.PriceListId;
            this.ItemId = PriceListItemHistory.ItemId;
            this.ModifierId = PriceListItemHistory.ModifierId;
            this.OldPrice = PriceListItemHistory.OldPrice;
            this.NewPrice = PriceListItemHistory.NewPrice;
            this.UpdatedAt = PriceListItemHistory.UpdatedAt;
            this.Source = PriceListItemHistory.Source;
            this.Modifier = PriceListItemHistory.Modifier == null ? null : new PriceList_AppUserDTO(PriceListItemHistory.Modifier);
        }
    }

    public class PriceList_PriceListItemHistoryFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter PriceListId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter ModifierId { get; set; }
        public LongFilter OldPrice { get; set; }
        public LongFilter NewPrice { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public StringFilter Source { get; set; }
        public ItemHistoryOrder OrderBy { get; set; }
    }
}
