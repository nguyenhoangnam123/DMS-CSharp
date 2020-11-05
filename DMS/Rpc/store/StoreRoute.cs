using DMS.Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.store
{
    public class StoreRoute : Root
    {
        public const string Parent = Module + "/location";
        public const string Master = Module + "/location/store/store-master";
        public const string Detail = Module + "/location/store/store-detail/*";
        public const string Mobile = Module + ".store.*";
        private const string Default = Rpc + Module + "/store";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string GetDraft = Default + "/get-draft";
        public const string Create = Default + "/create";
        public const string CreateStoreScouting = Default + "/create-store-scouting";
        public const string Update = Default + "/update";
        public const string Approve = Default + "/approve";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListParentStore = Default + "/filter-list-parent-store";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListStoreStatus = Default + "/filter-list-store-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListParentStore = Default + "/single-list-parent-store";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListStoreStatus = Default + "/single-list-store-status";

        public const string ListReseller = Default + "/list-reseller";
        public const string CountReseller = Default + "/count-reseller";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(StoreFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(StoreFilter.StoreTypeId), FieldTypeEnum.ID.Id },
            { nameof(StoreFilter.StoreGroupingId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType } },
            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get, GetDraft,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, Create, CreateStoreScouting, SaveImage,
                SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, SingleListParentStore, SingleListStoreStatus} },
            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, Update, SaveImage,
                SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, SingleListParentStore, SingleListStoreStatus} },
            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Delete, } },
            { "Xoá nhiều", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, BulkDelete } },
            { "Xuất excel", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, Export } },
            { "Nhập excel", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, ExportTemplate, Import } },
            { "Phê duyệt", new List<string> {
                Parent,
                Master, Count, List, Get,
                FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore, FilterListStoreStatus, FilterListStoreGrouping, FilterListStoreType,
                Detail, Approve, 
                SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, SingleListParentStore, SingleListStoreStatus} },
        };
    }
}
