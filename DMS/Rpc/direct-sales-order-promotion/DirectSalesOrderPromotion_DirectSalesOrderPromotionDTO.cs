using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.direct_sales_order_promotion
{
    public class DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO : DataDTO
    {
        public long Id { get; set; }
        public long DirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public string Note { get; set; }
        public DirectSalesOrderPromotion_DirectSalesOrderDTO DirectSalesOrder { get; set; }
        public DirectSalesOrderPromotion_ItemDTO Item { get; set; }
        public DirectSalesOrderPromotion_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public DirectSalesOrderPromotion_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO() {}
        public DirectSalesOrderPromotion_DirectSalesOrderPromotionDTO(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            this.Id = DirectSalesOrderPromotion.Id;
            this.DirectSalesOrderId = DirectSalesOrderPromotion.DirectSalesOrderId;
            this.ItemId = DirectSalesOrderPromotion.ItemId;
            this.UnitOfMeasureId = DirectSalesOrderPromotion.UnitOfMeasureId;
            this.Quantity = DirectSalesOrderPromotion.Quantity;
            this.PrimaryUnitOfMeasureId = DirectSalesOrderPromotion.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = DirectSalesOrderPromotion.RequestedQuantity;
            this.Note = DirectSalesOrderPromotion.Note;
            this.DirectSalesOrder = DirectSalesOrderPromotion.DirectSalesOrder == null ? null : new DirectSalesOrderPromotion_DirectSalesOrderDTO(DirectSalesOrderPromotion.DirectSalesOrder);
            this.Item = DirectSalesOrderPromotion.Item == null ? null : new DirectSalesOrderPromotion_ItemDTO(DirectSalesOrderPromotion.Item);
            this.PrimaryUnitOfMeasure = DirectSalesOrderPromotion.PrimaryUnitOfMeasure == null ? null : new DirectSalesOrderPromotion_UnitOfMeasureDTO(DirectSalesOrderPromotion.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = DirectSalesOrderPromotion.UnitOfMeasure == null ? null : new DirectSalesOrderPromotion_UnitOfMeasureDTO(DirectSalesOrderPromotion.UnitOfMeasure);
            this.Errors = DirectSalesOrderPromotion.Errors;
        }
    }

    public class DirectSalesOrderPromotion_DirectSalesOrderPromotionFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter DirectSalesOrderId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public LongFilter Quantity { get; set; }
        public IdFilter PrimaryUnitOfMeasureId { get; set; }
        public LongFilter RequestedQuantity { get; set; }
        public StringFilter Note { get; set; }
        public DirectSalesOrderPromotionOrder OrderBy { get; set; }
    }
}
