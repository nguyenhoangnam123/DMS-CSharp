using DMS.Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.showing_warehouse
{
    public class ShowingWarehouse_ShowingInventoryHistoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long ShowingInventoryId { get; set; }
        public long OldSaleStock { get; set; }
        public long SaleStock { get; set; }
        public long OldAccountingStock { get; set; }
        public long AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ShowingWarehouse_AppUserDTO AppUser { get; set; }
        public ShowingWarehouse_ShowingInventoryDTO Inventory { get; set; }
        public ShowingWarehouse_ShowingInventoryHistoryDTO() { }
        public ShowingWarehouse_ShowingInventoryHistoryDTO(ShowingInventoryHistory ShowingInventoryHistory)
        {
            this.Id = ShowingInventoryHistory.Id;
            this.ShowingInventoryId = ShowingInventoryHistory.ShowingInventoryId;
            this.OldSaleStock = ShowingInventoryHistory.OldSaleStock;
            this.SaleStock = ShowingInventoryHistory.SaleStock;
            this.OldAccountingStock = ShowingInventoryHistory.OldAccountingStock;
            this.AccountingStock = ShowingInventoryHistory.AccountingStock;
            this.AppUserId = ShowingInventoryHistory.AppUserId;
            this.UpdatedAt = ShowingInventoryHistory.UpdatedAt;
            this.AppUser = ShowingInventoryHistory.AppUser == null ? null : new ShowingWarehouse_AppUserDTO(ShowingInventoryHistory.AppUser);
            this.Inventory = ShowingInventoryHistory.ShowingInventory == null ? null : new ShowingWarehouse_ShowingInventoryDTO(ShowingInventoryHistory.ShowingInventory);
            this.Errors = ShowingInventoryHistory.Errors;
        }
    }

    public class ShowingWarehouse_ShowingInventoryHistoryFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ShowingInventoryId { get; set; }
        public LongFilter OldSaleStock { get; set; }
        public LongFilter SaleStock { get; set; }
        public LongFilter OldAccountingStock { get; set; }
        public LongFilter AccountingStock { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ShowingInventoryHistoryOrder OrderBy { get; set; }
    }
}
