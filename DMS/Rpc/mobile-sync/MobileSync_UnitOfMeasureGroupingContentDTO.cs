using Common;
using DMS.Entities;
using DMS.Models;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_UnitOfMeasureGroupingContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long UnitOfMeasureGroupingId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? Factor { get; set; }
        public MobileSync_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public MobileSync_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping { get; set; }
        public MobileSync_UnitOfMeasureGroupingContentDTO() { }
        public MobileSync_UnitOfMeasureGroupingContentDTO(UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO)
        {
            this.Id = UnitOfMeasureGroupingContentDAO.Id;
            this.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId;
            this.UnitOfMeasureId = UnitOfMeasureGroupingContentDAO.UnitOfMeasureId;
            this.Factor = UnitOfMeasureGroupingContentDAO.Factor;
            this.UnitOfMeasure = UnitOfMeasureGroupingContentDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(UnitOfMeasureGroupingContentDAO.UnitOfMeasure);
            this.UnitOfMeasureGrouping = UnitOfMeasureGroupingContentDAO.UnitOfMeasureGrouping == null ? null : new MobileSync_UnitOfMeasureGroupingDTO(UnitOfMeasureGroupingContentDAO.UnitOfMeasureGrouping);
        }
    }
}
