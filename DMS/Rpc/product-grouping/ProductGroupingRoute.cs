using Common;
using System.Collections.Generic;

namespace DMS.Rpc.product_grouping
{
    public class ProductGroupingRoute : Root
    {
        public const string Master = Module + "/product-category/product-grouping/product-grouping-master";
        public const string Detail = Module + "/product-category/product-grouping/product-grouping-detail/*";
        private const string Default = Rpc + Module + "/product-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListProduct = Default + "/filter-list-product";

        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProduct = Default + "/single-list-product";

        public const string CountProduct = Default + "/count-product";
        public const string ListProduct = Default + "/list-product";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct, } },

            { "Thêm", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                Detail, Create,
                SingleListProductGrouping, SingleListProduct,
                CountProduct, ListProduct, } },

            { "Sửa", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                Detail, Update,
                SingleListProductGrouping, SingleListProduct,
                CountProduct, ListProduct,} },

            { "Xoá", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                Detail, Delete,
                SingleListProductGrouping, SingleListProduct, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListProductGrouping, FilterListProduct,
                ExportTemplate, Import } },
        };
    }
}
