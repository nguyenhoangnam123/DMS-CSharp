using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpiRoute : Root
    {
        public const string Master = Module + "/general-kpi/general-kpi-master";
        public const string Detail = Module + "/general-kpi/general-kpi-detail";
        private const string Default = Rpc + Module + "/general-kpi";
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
        public const string CreateDraft = Default + "/create-draft";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(GeneralKpiFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(GeneralKpiFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(GeneralKpiFilter.EmployeeId), FieldTypeEnum.ID.Id },
            { nameof(GeneralKpiFilter.KpiPeriodId), FieldTypeEnum.ID.Id },
            { nameof(GeneralKpiFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(GeneralKpiFilter.CreatorId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, } },

            { "Thêm", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Detail, Create, CreateDraft,
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, CountAppUser, ListAppUser} },

            { "Sửa", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Detail, Update,
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, CountAppUser, ListAppUser} },

            { "Xoá", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Detail, Delete,
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                ExportTemplate, Import } },
        };
    }
}
