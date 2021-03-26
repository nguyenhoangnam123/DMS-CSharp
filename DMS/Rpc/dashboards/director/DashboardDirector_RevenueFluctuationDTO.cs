using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirector_RevenueFluctuationDTO : DataDTO
    {
        public List<DashboardDirector_RevenueFluctuationByMonthDTO> RevenueFluctuationByMonths { get; set; }
        public List<DashboardDirector_RevenueFluctuationByQuarterDTO> RevenueFluctuationByQuaters { get; set; }
        public List<DashboardDirector_RevenueFluctuationByYearDTO> RevenueFluctuationByYears { get; set; }
    }

    public class DashboardDirector_RevenueFluctuationByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardDirector_RevenueFluctuationByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardDirector_RevenueFluctuationByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardDirector_RevenueFluctuationFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
