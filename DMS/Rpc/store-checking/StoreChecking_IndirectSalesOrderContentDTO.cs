using Common;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_IndirectSalesOrderContentDTO : DataDTO
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
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxAmount { get; set; }
        public long? Factor { get; set; }
        public StoreChecking_ItemDTO Item { get; set; }
        public StoreChecking_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public StoreChecking_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public StoreChecking_IndirectSalesOrderContentDTO() { }
        public StoreChecking_IndirectSalesOrderContentDTO(IndirectSalesOrderContent IndirectSalesOrderContent)
        {
            this.Id = IndirectSalesOrderContent.Id;
            this.IndirectSalesOrderId = IndirectSalesOrderContent.IndirectSalesOrderId;
            this.ItemId = IndirectSalesOrderContent.ItemId;
            this.UnitOfMeasureId = IndirectSalesOrderContent.UnitOfMeasureId;
            this.Quantity = IndirectSalesOrderContent.Quantity;
            this.PrimaryUnitOfMeasureId = IndirectSalesOrderContent.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = IndirectSalesOrderContent.RequestedQuantity;
            this.PrimaryPrice = IndirectSalesOrderContent.PrimaryPrice;
            this.SalePrice = IndirectSalesOrderContent.SalePrice;
            this.DiscountPercentage = IndirectSalesOrderContent.DiscountPercentage;
            this.DiscountAmount = IndirectSalesOrderContent.DiscountAmount;
            this.GeneralDiscountPercentage = IndirectSalesOrderContent.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = IndirectSalesOrderContent.GeneralDiscountAmount;
            this.Amount = IndirectSalesOrderContent.Amount;
            this.TaxPercentage = IndirectSalesOrderContent.TaxPercentage;
            this.TaxAmount = IndirectSalesOrderContent.TaxAmount;
            this.Factor = IndirectSalesOrderContent.Factor;
            this.Item = IndirectSalesOrderContent.Item == null ? null : new StoreChecking_ItemDTO(IndirectSalesOrderContent.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderContent.PrimaryUnitOfMeasure == null ? null : new StoreChecking_UnitOfMeasureDTO(IndirectSalesOrderContent.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderContent.UnitOfMeasure == null ? null : new StoreChecking_UnitOfMeasureDTO(IndirectSalesOrderContent.UnitOfMeasure);
            this.Errors = IndirectSalesOrderContent.Errors;
        }
    }

    public class StoreChecking_IndirectSalesOrderContentFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter IndirectSalesOrderId { get; set; }

        public IdFilter ItemId { get; set; }

        public IdFilter UnitOfMeasureId { get; set; }

        public LongFilter Quantity { get; set; }

        public IdFilter PrimaryUnitOfMeasureId { get; set; }

        public LongFilter RequestedQuantity { get; set; }
        public LongFilter PrimaryPrice { get; set; }
        public LongFilter SalePrice { get; set; }

        public DecimalFilter DiscountPercentage { get; set; }

        public LongFilter DiscountAmount { get; set; }

        public DecimalFilter GeneralDiscountPercentage { get; set; }

        public LongFilter GeneralDiscountAmount { get; set; }

        public LongFilter Amount { get; set; }

        public DecimalFilter TaxPercentage { get; set; }

        public LongFilter TaxAmount { get; set; }

        public IndirectSalesOrderContentOrder OrderBy { get; set; }
    }
}
