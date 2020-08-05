﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectSalesOrderTransactionDAO
    {
        public long Id { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long OrganizationId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Revenue { get; set; }
        public long TypeId { get; set; }

        public virtual IndirectSalesOrderDAO IndirectSalesOrder { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
