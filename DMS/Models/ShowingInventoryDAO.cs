using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ShowingInventoryDAO
    {
        public ShowingInventoryDAO()
        {
            ShowingInventoryHistories = new HashSet<ShowingInventoryHistoryDAO>();
        }

        public long Id { get; set; }
        public long ShowingWarehouseId { get; set; }
        public long ShowingItemId { get; set; }
        public long SaleStock { get; set; }
        public long? AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual ShowingItemDAO ShowingItem { get; set; }
        public virtual ShowingWarehouseDAO ShowingWarehouse { get; set; }
        public virtual ICollection<ShowingInventoryHistoryDAO> ShowingInventoryHistories { get; set; }
    }
}
