using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiItemContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long KpiItemId { get; set; }
        public long ItemId { get; set; }
        public Mobile_ItemDTO Item { get; set; }
        public Dictionary<long, long?> KpiItemContentKpiCriteriaItemMappings { get; set; }


        public Mobile_KpiItemContentDTO() { }
        public Mobile_KpiItemContentDTO(KpiItemContent KpiItemContent)
        {
            this.Id = KpiItemContent.Id;
            this.KpiItemId = KpiItemContent.KpiItemId;
            this.ItemId = KpiItemContent.ItemId;
            this.Item = KpiItemContent.Item == null ? null : new Mobile_ItemDTO(KpiItemContent.Item);
            this.KpiItemContentKpiCriteriaItemMappings = KpiItemContent.KpiItemContentKpiCriteriaItemMappings?.ToDictionary(x => x.KpiCriteriaItemId, y => y.Value);
            this.Errors = KpiItemContent.Errors;
        }
    }
}