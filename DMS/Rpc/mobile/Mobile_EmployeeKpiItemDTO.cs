using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_EmployeeKpiItemDTO
    {
        public string ItemName { get; set; } // tên Item được áp dụng KPI

        public List<Mobile_EmployeeKpiItem> CurrentKpiItems { get; set; }
    }

    public class Mobile_EmployeeKpiItem
    {
        public long ItemId { get; set; }
        public string KpiCriteriaItemName { get; set; }
        public decimal PlannedValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal Percentage { get; set; } // giá trị thực hiện 

    }
}
