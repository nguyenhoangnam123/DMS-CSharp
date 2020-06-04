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
        public Dictionary<long,long> KpiItemContentKpiCriteriaItemMappings { get; set; }


        public KpiItem_KpiItemContentDTO() {}
        public KpiItem_KpiItemContentDTO(KpiItemContent KpiItemContent)
        {
            this.Id = KpiItemContent.Id;
            this.KpiItemId = KpiItemContent.KpiItemId;
            this.ItemId = KpiItemContent.ItemId;
            this.Item = KpiItemContent.Item == null ? null : new KpiItem_ItemDTO(KpiItemContent.Item);
            this.KpiItemContentKpiCriteriaItemMappings = KpiItemContent.KpiItemContentKpiCriteriaItemMappings?.ToDictionary(x => x.KpiCriteriaItemId, y => y.Value);
            this.Errors = KpiItemContent.Errors;
        }
    }
}