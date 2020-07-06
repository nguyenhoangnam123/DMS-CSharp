using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreCheckerRoute : Root
    {
        public const string Master = Module + "/report-store-checker/report-store-checker-master";

        private const string Default = Rpc + Module + "/report-store-checker";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStoreChecker_ReportStoreCheckerFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreChecker_ReportStoreCheckerFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, Export, FilterListOrganization,FilterListAppUser,FilterListStore, FilterListStoreType,FilterListStoreGrouping  } },

        };
    }
}
