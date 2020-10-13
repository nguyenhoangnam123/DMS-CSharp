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
using DMS.Services.MPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MPriceListType;
using DMS.Services.MSalesOrderType;
using DMS.Services.MStatus;

namespace DMS.Rpc.price_list
{
    public class PriceListRoute : Root
    {
        public const string Parent = Module + "/price-list-and-promotion";
        public const string Master = Module + "/price-list-and-promotion/price-list/price-list-master";
        public const string Detail = Module + "/price-list-and-promotion/price-list/price-list-detail/*";
        private const string Default = Rpc + Module + "/price-list";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string ImportItem = Default + "/import-item";
        public const string ExportItem = Default + "/export-item";
        public const string ExportTemplateItem = Default + "/export-template-item";
        public const string ImportStore = Default + "/import-store";
        public const string ExportStore = Default + "/export-store";
        public const string ExportTemplateStore = Default + "/export-template-store";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListPriceListType = Default + "/filter-list-price-list-type";
        public const string FilterListSalesOrderType = Default + "/filter-list-sales-order-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListPriceListType = Default + "/single-list-price-list-type";
        public const string SingleListSalesOrderType = Default + "/single-list-sales-order-type";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountPriceListItemHistory = Default + "/count-price-list-item-history";
        public const string ListPriceListItemHistory = Default + "/list-price-list-item-history";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(PriceListFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PriceListFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent, Master, Count, List,
                Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },
            { "Thêm", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Detail, Create,
                SingleListItem, SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus, SingleListProductGrouping, SingleListProductType, SingleListStoreGrouping, SingleListStore, SingleListStoreType, SingleListProvince,
                Count, List, Count, List, Count, List, Count, List,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },

            { "Sửa", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Detail, Update,
                SingleListItem, SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus, SingleListProductGrouping, SingleListProductType, SingleListStoreGrouping, SingleListStore, SingleListStoreType, SingleListProvince,
                Count, List, Count, List, Count, List, Count, List,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                }},

            { "Xoá", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                Delete,
                SingleListItem, SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus, SingleListProductGrouping, SingleListProductType, SingleListStoreGrouping, SingleListStore, SingleListStoreType, SingleListProvince,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },

            { "Xoá nhiều", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                BulkDelete,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },

            { "Xuất excel", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                ExportItem, ExportStore,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },

            { "Nhập excel", new List<string> {
                Parent, Master, Count, List, Get, GetPreview,
                FilterListItem, FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, FilterListStoreGrouping, FilterListStore, FilterListStoreType,
                ExportTemplateItem, ExportTemplateStore, ImportItem, ImportStore,
                CountItem, ListItem, CountStore, ListStore, CountPriceListItemHistory, ListPriceListItemHistory
                } },
        };
    }
}
