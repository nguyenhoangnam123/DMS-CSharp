﻿using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_checked
{
    public class ReportStoreCheckedRoute : Root
    {
        public const string Master = Module + "/store-checking-report/store-checked-report-master";

        private const string Default = Rpc + Module + "/store-checked-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreChecked_ReportStoreCheckedFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Export, FilterListOrganization, FilterListAppUser, FilterListStore, FilterListStoreType, FilterListStoreGrouping  } },

        };
    }
}