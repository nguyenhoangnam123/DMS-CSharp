using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.showing_warehouse
{
    public class ShowingWarehouse_ShowingInventoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long ShowingWarehouseId { get; set; }
        public long ShowingItemId { get; set; }
        public long SaleStock { get; set; }
        public long? AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public ShowingWarehouse_AppUserDTO AppUser { get; set; }   
        public ShowingWarehouse_ShowingItemDTO ShowingItem { get; set; }   

        public ShowingWarehouse_ShowingInventoryDTO() {}
        public ShowingWarehouse_ShowingInventoryDTO(ShowingInventory ShowingInventory)
        {
            this.Id = ShowingInventory.Id;
            this.ShowingWarehouseId = ShowingInventory.ShowingWarehouseId;
            this.ShowingItemId = ShowingInventory.ShowingItemId;
            this.SaleStock = ShowingInventory.SaleStock;
            this.AccountingStock = ShowingInventory.AccountingStock;
            this.AppUserId = ShowingInventory.AppUserId;
            this.AppUser = ShowingInventory.AppUser == null ? null : new ShowingWarehouse_AppUserDTO(ShowingInventory.AppUser);
            this.ShowingItem = ShowingInventory.ShowingItem == null ? null : new ShowingWarehouse_ShowingItemDTO(ShowingInventory.ShowingItem);
            this.Errors = ShowingInventory.Errors;
        }
    }

    public class ShowingWarehouse_ShowingInventoryFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ShowingWarehouseId { get; set; }
        
        public IdFilter ShowingItemId { get; set; }
        
        public LongFilter SaleStock { get; set; }
        
        public LongFilter AccountingStock { get; set; }
        
        public IdFilter AppUserId { get; set; }
        
        public ShowingInventoryOrder OrderBy { get; set; }
    }
}