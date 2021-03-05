using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_store_state_change
{
    public class ReportStoreStateChangeRoute : Root
    {
        public const string Parent = Module + "/store-report";
        public const string Master = Module + "/store-report/report-store-state-change-master";

        private const string Default = Rpc + Module + "/report-store-state-change";
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
            { nameof(ReportStoreStateChange_ReportStoreStateChangeFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreStateChange_ReportStoreStateChangeFilterDTO.StoreGroupingId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreStateChange_ReportStoreStateChangeFilterDTO.StoreTypeId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreStateChange_ReportStoreStateChangeFilterDTO.StoreId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Export,
                FilterListOrganization, FilterListStore, FilterListStoreType, FilterListStoreGrouping, FilterListStoreStatus } },

        };
    }
}
