using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectSalesOrderDAO
    {
        public IndirectSalesOrderDAO()
        {
            IndirectSalesOrderContents = new HashSet<IndirectSalesOrderContentDAO>();
            IndirectSalesOrderPromotions = new HashSet<IndirectSalesOrderPromotionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SellerStoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long RequestStateId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public long SubTotal { get; set; }
        public long? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long TotalTaxAmount { get; set; }
        public long Total { get; set; }

        public virtual StoreDAO BuyerStore { get; set; }
        public virtual EditedPriceStatusDAO EditedPriceStatus { get; set; }
        public virtual RequestStateDAO RequestState { get; set; }
        public virtual AppUserDAO SaleEmployee { get; set; }
        public virtual StoreDAO SellerStore { get; set; }
        public virtual ICollection<IndirectSalesOrderContentDAO> IndirectSalesOrderContents { get; set; }
        public virtual ICollection<IndirectSalesOrderPromotionDAO> IndirectSalesOrderPromotions { get; set; }
    }
}
