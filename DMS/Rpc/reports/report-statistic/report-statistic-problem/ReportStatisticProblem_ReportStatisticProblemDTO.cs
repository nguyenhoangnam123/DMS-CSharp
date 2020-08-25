using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_statistic.report_statistic_problem
{
    public class ReportStatisticProblem_ReportStatisticProblemDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportStatisticProblem_StoreDTO> Stores { get; set; }
    }

    public class ReportStatisticProblem_ReportStatisticProblemFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public DateFilter Date { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        internal bool HasValue => (OrganizationId != null && OrganizationId.HasValue) ||
            (Date != null && Date.HasValue) ||
            (StoreId != null && StoreId.HasValue) ||
            (StoreTypeId != null && StoreTypeId.HasValue) ||
            (StoreGroupingId != null && StoreGroupingId.HasValue);
    }
}
