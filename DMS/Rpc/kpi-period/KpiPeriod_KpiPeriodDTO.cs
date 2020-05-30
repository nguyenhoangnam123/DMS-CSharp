using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_period
{
    public class KpiPeriod_KpiPeriodDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<KpiPeriod_ItemSpecificKpiDTO> ItemSpecificKpis { get; set; }
        public KpiPeriod_KpiPeriodDTO() {}
        public KpiPeriod_KpiPeriodDTO(KpiPeriod KpiPeriod)
        {
            this.Id = KpiPeriod.Id;
            this.Code = KpiPeriod.Code;
            this.Name = KpiPeriod.Name;
            this.ItemSpecificKpis = KpiPeriod.ItemSpecificKpis?.Select(x => new KpiPeriod_ItemSpecificKpiDTO(x)).ToList();
            this.Errors = KpiPeriod.Errors;
        }
    }

    public class KpiPeriod_KpiPeriodFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public KpiPeriodOrder OrderBy { get; set; }
    }
}
