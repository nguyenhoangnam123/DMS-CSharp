using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.warehouse
{
    public class WarehouseRoute : Root
    {
        public const string Master = Module + "/inventory/warehouse/warehouse-master";
        public const string Detail = Module + "/inventory/warehouse/warehouse-detail/*";
        private const string Default = Rpc + Module + "/warehouse";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string GetPreview = Default + "/get-preview";
        public const string ListHistory = Default + "/list-history";
        public const string CountHistory = Default + "/count-history";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string ImportInventory = Default + "/import-inventory";
        public const string ExportInventory = Default + "/export-inventory";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";


        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductTypeId = Default + "/single-list-product-grouping";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(WarehouseFilter.OrganizationId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListStatus,
                CountHistory, ListHistory,
                } },
            { "Thêm", new List<string> {
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListStatus,
                Create, CountHistory, ListHistory,
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListWard, SingleListItem, } },
            { "Sửa", new List<string> {
                Master, Count, List, Get,GetPreview,
                FilterListOrganization, FilterListStatus,
                Detail, Update, CountHistory, ListHistory,
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListWard, SingleListItem, } },
            { "Xoá", new List<string> {
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListStatus,
                Detail, Delete,
                } },
            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListStatus,
                BulkDelete } },
            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListOrganization, FilterListStatus,
                Detail, ExportInventory } },
            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListOrganization, FilterListStatus,
                Detail, ExportTemplate, ImportInventory } },
        };
    }
}
