using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobile_TopRevenueBySalesEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string SaleEmployeeName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardMobile_TopRevenueBySalesEmployeeFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
