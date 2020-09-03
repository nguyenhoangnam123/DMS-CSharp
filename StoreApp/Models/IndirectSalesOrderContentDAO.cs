using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class IndirectSalesOrderContentDAO
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// FK của đơn hàng gián tiếp
        /// </summary>
        public long IndirectSalesOrderId { get; set; }
        /// <summary>
        /// Sản phẩm
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// Đơn vị để xuất hàng
        /// </summary>
        public long UnitOfMeasureId { get; set; }
        /// <summary>
        /// Số lượng xuất hàng
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// Đơn vị lưu kho
        /// </summary>
        public long PrimaryUnitOfMeasureId { get; set; }
        /// <summary>
        /// Số lượng xuất hàng theo đơn vị lưu kho
        /// </summary>
        public long RequestedQuantity { get; set; }
        /// <summary>
        /// Giá theo đơn vị lưu kho
        /// </summary>
        public decimal PrimaryPrice { get; set; }
        /// <summary>
        /// Giá bán theo đơn vị xuất hàng
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// % chiết khấu theo dòng
        /// </summary>
        public decimal? DiscountPercentage { get; set; }
        /// <summary>
        /// Số tiền chiết khấu theo dòng
        /// </summary>
        public decimal? DiscountAmount { get; set; }
        /// <summary>
        /// % chiết khấu tổng 
        /// </summary>
        public decimal? GeneralDiscountPercentage { get; set; }
        /// <summary>
        /// Số tiền sau chiết khấu từng dòng
        /// </summary>
        public decimal? GeneralDiscountAmount { get; set; }
        /// <summary>
        /// % thuế lấy theo sản phẩm
        /// </summary>
        public decimal? TaxPercentage { get; set; }
        /// <summary>
        /// Số tiền thuế sau tất cả các loại chiết khấu
        /// </summary>
        public decimal? TaxAmount { get; set; }
        /// <summary>
        /// Tổng số tiền từng dòng trước chiết khấu tổng
        /// </summary>
        public decimal Amount { get; set; }
        public long? Factor { get; set; }

        public virtual IndirectSalesOrderDAO IndirectSalesOrder { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual UnitOfMeasureDAO PrimaryUnitOfMeasure { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
