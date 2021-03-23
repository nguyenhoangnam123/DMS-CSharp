using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_PrintDirectOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public string sOrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string sDeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public string SubTotalString { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public string Discount { get; set; }
        public decimal Total { get; set; }
        public string TotalString { get; set; }
        public string TotalText { get; set; }
        public string Note { get; set; }
        public GeneralMobile_StoreDTO BuyerStore { get; set; }
        public GeneralMobile_AppUserDTO SaleEmployee { get; set; }
        public GeneralMobile_StoreDTO SellerStore { get; set; }
        public List<GeneralMobile_PrintDirectOrderContentDTO> Contents { get; set; }
        public List<GeneralMobile_PrintDirectOrderPromotionDTO> Promotions { get; set; }
        public GeneralMobile_PrintDirectOrderDTO() { }
        public GeneralMobile_PrintDirectOrderDTO(DirectSalesOrder DirectSalesOrder)
        {
            this.Id = DirectSalesOrder.Id;
            this.Code = DirectSalesOrder.Code;
            this.PhoneNumber = DirectSalesOrder.PhoneNumber;
            this.StoreAddress = DirectSalesOrder.StoreAddress;
            this.DeliveryAddress = DirectSalesOrder.DeliveryAddress;
            this.OrderDate = DirectSalesOrder.OrderDate;
            this.DeliveryDate = DirectSalesOrder.DeliveryDate;
            this.SubTotal = DirectSalesOrder.SubTotal;
            this.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            this.Total = DirectSalesOrder.Total;
            this.Note = DirectSalesOrder.Note;
            this.BuyerStore = DirectSalesOrder.BuyerStore == null ? null : new GeneralMobile_StoreDTO(DirectSalesOrder.BuyerStore);
            this.SaleEmployee = DirectSalesOrder.SaleEmployee == null ? null : new GeneralMobile_AppUserDTO(DirectSalesOrder.SaleEmployee);
            this.Contents = DirectSalesOrder.DirectSalesOrderContents?.Select(x => new GeneralMobile_PrintDirectOrderContentDTO(x)).ToList();
            this.Promotions = DirectSalesOrder.DirectSalesOrderPromotions?.Select(x => new GeneralMobile_PrintDirectOrderPromotionDTO(x)).ToList();
            this.Errors = DirectSalesOrder.Errors;
        }
    }
}
