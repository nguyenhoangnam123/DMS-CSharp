using Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.RpcPublic
{
    public class Public_InventoryDTO : DataDTO
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long SaleStock { get; set; }
        public long AccountingStock { get; set; }

        public Public_InventoryDTO() { }
        public Public_InventoryDTO(Inventory Inventory)
        {
            this.Id = Inventory.Id;
            this.WarehouseId = Inventory.WarehouseId;
            this.ItemId = Inventory.ItemId;
            this.SaleStock = Inventory.SaleStock;
            this.AccountingStock = Inventory.AccountingStock;
            this.Errors = Inventory.Errors;
        }
    }

    public class Public_InventoryFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter WarehouseId { get; set; }

        public IdFilter ItemId { get; set; }

        public LongFilter SaleStock { get; set; }

        public LongFilter AccountingStock { get; set; }

        public InventoryOrder OrderBy { get; set; }
    }
}