using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemDAO
    {
        public ItemDAO()
        {
            Inventories = new HashSet<InventoryDAO>();
        }

        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ProductDAO Product { get; set; }
        public virtual ICollection<InventoryDAO> Inventories { get; set; }
    }
}
