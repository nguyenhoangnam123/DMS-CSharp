using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_KpiItemKpiCriteriaTotalMapping : DataDTO
    {
        public long KpiItemId { get; set; }
        public long KpiCriteriaTotalId { get; set; }
        public decimal Value { get; set; }


        //public List<MonitorStoreChecker_StoreCheckingDTO> StoreCheckings { get; set; }
    }
}
