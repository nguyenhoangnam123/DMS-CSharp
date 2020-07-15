using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.store
{
    public class StoreRoute : Root
    {
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
        public const string Reject = Default + "/reject";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListParentStore = Default + "/filter-list-parent-store";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListParentStore = Default + "/single-list-parent-store";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        

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
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore } },
            { "Thêm", new List<string> {
                Master, Count, List, Get, GetDraft,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Detail, Create, CreateStoreScouting, SaveImage,
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, SingleListParentStore} },
            { "Sửa", new List<string> {
                Master, Count, List, Get,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Detail, Update, SaveImage,
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, SingleListParentStore} },
            { "Xoá", new List<string> {
                Master, Count, List, Get,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Delete, } },
            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Detail, BulkDelete } },
            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Detail, Export } },
            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, FilterListStatus, FilterListParentStore,
                Detail, ExportTemplate, Import } },
        };
    }
}
