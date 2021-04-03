using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class DirectSalesOrderContentDAO
    {
        public long Id { get; set; }
        public long DirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        /// <summary>
        /// Giá theo đơn vị lưu kho
        /// </summary>
        public decimal PrimaryPrice { get; set; }
        /// <summary>
        /// Giá bán theo đơn vị xuất hàng
        /// </summary>
        public decimal SalePrice { get; set; }
        public long EditedPriceStatusId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal Amount { get; set; }
        public long? Factor { get; set; }

        public virtual DirectSalesOrderDAO DirectSalesOrder { get; set; }
        public virtual EditedPriceStatusDAO EditedPriceStatus { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual UnitOfMeasureDAO PrimaryUnitOfMeasure { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
