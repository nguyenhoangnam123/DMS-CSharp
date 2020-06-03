using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_KpiItemContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long KpiItemId { get; set; }
        public long ItemId { get; set; }
        public KpiItem_ItemDTO Item { get; set; }   
        public List<KpiItem_KpiItemContentKpiCriteriaItemMappingDTO> KpiItemContentKpiCriteriaItemMappings { get; set; }


        public KpiItem_KpiItemContentDTO() {}
        public KpiItem_KpiItemContentDTO(KpiItemContent KpiItemContent)
        {
            this.Id = KpiItemContent.Id;
            this.KpiItemId = KpiItemContent.KpiItemId;
            this.ItemId = KpiItemContent.ItemId;
            this.Item = KpiItemContent.Item == null ? null : new KpiItem_ItemDTO(KpiItemContent.Item);
            this.KpiItemContentKpiCriteriaItemMappings = KpiItemContent.KpiItemContentKpiCriteriaItemMappings?.Select(x => new KpiItem_KpiItemContentKpiCriteriaItemMappingDTO(x)).ToList();
            this.Errors = KpiItemContent.Errors;
        }
    }
}