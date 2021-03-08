using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirector_IndirectSalesOrderFluctuationDTO : DataDTO
    {
        public List<DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO> IndirectSalesOrderFluctuationByMonths { get; set; }
        public List<DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO> IndirectSalesOrderFluctuationByQuaters { get; set; }
        public List<DashboardDirector_IndirectSalesOrderFluctuationByYearDTO> IndirectSalesOrderFluctuationByYears { get; set; }
    }

    public class DashboardDirector_IndirectSalesOrderFluctuationByMonthDTO : DataDTO
    {
        public long Day { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
    }

    public class DashboardDirector_IndirectSalesOrderFluctuationByQuarterDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
    }

    public class DashboardDirector_IndirectSalesOrderFluctuationByYearDTO : DataDTO
    {
        public long Month { get; set; }
        public decimal IndirectSalesOrderCounter { get; set; }
    }

    public class DashboardDirector_IndirectSalesOrderFluctuationFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
