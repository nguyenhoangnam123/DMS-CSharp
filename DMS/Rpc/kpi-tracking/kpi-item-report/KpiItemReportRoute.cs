using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReportRoute : Root
    {
        public const string Parent = Module + "/kpi-tracking";
        public const string Master = Module + "/kpi-tracking/kpi-item-report/kpi-item-report-master";

        private const string Default = Rpc + Module + "/kpi-item-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListKpiYear = Default + "/filter-list-kpi-year";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(KpiItemReport_KpiItemReportFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(KpiItemReport_KpiItemReportFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Export,
                FilterListOrganization,FilterListAppUser,FilterListKpiYear, FilterListKpiPeriod, FilterListItem} },

        };
    }
}
