using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_ItemDTO : DataDTO
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public long SaleEmployeeId { get; set; }
        public long KpiCriteriaItemId { get; set; }
        public decimal Value { get; set; }
    }
}
