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
using DMS.Services.MItemSpecificKpi;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MTotalItemSpecificCriteria;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpiRoute : Root
    {
        public const string Master = Module + "/item-specific-kpi/item-specific-kpi-master";
        public const string Detail = Module + "/item-specific-kpi/item-specific-kpi-detail";
        private const string Default = Rpc + Module + "/item-specific-kpi";
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
        public const string CreateDraft = Default + "/create-draft";
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ItemSpecificKpiFilter.Id), FieldType.ID },
            { nameof(ItemSpecificKpiFilter.OrganizationId), FieldType.ID },
            { nameof(ItemSpecificKpiFilter.KpiPeriodId), FieldType.ID },
            { nameof(ItemSpecificKpiFilter.StatusId), FieldType.ID },
            { nameof(ItemSpecificKpiFilter.EmployeeId), FieldType.ID },
            { nameof(ItemSpecificKpiFilter.CreatorId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, 
                Detail, Create, CreateDraft,
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListProductGrouping, SingleListProductType, SingleListSupplier,SingleListStatus, 
                CountAppUser, ListAppUser, CountItem, ListItem} },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, 
                Detail, Update, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListProductGrouping, SingleListProductType, SingleListSupplier,SingleListStatus,
                CountAppUser, ListAppUser, CountItem, ListItem } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Detail, Delete, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListProductGrouping, SingleListProductType, SingleListSupplier,SingleListStatus,  } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus,
                ExportTemplate, Import } },
        };
    }
}
