using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_KpiItemContentKpiCriteriaItemMappingDTO : DataDTO
    {
        public long KpiItemContentId { get; set; }
        public long KpiCriteriaItemId { get; set; }
        public long? Value { get; set; }

        public KpiItem_KpiItemContentKpiCriteriaItemMappingDTO() { }
        public KpiItem_KpiItemContentKpiCriteriaItemMappingDTO(KpiItemContentKpiCriteriaItemMapping KpiItemContentKpiCriteriaItemMapping)
        {
            this.KpiItemContentId = KpiItemContentKpiCriteriaItemMapping.KpiItemContentId;
            this.KpiCriteriaItemId = KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId;
            this.Value = KpiItemContentKpiCriteriaItemMapping.Value;
        }
    }
}
