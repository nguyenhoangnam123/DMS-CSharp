using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_IndirectSalesOrderContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public decimal PrimaryPrice { get; set; }
        public decimal SalePrice { get; set; }
        public long EditedPriceStatusId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxAmount { get; set; }
        public long? Factor { get; set; }
        public MobileSync_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public MobileSync_ItemDTO Item { get; set; }
        public MobileSync_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public MobileSync_TaxTypeDTO TaxType { get; set; }
        public MobileSync_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public MobileSync_IndirectSalesOrderContentDTO() { }
        public MobileSync_IndirectSalesOrderContentDTO(IndirectSalesOrderContentDAO IndirectSalesOrderContentDAO)
        {
            this.Id = IndirectSalesOrderContentDAO.Id;
            this.IndirectSalesOrderId = IndirectSalesOrderContentDAO.IndirectSalesOrderId;
            this.ItemId = IndirectSalesOrderContentDAO.ItemId;
            this.UnitOfMeasureId = IndirectSalesOrderContentDAO.UnitOfMeasureId;
            this.Quantity = IndirectSalesOrderContentDAO.Quantity;
            this.PrimaryUnitOfMeasureId = IndirectSalesOrderContentDAO.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = IndirectSalesOrderContentDAO.RequestedQuantity;
            this.PrimaryPrice = IndirectSalesOrderContentDAO.PrimaryPrice;
            this.SalePrice = IndirectSalesOrderContentDAO.SalePrice;
            this.EditedPriceStatusId = IndirectSalesOrderContentDAO.EditedPriceStatusId;
            this.DiscountPercentage = IndirectSalesOrderContentDAO.DiscountPercentage;
            this.DiscountAmount = IndirectSalesOrderContentDAO.DiscountAmount;
            this.GeneralDiscountPercentage = IndirectSalesOrderContentDAO.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = IndirectSalesOrderContentDAO.GeneralDiscountAmount;
            this.Amount = IndirectSalesOrderContentDAO.Amount;
            this.TaxPercentage = IndirectSalesOrderContentDAO.TaxPercentage;
            this.TaxAmount = IndirectSalesOrderContentDAO.TaxAmount;
            this.Factor = IndirectSalesOrderContentDAO.Factor;
            this.EditedPriceStatus = IndirectSalesOrderContentDAO.EditedPriceStatus == null ? null : new MobileSync_EditedPriceStatusDTO(IndirectSalesOrderContentDAO.EditedPriceStatus);
            this.Item = IndirectSalesOrderContentDAO.Item == null ? null : new MobileSync_ItemDTO(IndirectSalesOrderContentDAO.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderContentDAO.PrimaryUnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(IndirectSalesOrderContentDAO.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderContentDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(IndirectSalesOrderContentDAO.UnitOfMeasure);
            
        }
    }
}