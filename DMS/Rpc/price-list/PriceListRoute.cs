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
        public const string Master = Module + "/price-list/price-list-master";
        public const string Detail = Module + "/price-list/price-list-detail";
        private const string Default = Rpc + Module + "/price-list";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListPriceListType = Default + "/filter-list-price-list-type";
        public const string FilterListSalesOrderType = Default + "/filter-list-sales-order-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListPriceListType = Default + "/single-list-price-list-type";
        public const string SingleListSalesOrderType = Default + "/single-list-sales-order-type";
        public const string SingleListStatus = Default + "/single-list-status";

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
            { nameof(PriceListFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(PriceListFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(PriceListFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(PriceListFilter.StartDate), FieldTypeEnum.DATE.Id },
            { nameof(PriceListFilter.EndDate), FieldTypeEnum.DATE.Id },
            { nameof(PriceListFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(PriceListFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(PriceListFilter.PriceListTypeId), FieldTypeEnum.ID.Id },
            { nameof(PriceListFilter.SalesOrderTypeId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                Get, GetPreview,
                
                 FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                Detail, Create, 
                SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus, 
                Count, List, Count, List, Count, List, Count, List,  } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                Detail, Update, 
                SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus,  
                Count, List, Count, List, Count, List, Count, List,  } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                Delete, 
                SingleListOrganization, SingleListPriceListType, SingleListSalesOrderType, SingleListStatus,  } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListOrganization, FilterListPriceListType, FilterListSalesOrderType, FilterListStatus,  
                ExportTemplate, Import } },
        };
    }
}
