using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_statistic_problem
{
    public class ReportStatisticProblem_TotalDTO : DataDTO
    {
        public long WaitingCounter { get; set; }
        public long ProcessCounter { get; set; }
        public long CompletedCounter { get; set; }
        public long Total => WaitingCounter + ProcessCounter + CompletedCounter;
    }
}
