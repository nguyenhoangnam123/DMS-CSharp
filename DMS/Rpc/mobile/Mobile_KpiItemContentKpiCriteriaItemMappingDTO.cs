using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiItemContentKpiCriteriaItemMappingDTO : DataDTO
    {
        public long KpiItemContentId { get; set; }
        public long KpiCriteriaItemId { get; set; }
        public long? Value { get; set; }

        public Mobile_KpiItemContentKpiCriteriaItemMappingDTO() { }
        public Mobile_KpiItemContentKpiCriteriaItemMappingDTO(KpiItemContentKpiCriteriaItemMapping KpiItemContentKpiCriteriaItemMapping)
        {
            this.KpiItemContentId = KpiItemContentKpiCriteriaItemMapping.KpiItemContentId;
            this.KpiCriteriaItemId = KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId;
            this.Value = KpiItemContentKpiCriteriaItemMapping.Value;
        }
    }
}
