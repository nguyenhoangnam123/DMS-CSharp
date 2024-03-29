﻿using DMS.ABE.Common;
using DMS.ABE.Entities;

namespace DMS.ABE.Rpc.direct_sales_order
{
    public class DirectSalesOrder_PrintDirectOrderPromotionDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long Quantity { get; set; }
        public string QuantityString { get; set; }
        public long RequestedQuantity { get; set; }
        public string RequestedQuantityString { get; set; }
        public long? Factor { get; set; }
        public DirectSalesOrder_ItemDTO Item { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public DirectSalesOrder_PrintDirectOrderPromotionDTO() { }
        public DirectSalesOrder_PrintDirectOrderPromotionDTO(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            this.Id = DirectSalesOrderPromotion.Id;
            this.Quantity = DirectSalesOrderPromotion.Quantity;
            this.RequestedQuantity = DirectSalesOrderPromotion.RequestedQuantity;
            this.Factor = DirectSalesOrderPromotion.Factor;
            this.Item = DirectSalesOrderPromotion.Item == null ? null : new DirectSalesOrder_ItemDTO(DirectSalesOrderPromotion.Item);
            this.PrimaryUnitOfMeasure = DirectSalesOrderPromotion.PrimaryUnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderPromotion.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = DirectSalesOrderPromotion.UnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderPromotion.UnitOfMeasure);
            this.Errors = DirectSalesOrderPromotion.Errors;
        }
    }
}
