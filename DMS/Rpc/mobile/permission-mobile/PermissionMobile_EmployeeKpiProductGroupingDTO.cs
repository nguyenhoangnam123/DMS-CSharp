using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public class PermissionMobile_EmployeeKpiProductGroupingReportDTO
    {
        internal long ProductGroupingId { get; set; }
        public string ProductGroupingName { get; set; } // tên ProductGrouping được áp dụng KPI

        public List<PermissionMobile_EmployeeKpiProductGrouping> CurrentKpiProductGroupings { get; set; }
    }

    public class PermissionMobile_EmployeeKpiProductGrouping
    {
        public long ProductGroupingId { get; set; }
        public string KpiProductGroupingCriteriaName { get; set; }
        public decimal PlannedValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal Percentage { get; set; } // giá trị thực hiện 

    }

    public class PermissionMobile_EmployeeKpiProductGroupingReportFilterDTO : FilterDTO
    {
        public IdFilter EmployeeId { get; set; }
    }
}
