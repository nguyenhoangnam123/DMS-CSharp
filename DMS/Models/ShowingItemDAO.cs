﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ShowingItemDAO
    {
        public ShowingItemDAO()
        {
            ShowingInventories = new HashSet<ShowingInventoryDAO>();
            ShowingOrderContents = new HashSet<ShowingOrderContentDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long CategoryId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal SalePrice { get; set; }
        public string Desception { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual CategoryDAO Category { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
        public virtual ICollection<ShowingInventoryDAO> ShowingInventories { get; set; }
        public virtual ICollection<ShowingOrderContentDAO> ShowingOrderContents { get; set; }
    }
}
