using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_PrintDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public decimal SubTotal { get; set; }
        public string SubTotalString { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public string Discount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public string Tax { get; set; }
        public decimal Total { get; set; }
        public string TotalString { get; set; }
        public string TotalText { get; set; }
        public IndirectSalesOrder_StoreDTO BuyerStore { get; set; }
        public IndirectSalesOrder_AppUserDTO SaleEmployee { get; set; }
        public IndirectSalesOrder_StoreDTO SellerStore { get; set; }
        public List<IndirectSalesOrder_PrintContentDTO> Contents { get; set; }
        public List<IndirectSalesOrder_PrintPromotionDTO> Promotions { get; set; }
        public IndirectSalesOrder_PrintDTO() { }
        public IndirectSalesOrder_PrintDTO(IndirectSalesOrder IndirectSalesOrder)
        {
            this.Id = IndirectSalesOrder.Id;
            this.Code = IndirectSalesOrder.Code;
            this.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            this.StoreAddress = IndirectSalesOrder.StoreAddress;
            this.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            this.OrderDate = IndirectSalesOrder.OrderDate;
            this.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            this.SubTotal = IndirectSalesOrder.SubTotal;
            this.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            this.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
            this.Total = IndirectSalesOrder.Total;
            this.BuyerStore = IndirectSalesOrder.BuyerStore == null ? null : new IndirectSalesOrder_StoreDTO(IndirectSalesOrder.BuyerStore);
            this.SaleEmployee = IndirectSalesOrder.SaleEmployee == null ? null : new IndirectSalesOrder_AppUserDTO(IndirectSalesOrder.SaleEmployee);
            this.SellerStore = IndirectSalesOrder.SellerStore == null ? null : new IndirectSalesOrder_StoreDTO(IndirectSalesOrder.SellerStore);
            this.Contents = IndirectSalesOrder.IndirectSalesOrderContents?.Select(x => new IndirectSalesOrder_PrintContentDTO(x)).ToList();
            this.Promotions = IndirectSalesOrder.IndirectSalesOrderPromotions?.Select(x => new IndirectSalesOrder_PrintPromotionDTO(x)).ToList();
            this.Errors = IndirectSalesOrder.Errors;
        }
    }
}
