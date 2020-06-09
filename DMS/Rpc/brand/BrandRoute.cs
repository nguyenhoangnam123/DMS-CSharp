using Common;
using System.Collections.Generic;

namespace DMS.Rpc.brand
{
    public class BrandRoute : Root
    {
        public const string Master = Module + "/brand/brand-master";
        public const string Detail = Module + "/brand/brand-detail";
        private const string Default = Rpc + Module + "/brand";
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

        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { Master, Count, List, Get, FilterListStatus, } },
            { "Thêm", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Create,  SingleListStatus, } },
            { "Sửa", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Update,  SingleListStatus, } },
            { "Xoá", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Delete,  SingleListStatus, } },
            { "Xoá nhiều", new List<string> { Master, Count, List, Get, FilterListStatus, BulkDelete } },
            { "Xuất excel", new List<string> { Master, Count, List, Get, FilterListStatus, Export } },
            { "Nhập excel", new List<string> { Master, Count, List, Get, FilterListStatus, ExportTemplate, Import } },
        };
    }
}
