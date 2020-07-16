﻿using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_un_checked
{
    public class ReportStoreUnCheckedRoute : Root
    {
        public const string Master = Module + "/report-store-un-checked/report-store-un-checked-master";

        private const string Default = Rpc + Module + "/report-store-un-checked";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListERoute = Default + "/filter-list-e-route";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStoreChecked_ReportStoreCheckedFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreChecked_ReportStoreCheckedFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(ReportStoreChecked_ReportStoreCheckedFilterDTO.ERouteId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Export, FilterListOrganization, FilterListAppUser, FilterListERoute } },

        };
    }
}
