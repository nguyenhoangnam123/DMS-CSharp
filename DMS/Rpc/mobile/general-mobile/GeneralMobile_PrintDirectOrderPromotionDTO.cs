using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_PrintDirectOrderPromotionDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long Quantity { get; set; }
        public string QuantityString { get; set; }
        public long RequestedQuantity { get; set; }
        public string RequestedQuantityString { get; set; }
        public long? Factor { get; set; }
        public GeneralMobile_ItemDTO Item { get; set; }
        public GeneralMobile_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public GeneralMobile_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public GeneralMobile_PrintDirectOrderPromotionDTO() { }
        public GeneralMobile_PrintDirectOrderPromotionDTO(DirectSalesOrderPromotion DirectSalesOrderPromotion)
        {
            this.Id = DirectSalesOrderPromotion.Id;
            this.Quantity = DirectSalesOrderPromotion.Quantity;
            this.RequestedQuantity = DirectSalesOrderPromotion.RequestedQuantity;
            this.Factor = DirectSalesOrderPromotion.Factor;
            this.Item = DirectSalesOrderPromotion.Item == null ? null : new GeneralMobile_ItemDTO(DirectSalesOrderPromotion.Item);
            this.PrimaryUnitOfMeasure = DirectSalesOrderPromotion.PrimaryUnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(DirectSalesOrderPromotion.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = DirectSalesOrderPromotion.UnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(DirectSalesOrderPromotion.UnitOfMeasure);
            this.Errors = DirectSalesOrderPromotion.Errors;
        }
    }
}
