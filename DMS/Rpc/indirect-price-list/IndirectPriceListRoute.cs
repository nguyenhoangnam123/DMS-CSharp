using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceListRoute : Root
    {
        public const string Master = Module + "/indirect-price-list/indirect-price-list-master";
        public const string Detail = Module + "/indirect-price-list/indirect-price-list-detail";
        private const string Default = Rpc + Module + "/indirect-price-list";
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

        public const string FilterListIndirectPriceListType = Default + "/filter-list-indirect-price-list-type";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";

        public const string SingleListIndirectPriceListType = Default + "/single-list-indirect-price-list-type";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreType = Default + "/single-list-store-type";

        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string CountStoreGrouping = Default + "/count-store-grouping";
        public const string ListStoreGrouping = Default + "/list-store-grouping";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountStoreType = Default + "/count-store-type";
        public const string ListStoreType = Default + "/list-store-type";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(IndirectPriceListFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(IndirectPriceListFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(IndirectPriceListFilter.StartDate), FieldTypeEnum.DATE.Id },
            { nameof(IndirectPriceListFilter.EndDate), FieldTypeEnum.DATE.Id },
            { nameof(IndirectPriceListFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(IndirectPriceListFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(IndirectPriceListFilter.IndirectPriceListTypeId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, } },

            { "Thêm", new List<string> {
                Master, Count, List, Get,  FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Detail, Create,
                SingleListIndirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },

            { "Sửa", new List<string> {
                Master, Count, List, Get,  FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Detail, Update,
                SingleListIndirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },

            { "Xoá", new List<string> {
                Master, Count, List, Get,  FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Detail, Delete,
                SingleListIndirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListIndirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                ExportTemplate, Import } },
        };
    }
}
