using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class IndirectSalesOrderPromotionDAO
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Id đơn hàng gián tiếp
        /// </summary>
        public long IndirectSalesOrderId { get; set; }
        /// <summary>
        /// Sản phẩm khuyến mãi
        /// </summary>
        public long ItemId { get; set; }
        /// <summary>
        /// Đơn vị xuất kho
        /// </summary>
        public long UnitOfMeasureId { get; set; }
        /// <summary>
        /// Số lượng bán
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// Đơn vị lưu kho
        /// </summary>
        public long PrimaryUnitOfMeasureId { get; set; }
        /// <summary>
        /// Số lượng yêu cầu xuất kho
        /// </summary>
        public long RequestedQuantity { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public long? Factor { get; set; }

        public virtual IndirectSalesOrderDAO IndirectSalesOrder { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual UnitOfMeasureDAO PrimaryUnitOfMeasure { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
