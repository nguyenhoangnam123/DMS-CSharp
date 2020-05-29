using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MIndirectPriceList;
using DMS.Services.MIndirectPriceListType;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItem;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;

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
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(IndirectPriceListFilter.Code), FieldType.STRING },
            { nameof(IndirectPriceListFilter.Name), FieldType.STRING },
            { nameof(IndirectPriceListFilter.StartDate), FieldType.DATE },
            { nameof(IndirectPriceListFilter.EndDate), FieldType.DATE },
            { nameof(IndirectPriceListFilter.StatusId), FieldType.ID },
            { nameof(IndirectPriceListFilter.OrganizationId), FieldType.ID },
            { nameof(IndirectPriceListFilter.IndirectPriceListTypeId), FieldType.ID },
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
