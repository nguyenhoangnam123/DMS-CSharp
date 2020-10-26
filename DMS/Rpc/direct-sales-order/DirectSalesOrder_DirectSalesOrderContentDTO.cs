using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_DirectSalesOrderContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long DirectSalesOrderId { get; set; }
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
        public DirectSalesOrder_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public DirectSalesOrder_ItemDTO Item { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public DirectSalesOrder_TaxTypeDTO TaxType { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public DirectSalesOrder_DirectSalesOrderContentDTO() { }
        public DirectSalesOrder_DirectSalesOrderContentDTO(DirectSalesOrderContent DirectSalesOrderContent)
        {
            this.Id = DirectSalesOrderContent.Id;
            this.DirectSalesOrderId = DirectSalesOrderContent.DirectSalesOrderId;
            this.ItemId = DirectSalesOrderContent.ItemId;
            this.UnitOfMeasureId = DirectSalesOrderContent.UnitOfMeasureId;
            this.Quantity = DirectSalesOrderContent.Quantity;
            this.PrimaryUnitOfMeasureId = DirectSalesOrderContent.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = DirectSalesOrderContent.RequestedQuantity;
            this.PrimaryPrice = DirectSalesOrderContent.PrimaryPrice;
            this.SalePrice = DirectSalesOrderContent.SalePrice;
            this.EditedPriceStatusId = DirectSalesOrderContent.EditedPriceStatusId;
            this.DiscountPercentage = DirectSalesOrderContent.DiscountPercentage;
            this.DiscountAmount = DirectSalesOrderContent.DiscountAmount;
            this.GeneralDiscountPercentage = DirectSalesOrderContent.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = DirectSalesOrderContent.GeneralDiscountAmount;
            this.Amount = DirectSalesOrderContent.Amount;
            this.TaxPercentage = DirectSalesOrderContent.TaxPercentage;
            this.TaxAmount = DirectSalesOrderContent.TaxAmount;
            this.Factor = DirectSalesOrderContent.Factor;
            this.EditedPriceStatus = DirectSalesOrderContent.EditedPriceStatus == null ? null : new DirectSalesOrder_EditedPriceStatusDTO(DirectSalesOrderContent.EditedPriceStatus);
            this.Item = DirectSalesOrderContent.Item == null ? null : new DirectSalesOrder_ItemDTO(DirectSalesOrderContent.Item);
            this.PrimaryUnitOfMeasure = DirectSalesOrderContent.PrimaryUnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderContent.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = DirectSalesOrderContent.UnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderContent.UnitOfMeasure);
            this.Errors = DirectSalesOrderContent.Errors;
        }
    }

    public class DirectSalesOrder_DirectSalesOrderContentFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter DirectSalesOrderId { get; set; }

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

        public DirectSalesOrderContentOrder OrderBy { get; set; }
    }
}