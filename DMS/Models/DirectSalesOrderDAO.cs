using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectSalesOrderDAO
    {
        public DirectSalesOrderDAO()
        {
            DirectSalesOrderContents = new HashSet<DirectSalesOrderContentDAO>();
            DirectSalesOrderPromotions = new HashSet<DirectSalesOrderPromotionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
        public string StoreDeliveryAddress { get; set; }
        public string TaxCode { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal Total { get; set; }
        public long RequestStateId { get; set; }

        public virtual StoreDAO BuyerStore { get; set; }
        public virtual EditedPriceStatusDAO EditedPriceStatus { get; set; }
        public virtual RequestStateDAO RequestState { get; set; }
        public virtual AppUserDAO SaleEmployee { get; set; }
        public virtual ICollection<DirectSalesOrderContentDAO> DirectSalesOrderContents { get; set; }
        public virtual ICollection<DirectSalesOrderPromotionDAO> DirectSalesOrderPromotions { get; set; }
    }
}
