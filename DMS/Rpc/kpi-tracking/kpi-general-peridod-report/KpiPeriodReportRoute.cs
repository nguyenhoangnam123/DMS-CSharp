using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiPeriodGeneralReportRoute : Root
    {
        public const string Master = Module + "/kpi-period-report/kpi-period-report-master";

        private const string Default = Rpc + Module + "/kpi-period-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListKpiYear = Default + "/filter-list-kpi-year";

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Export, FilterListOrganization,FilterListAppUser,FilterListKpiYear, FilterListKpiPeriod} },

        };
    }
}
