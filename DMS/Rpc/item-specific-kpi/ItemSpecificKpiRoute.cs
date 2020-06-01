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
        
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        
        public const string FilterListItemSpecificKpiContent = Default + "/filter-list-item-specific-kpi-content";
        
        public const string FilterListItem = Default + "/filter-list-item";
        
        public const string FilterListItemSpecificCriteria = Default + "/filter-list-item-specific-criteria";
        
        public const string FilterListTotalItemSpecificCriteria = Default + "/filter-list-total-item-specific-criteria";
        

        
        public const string SingleListAppUser = Default + "/single-list-app-user";
        
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        
        public const string SingleListOrganization = Default + "/single-list-organization";
        
        public const string SingleListStatus = Default + "/single-list-status";
        
        public const string SingleListItemSpecificKpiContent = Default + "/single-list-item-specific-kpi-content";
        
        public const string SingleListItem = Default + "/single-list-item";
        
        public const string SingleListItemSpecificCriteria = Default + "/single-list-item-specific-criteria";
        
        public const string SingleListTotalItemSpecificCriteria = Default + "/single-list-total-item-specific-criteria";
        
        public const string CountTotalItemSpecificCriteria = Default + "/count-total-item-specific-criteria";
        public const string ListTotalItemSpecificCriteria = Default + "/list-total-item-specific-criteria";
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
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                Detail, Create, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificCriteria, SingleListTotalItemSpecificCriteria, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                Detail, Update, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificCriteria, SingleListTotalItemSpecificCriteria, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                Detail, Delete, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificCriteria, SingleListTotalItemSpecificCriteria, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificCriteria, FilterListTotalItemSpecificCriteria, 
                ExportTemplate, Import } },
        };
    }
}
