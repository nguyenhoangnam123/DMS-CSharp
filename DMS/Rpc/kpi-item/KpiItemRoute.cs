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
using DMS.Services.MKpiItem;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MItem;

namespace DMS.Rpc.kpi_item
{
    public class KpiItemRoute : Root
    {
        public const string Master = Module + "/kpi-item/kpi-item-master";
        public const string Detail = Module + "/kpi-item/kpi-item-detail";
        private const string Default = Rpc + Module + "/kpi-item";
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
        public const string FilterListKpiItemContent = Default + "/filter-list-kpi-item-content";
        public const string FilterListItem = Default + "/filter-list-item";

        
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListKpiItemContent = Default + "/single-list-kpi-item-content";
        public const string SingleListItem = Default + "/single-list-item";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(KpiItemFilter.Id), FieldType.ID },
            { nameof(KpiItemFilter.OrganizationId), FieldType.ID },
            { nameof(KpiItemFilter.KpiPeriodId), FieldType.ID },
            { nameof(KpiItemFilter.StatusId), FieldType.ID },
            { nameof(KpiItemFilter.EmployeeId), FieldType.ID },
            { nameof(KpiItemFilter.CreatorId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                Detail, Create, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                Detail, Update, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                Detail, Delete, 
                 SingleListAppUser, SingleListKpiPeriod, SingleListOrganization, SingleListStatus, SingleListKpiItemContent, SingleListItem, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListKpiPeriod, FilterListOrganization, FilterListStatus, FilterListKpiItemContent, FilterListItem, 
                ExportTemplate, Import } },
        };
    }
}
