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
using DMS.Services.MDirectPriceList;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItem;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStore;
using DMS.Services.MStoreType;

namespace DMS.Rpc.direct_price_list
{
    public class DirectPriceListRoute : Root
    {
        public const string Master = Module + "/direct-price-list/direct-price-list-master";
        public const string Detail = Module + "/direct-price-list/direct-price-list-detail";
        private const string Default = Rpc + Module + "/direct-price-list";
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

        public const string FilterListDirectPriceListType = Default + "/filter-list-direct-price-list-type";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";

        public const string SingleListDirectPriceListType = Default + "/single-list-direct-price-list-type";
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
            { nameof(DirectPriceListFilter.Code), FieldType.STRING },
            { nameof(DirectPriceListFilter.Name), FieldType.STRING },
            { nameof(DirectPriceListFilter.StartDate), FieldType.DATE },
            { nameof(DirectPriceListFilter.EndDate), FieldType.DATE },
            { nameof(DirectPriceListFilter.StatusId), FieldType.ID },
            { nameof(DirectPriceListFilter.OrganizationId), FieldType.ID },
            { nameof(DirectPriceListFilter.DirectPriceListTypeId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { Master, Count, List, Get, FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, } },
            { "Thêm", new List<string> { Master, Count, List, Get,  FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, Detail, Create,  SingleListDirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },
            { "Sửa", new List<string> { Master, Count, List, Get,  FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, Detail, Update,  SingleListDirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },
            { "Xoá", new List<string> { Master, Count, List, Get,  FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, Detail, Delete,  SingleListDirectPriceListType, SingleListOrganization, SingleListStatus, SingleListItem, SingleListStoreGrouping, SingleListStore, SingleListStoreType, } },
            { "Xoá nhiều", new List<string> { Master, Count, List, Get, FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, BulkDelete } },
            { "Xuất excel", new List<string> { Master, Count, List, Get, FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, Export } },
            { "Nhập excel", new List<string> { Master, Count, List, Get, FilterListDirectPriceListType, FilterListOrganization, FilterListStatus, FilterListItem, FilterListStoreGrouping, FilterListStore, FilterListStoreType, ExportTemplate, Import } },
        };
    }
}
