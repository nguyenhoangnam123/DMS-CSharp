using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneralRoute : Root
    {
        public const string Master = Module + "/store-report/store-general-report-master";

        private const string Default = Rpc + Module + "/store-general-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Export, FilterListOrganization, FilterListStore, FilterListStoreType, FilterListStoreGrouping  } },

        };
    }
}
