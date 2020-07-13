using Common;
using System.Collections.Generic;

namespace DMS.Rpc.banner
{
    public class BannerRoute : Root
    {
        public const string Master = Module + "/application-banner/banner/banner-master";
        public const string Detail = Module + "/application-banner/banner/banner-detail/*";
        private const string Default = Rpc + Module + "/banner";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string>
            {
                Master,
                Detail,Count,List,Get,
                FilterListAppUser, FilterListStatus}
            },
            { "Thêm mới", new List<string>
            {
                Master,
                Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Create,
                SingleListAppUser, SingleListAppUser, SingleListStatus}
            },
            { "Sửa", new List<string>
            {
                Master,
                Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Update,
                SingleListAppUser, SingleListAppUser, SingleListStatus}
            },
            { "Lưu ảnh", new List<string>
            {
                SaveImage}
            },
            { "Xoá", new List<string>
            {
                Master,
                Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Delete,
                SingleListAppUser, SingleListAppUser, SingleListStatus}
            },
            { "Xoá nhiều", new List<string>
            {
                Master,
                Detail,Count,List,Get, BulkDelete,
                FilterListAppUser, FilterListStatus}
            },
            { "Xuất excel", new List<string>
            {
                Master,
                Detail,Count,List,Get, Export,
                FilterListAppUser, FilterListStatus}
            },
            { "Nhập excel", new List<string>
            {
                Master,
                Detail,Count,List,Get, ExportTemplate, Import,
                FilterListAppUser, FilterListStatus}
            },
        };
    }
}
