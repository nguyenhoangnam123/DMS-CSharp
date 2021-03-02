using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_IndirectSalesOrderPromotionDTO : DataDTO
    {
        public long Id { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public string Note { get; set; }
        public long? Factor { get; set; }
        public GeneralMobile_ItemDTO Item { get; set; }
        public GeneralMobile_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public GeneralMobile_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public GeneralMobile_IndirectSalesOrderPromotionDTO() { }
        public GeneralMobile_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            this.Id = IndirectSalesOrderPromotion.Id;
            this.IndirectSalesOrderId = IndirectSalesOrderPromotion.IndirectSalesOrderId;
            this.ItemId = IndirectSalesOrderPromotion.ItemId;
            this.UnitOfMeasureId = IndirectSalesOrderPromotion.UnitOfMeasureId;
            this.Quantity = IndirectSalesOrderPromotion.Quantity;
            this.PrimaryUnitOfMeasureId = IndirectSalesOrderPromotion.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = IndirectSalesOrderPromotion.RequestedQuantity;
            this.Note = IndirectSalesOrderPromotion.Note;
            this.Factor = IndirectSalesOrderPromotion.Factor;
            this.Item = IndirectSalesOrderPromotion.Item == null ? null : new GeneralMobile_ItemDTO(IndirectSalesOrderPromotion.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderPromotion.PrimaryUnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(IndirectSalesOrderPromotion.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderPromotion.UnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(IndirectSalesOrderPromotion.UnitOfMeasure);
            this.Errors = IndirectSalesOrderPromotion.Errors;
        }
    }

    public class GeneralMobile_IndirectSalesOrderPromotionFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter IndirectSalesOrderId { get; set; }

        public IdFilter ItemId { get; set; }

        public IdFilter UnitOfMeasureId { get; set; }

        public LongFilter Quantity { get; set; }

        public IdFilter PrimaryUnitOfMeasureId { get; set; }

        public LongFilter RequestedQuantity { get; set; }

        public StringFilter Note { get; set; }

        public IndirectSalesOrderPromotionOrder OrderBy { get; set; }
    }
}
