using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobile_RevenueGrowthDTO : DataDTO
    {
        public List<DashboardMobile_RevenueGrowthByMonthDTO> IndirectRevenueGrowthByMonths { get; set; }
        public List<DashboardMobile_RevenueGrowthByQuarterDTO> IndirectRevenueGrowthByQuaters { get; set; }
        public List<DashboardMobile_RevenueGrowthByYearDTO> IndirectRevenueGrowthByYears { get; set; }
        public List<DashboardMobile_RevenueGrowthByMonthDTO> DirectRevenueGrowthByMonths { get; set; }
        public List<DashboardMobile_RevenueGrowthByQuarterDTO> DirectRevenueGrowthByQuaters { get; set; }
        public List<DashboardMobile_RevenueGrowthByYearDTO> DirectRevenueGrowthByYears { get; set; }
    }

    public class DashboardMobile_RevenueGrowthByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardMobile_RevenueGrowthByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardMobile_RevenueGrowthByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardMobile_RevenueGrowthFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
