using Common;
using DMS.Entities;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_IndirectSalesOrderContentDTO : DataDTO
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
        public IndirectSalesOrder_ItemDTO Item { get; set; }
        public IndirectSalesOrder_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public IndirectSalesOrder_TaxTypeDTO TaxType { get; set; }
        public IndirectSalesOrder_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public IndirectSalesOrder_IndirectSalesOrderContentDTO() { }
        public IndirectSalesOrder_IndirectSalesOrderContentDTO(IndirectSalesOrderContent IndirectSalesOrderContent)
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
            this.Item = IndirectSalesOrderContent.Item == null ? null : new IndirectSalesOrder_ItemDTO(IndirectSalesOrderContent.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderContent.PrimaryUnitOfMeasure == null ? null : new IndirectSalesOrder_UnitOfMeasureDTO(IndirectSalesOrderContent.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderContent.UnitOfMeasure == null ? null : new IndirectSalesOrder_UnitOfMeasureDTO(IndirectSalesOrderContent.UnitOfMeasure);
            this.Errors = IndirectSalesOrderContent.Errors;
        }
    }

    public class IndirectSalesOrder_IndirectSalesOrderContentFilterDTO : FilterDTO
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