using Common;
using DMS.Entities;
using DMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_UnitOfMeasureGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long UnitOfMeasureId { get; set; }
        public MobileSync_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public List<MobileSync_UnitOfMeasureGroupingContentDTO> UnitOfMeasureGroupingContents { get; set; }
        public MobileSync_UnitOfMeasureGroupingDTO() { }
        public MobileSync_UnitOfMeasureGroupingDTO(UnitOfMeasureGroupingDAO UnitOfMeasureGroupingDAO)
        {
            this.Id = UnitOfMeasureGroupingDAO.Id;
            this.Code = UnitOfMeasureGroupingDAO.Code;
            this.Name = UnitOfMeasureGroupingDAO.Name;
            this.Description = UnitOfMeasureGroupingDAO.Description;
            this.UnitOfMeasureId = UnitOfMeasureGroupingDAO.UnitOfMeasureId;
            this.UnitOfMeasure = UnitOfMeasureGroupingDAO.UnitOfMeasure == null ? null : new MobileSync_UnitOfMeasureDTO(UnitOfMeasureGroupingDAO.UnitOfMeasure);
            this.UnitOfMeasureGroupingContents = UnitOfMeasureGroupingDAO.UnitOfMeasureGroupingContents?.Select(x => new MobileSync_UnitOfMeasureGroupingContentDTO(x)).ToList();
        }
    }
}
