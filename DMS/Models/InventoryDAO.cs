﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class InventoryDAO
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long ProductId { get; set; }
        public long SaleStock { get; set; }
        public long AccountingStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ProductDAO Product { get; set; }
        public virtual WarehouseDAO Warehouse { get; set; }
    }
}
