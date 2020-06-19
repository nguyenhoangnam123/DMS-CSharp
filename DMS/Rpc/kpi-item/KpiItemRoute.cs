using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_item
{
    public class KpiItemRoute : Root
    {
        public const string Master = Module + "/kpi-item/kpi-item-master";
        public const string Detail = Module + "/kpi-item/kpi-item-detail";
        private const string Default = Rpc + Module + "/kpi-item";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string GetDraft = Default + "/get-draft";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListKpiItemContent = Default + "/filter-list-kpi-item-content";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListKpiCriteriaItem = Default + "/filter-list-criteria-item";
        public const string FilterListKpiCriteriaTotal = Default + "/filter-list-criteria-total";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListSupplier = Default + "/filter-list-supplier";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListKpiItemContent = Default + "/single-list-kpi-item-content";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListKpiCriteriaItem = Default + "/single-list-criteria-item";
        public const string SingleListKpiCriteriaTotal = Default + "/single-list-criteria-total";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(KpiItemFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(KpiItemFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(KpiItemFilter.KpiPeriodId), FieldTypeEnum.ID.Id },
            { nameof(KpiItemFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(KpiItemFilter.EmployeeId), FieldTypeEnum.ID.Id },
            { nameof(KpiItemFilter.CreatorId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier} },

            { "Thêm", new List<string> {
                Master, Count, List, Get, CountAppUser, ListAppUser,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                Detail, Create, GetDraft,
                SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, SingleListKpiCriteriaItem, SingleListKpiCriteriaTotal, } },

            { "Sửa", new List<string> {
                Master, Count, List, Get, CountAppUser, ListAppUser,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                Detail, Update, GetDraft,
                SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, SingleListKpiCriteriaItem, SingleListKpiCriteriaTotal, } },

            { "Xoá", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                Detail, Delete,
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, FilterListKpiCriteriaItem, FilterListKpiCriteriaTotal, FilterListProductGrouping, FilterListProductType, FilterListSupplier,
                ExportTemplate, Import } },
        };
    }
}
