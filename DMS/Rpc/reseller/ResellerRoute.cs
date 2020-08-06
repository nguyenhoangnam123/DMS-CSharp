using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reseller
{
    public class ResellerRoute : Root
    {
        public const string Parent = Module + "/reseller";
        public const string Master = Module + "/reseller/reseller-master";
        public const string Detail = Module + "/reseller/reseller-detail";
        private const string Default = Rpc + Module + "/reseller";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Approve = Default + "/approve";
        public const string Reject = Default + "/reject";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListResellerStatus = Default + "/filter-list-reseller-status";
        public const string FilterListResellerType = Default + "/filter-list-reseller-type";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListResellerStatus = Default + "/single-list-reseller-status";
        public const string SingleListResellerType = Default + "/single-list-reseller-type";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus, } },

            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                Detail, Create,
                SingleListOrganization, SingleListResellerStatus, SingleListResellerType, SingleListAppUser, SingleListStatus, CountStore, ListStore } },

            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                Detail, Update,
                SingleListOrganization, SingleListResellerStatus, SingleListResellerType, SingleListAppUser, SingleListStatus, CountStore, ListStore } },

            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                Detail, Delete,
                SingleListOrganization, SingleListResellerStatus, SingleListResellerType, SingleListAppUser, SingleListStatus, CountStore, ListStore } },

            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                Export } },

            { "Nhập excel", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListOrganization, FilterListResellerStatus, FilterListResellerType, FilterListAppUser, FilterListStatus,
                ExportTemplate, Import } },
        };
    }
}
