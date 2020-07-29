using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_InventoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long SaleStock { get; set; }
        public long AccountingStock { get; set; }
        public Mobile_WarehouseDTO Warehouse { get; set; }

        public Mobile_InventoryDTO() { }
        public Mobile_InventoryDTO(Inventory Inventory)
        {
            this.Id = Inventory.Id;
            this.WarehouseId = Inventory.WarehouseId;
            this.ItemId = Inventory.ItemId;
            this.SaleStock = Inventory.SaleStock;
            this.AccountingStock = Inventory.AccountingStock;
            this.Warehouse = Inventory.Warehouse == null ? null : new Mobile_WarehouseDTO(Inventory.Warehouse);
            this.Errors = Inventory.Errors;
        }
    }

    public class Mobile_InventoryFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter WarehouseId { get; set; }

        public IdFilter ItemId { get; set; }

        public LongFilter SaleStock { get; set; }

        public LongFilter AccountingStock { get; set; }

        public InventoryOrder OrderBy { get; set; }
    }
}