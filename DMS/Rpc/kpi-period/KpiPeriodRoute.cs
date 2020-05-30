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
using DMS.Services.MKpiPeriod;
using DMS.Services.MItemSpecificKpi;
using DMS.Services.MOrganization;
using DMS.Services.MStatus;

namespace DMS.Rpc.kpi_period
{
    public class KpiPeriodRoute : Root
    {
        public const string Master = Module + "/kpi-period/kpi-period-master";
        public const string Detail = Module + "/kpi-period/kpi-period-detail";
        private const string Default = Rpc + Module + "/kpi-period";
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
        
        
        public const string FilterListItemSpecificKpi = Default + "/filter-list-item-specific-kpi";
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        

        
        public const string SingleListItemSpecificKpi = Default + "/single-list-item-specific-kpi";
        
        public const string SingleListOrganization = Default + "/single-list-organization";
        
        public const string SingleListStatus = Default + "/single-list-status";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(KpiPeriodFilter.Id), FieldType.ID },
            { nameof(KpiPeriodFilter.Code), FieldType.STRING },
            { nameof(KpiPeriodFilter.Name), FieldType.STRING },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                Detail, Create, 
                 SingleListItemSpecificKpi, SingleListOrganization, SingleListStatus, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                Detail, Update, 
                 SingleListItemSpecificKpi, SingleListOrganization, SingleListStatus, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                Detail, Delete, 
                 SingleListItemSpecificKpi, SingleListOrganization, SingleListStatus, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, FilterListOrganization, FilterListStatus, 
                ExportTemplate, Import } },
        };
    }
}
