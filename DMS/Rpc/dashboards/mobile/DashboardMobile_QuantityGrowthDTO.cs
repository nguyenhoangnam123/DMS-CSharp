using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobile_QuantityGrowthDTO : DataDTO
    {
        public List<DashboardMobile_QuantityGrowthByMonthDTO> IndirectSalesOrderQuantityGrowthByMonths { get; set; }
        public List<DashboardMobile_QuantityGrowthByQuarterDTO> IndirectSalesOrderQuantityGrowthByQuaters { get; set; }
        public List<DashboardMobile_QuantityGrowthByYearDTO> IndirectSalesOrderQuantityGrowthByYears { get; set; }

        public List<DashboardMobile_QuantityGrowthByMonthDTO> DirectSalesOrderQuantityGrowthByMonths { get; set; }
        public List<DashboardMobile_QuantityGrowthByQuarterDTO> DirectSalesOrderQuantityGrowthByQuaters { get; set; }
        public List<DashboardMobile_QuantityGrowthByYearDTO> DirectSalesOrderQuantityGrowthByYears { get; set; }
    }

    public class DashboardMobile_QuantityGrowthByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
        public decimal DirectSalesOrderCounter { get; set; }
    }

    public class DashboardMobile_QuantityGrowthByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
        public decimal DirectSalesOrderCounter { get; set; }

    }

    public class DashboardMobile_QuantityGrowthByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
        public decimal DirectSalesOrderCounter { get; set; }
    }

    public class DashboardMobile_QuantityGrowthFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
