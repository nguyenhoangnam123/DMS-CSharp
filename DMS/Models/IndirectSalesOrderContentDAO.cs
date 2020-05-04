using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectSalesOrderContentDAO
    {
        public long Id { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public long SalePrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public long? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long Amount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public long? TaxAmount { get; set; }

        public virtual IndirectSalesOrderDAO IndirectSalesOrder { get; set; }
        public virtual ItemDAO IndirectSalesOrderNavigation { get; set; }
        public virtual UnitOfMeasureDAO PrimaryUnitOfMeasure { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
