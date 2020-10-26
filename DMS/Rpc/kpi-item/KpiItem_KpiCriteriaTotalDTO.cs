using DMS.Common;
using DMS.Entities;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_KpiCriteriaTotalDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public KpiItem_KpiCriteriaTotalDTO() { }
        public KpiItem_KpiCriteriaTotalDTO(KpiCriteriaTotal KpiCriteriaTotal)
        {

            this.Id = KpiCriteriaTotal.Id;

            this.Code = KpiCriteriaTotal.Code;

            this.Name = KpiCriteriaTotal.Name;

            this.Errors = KpiCriteriaTotal.Errors;
        }
    }
}
