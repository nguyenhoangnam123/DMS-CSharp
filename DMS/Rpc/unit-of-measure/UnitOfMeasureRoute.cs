using Common;
using System.Collections.Generic;

namespace DMS.Rpc.unit_of_measure
{
    public class UnitOfMeasureRoute : Root
    {
        public const string Master = Module + "/product-category/unit-of-measure/unit-of-measure-master";
        public const string Detail = Module + "/product-category/unit-of-measure/unit-of-measure-detail/*";
        private const string Default = Rpc + Module + "/unit-of-measure";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
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
            { "Xuất excel", new List<string> {
                Master, Count, List, Get,
                Export } },
            { "Nhập excel", new List<string> {
                Master, Count, List, Get,
                Import } },
        };
    }
}
