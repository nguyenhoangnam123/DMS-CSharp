using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobile_PrintIndirectOrderPromotionDTO : DataDTO
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

        public GeneralMobile_PrintIndirectOrderPromotionDTO() { }
        public GeneralMobile_PrintIndirectOrderPromotionDTO(IndirectSalesOrderPromotion IndirectSalesOrderPromotion)
        {
            this.Id = IndirectSalesOrderPromotion.Id;
            this.Quantity = IndirectSalesOrderPromotion.Quantity;
            this.RequestedQuantity = IndirectSalesOrderPromotion.RequestedQuantity;
            this.Factor = IndirectSalesOrderPromotion.Factor;
            this.Item = IndirectSalesOrderPromotion.Item == null ? null : new GeneralMobile_ItemDTO(IndirectSalesOrderPromotion.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderPromotion.PrimaryUnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(IndirectSalesOrderPromotion.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderPromotion.UnitOfMeasure == null ? null : new GeneralMobile_UnitOfMeasureDTO(IndirectSalesOrderPromotion.UnitOfMeasure);
            this.Errors = IndirectSalesOrderPromotion.Errors;
        }
    }
}
