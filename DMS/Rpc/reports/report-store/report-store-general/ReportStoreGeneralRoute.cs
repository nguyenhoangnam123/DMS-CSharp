using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneralRoute : Root
    {
        public const string Parent = Module + "/store-report";
        public const string Master = Module + "/store-report/store-general-report-master";

        private const string Default = Rpc + Module + "/store-general-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStoreStatus = Default + "/filter-list-store-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStoreGeneral_ReportStoreGeneralFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreGroupingId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreTypeId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreGeneral_ReportStoreGeneralFilterDTO.StoreId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Export, 
                FilterListOrganization, FilterListStore, FilterListStoreType, FilterListStoreGrouping, FilterListStoreStatus } },

        };
    }
}
