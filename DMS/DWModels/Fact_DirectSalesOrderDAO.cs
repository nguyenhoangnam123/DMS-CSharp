using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_DirectSalesOrderDAO
    {
        public long DirectSaleOrderTransactionId { get; set; }
        public long DirectSalesOrderId { get; set; }
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
        /// Nhân viên kinh doanh
        /// </summary>
        public long SaleEmployeeId { get; set; }
        /// <summary>
        /// Ngày giao hàng
        /// </summary>
        public DateTime? DeliveryDate { get; set; }
        public long? ErpApprovalStateId { get; set; }
        public long? StoreApprovalStateId { get; set; }
        public long RequestStateId { get; set; }
        public long? DirectSalesOrderSourceTypeId { get; set; }
        /// <summary>
        /// Sửa giá
        /// </summary>
        public long EditedPriceStatusId { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Tổng tiền trước thuế
        /// </summary>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Số tiền chiết khấu tổng
        /// </summary>
        public decimal? GeneralDiscount { get; set; }
        /// <summary>
        /// Tổng thuế
        /// </summary>
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAfterTax { get; set; }
        public string PromotionCode { get; set; }
        public decimal? PromotionValue { get; set; }
        /// <summary>
        /// Tổng tiền sau thuế
        /// </summary>
        public decimal Total { get; set; }
        public long? StoreCheckingId { get; set; }
        public long? StoreUserCreatorId { get; set; }
        public long? CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
