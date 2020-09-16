using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobile_KpiGeneralCriterialDTO : DataDTO
    {
        public long KpiCriterialId { get; set; }
        public string KpiCriterialName { get; set; }
        internal decimal? Plan { get; set; }
        public decimal? Value { get; set; }
        public decimal? Rate => Plan == null || Plan == 0 ? null : (decimal?)Math.Round(Value.Value / Plan.Value * 100, 0);
    }

    public class DashboardMobile_KpiGeneralCriterialFilterDTO : FilterDTO
    {
        public IdFilter Period { get; set; }
    }
}
