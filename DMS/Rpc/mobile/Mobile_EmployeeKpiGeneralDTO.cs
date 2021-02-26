using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_EmployeeKpiGeneralReportDTO
    {
        public string KpiCriticalGeneralName { get; set; } // tên chỉ tiêu
        public decimal PlannedValue { get; set; } // giá trị kế hoạch trong tháng
        public decimal CurrentValue { get; set; } // giá trị thực hiện trong tháng
        public decimal Percentage { get; set; } // giá trị thực hiện 
    }

    public class Mobile_EmployeeKpiGeneralReportFilterDTO : FilterDTO
    {
        public IdFilter EmployeeId { get; set; }
    }
}
