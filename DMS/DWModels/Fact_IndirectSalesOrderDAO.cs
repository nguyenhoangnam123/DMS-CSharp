using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_IndirectSalesOrderDAO
    {
        public long Id { get; set; }
        public long IndirectSalesOrderTransactionId { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long OrganizationId { get; set; }
        public long BuyerStoreId { get; set; }
        public long SalesEmployeeId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Revenue { get; set; }
        public long TypeId { get; set; }
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        /// <summary>
        /// Địa chỉ giao hàng
        /// </summary>
        public string DeliveryAddress { get; set; }
        /// <summary>
        /// Cửa hàng bán
        /// </summary>
        public long SellerStoreId { get; set; }
        /// <summary>
        /// Ngày giao hàng
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        /// <summary>
        /// Sửa giá
        /// </summary>
        public long EditedPriceStatusId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        public long? StoreCheckingId { get; set; }
        public long RequestStateId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
