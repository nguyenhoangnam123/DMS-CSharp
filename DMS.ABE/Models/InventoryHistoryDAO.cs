using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class InventoryHistoryDAO
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public long OldSaleStock { get; set; }
        public long SaleStock { get; set; }
        public long OldAccountingStock { get; set; }
        public long AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual InventoryDAO Inventory { get; set; }
    }
}
