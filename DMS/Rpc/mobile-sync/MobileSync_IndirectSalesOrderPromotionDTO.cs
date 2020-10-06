using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_IndirectSalesOrderPromotionDTO : DataDTO
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
        public MobileSync_ItemDTO Item { get; set; }
        public MobileSync_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public MobileSync_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public MobileSync_IndirectSalesOrderPromotionDTO() { }
        public MobileSync_IndirectSalesOrderPromotionDTO(IndirectSalesOrderPromotionDAO IndirectSalesOrderPromotionDAO)
        {
            this.Id = IndirectSalesOrderPromotionDAO.Id;
            this.IndirectSalesOrderId = IndirectSalesOrderPromotionDAO.IndirectSalesOrderId;
            this.ItemId = IndirectSalesOrderPromotionDAO.ItemId;
            this.UnitOfMeasureId = IndirectSalesOrderPromotionDAO.UnitOfMeasureId;
            this.Quantity = IndirectSalesOrderPromotionDAO.Quantity;
            this.PrimaryUnitOfMeasureId = IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasureId;
            this.RequestedQuantity = IndirectSalesOrderPromotionDAO.RequestedQuantity;
            this.Note = IndirectSalesOrderPromotionDAO.Note;
            this.Factor = IndirectSalesOrderPromotionDAO.Factor;
            this.Item = IndirectSalesOrderPromotionDAO.Item == null ? null : new MobileSync_ItemDTO(IndirectSalesOrderPromotionDAO.Item);
            this.PrimaryUnitOfMeasure = IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(IndirectSalesOrderPromotionDAO.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = IndirectSalesOrderPromotionDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(IndirectSalesOrderPromotionDAO.UnitOfMeasure);
        }
    }

    public class MobileSync_IndirectSalesOrderPromotionFilterDTO : FilterDTO
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