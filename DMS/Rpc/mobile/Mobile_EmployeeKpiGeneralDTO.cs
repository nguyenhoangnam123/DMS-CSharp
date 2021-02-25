using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_EmployeeKpiGeneralDTO
    {
        public string KpiCriticalGeneralName { get; set; } // tên chỉ tiêu
        public decimal PlannedValue { get; set; } // giá trị kế hoạch trong tháng
        public decimal CurrentValue { get; set; } // giá trị thực hiện trong tháng
        public decimal Percentage { get; set; } // giá trị thực hiện 
    }
}
