using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long StatusId { get; set; }
        public UnitOfMeasureGrouping_StatusDTO Status { get; set; }
        public UnitOfMeasureGrouping_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public List<UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO> UnitOfMeasureGroupingContents { get; set; }
        public UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO() {}
        public UnitOfMeasureGrouping_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            this.Id = UnitOfMeasureGrouping.Id;
            this.Name = UnitOfMeasureGrouping.Name;
            this.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
            this.StatusId = UnitOfMeasureGrouping.StatusId;
            this.Status = UnitOfMeasureGrouping.Status == null ? null : new UnitOfMeasureGrouping_StatusDTO(UnitOfMeasureGrouping.Status);
            this.UnitOfMeasure = UnitOfMeasureGrouping.UnitOfMeasure == null ? null : new UnitOfMeasureGrouping_UnitOfMeasureDTO(UnitOfMeasureGrouping.UnitOfMeasure);
            this.UnitOfMeasureGroupingContents = UnitOfMeasureGrouping.UnitOfMeasureGroupingContents?.Select(x => new UnitOfMeasureGrouping_UnitOfMeasureGroupingContentDTO(x)).ToList();
        }
    }

    public class UnitOfMeasureGrouping_UnitOfMeasureGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter StatusId { get; set; }
        public UnitOfMeasureGroupingOrder OrderBy { get; set; }
    }
}
