using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirector_Top5RevenueByEmployeeDTO : DataDTO
    {
        public string EmployeeName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardDirector_Top5RevenueByEmployeeFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter Time { get; set; }
    }
}
