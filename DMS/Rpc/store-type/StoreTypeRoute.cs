using Common;
using System.Collections.Generic;

namespace DMS.Rpc.store_type
{
    public class StoreTypeRoute : Root
    {
        public const string Master = Module + "/location/store-type/store-type-master";
        public const string Detail = Module + "/location/store-type/store-type-detail/*";
        private const string Default = Rpc + Module + "/store-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, } },
            { "Thêm", new List<string> {
                Master, Count, List, Get,
                Detail, Create,
                SingleListStatus, } },
            { "Sửa", new List<string> {
                Master, Count, List, Get,
                Detail, Update,
                SingleListStatus, } },
            { "Xoá", new List<string> {
                Master, Count, List, Get,
                Detail, Delete,
                SingleListStatus, } },
            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get,
                BulkDelete } },
        };
    }
}
