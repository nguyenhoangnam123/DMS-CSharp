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
using DMS.Services.MGeneralKpi;
using DMS.Services.MKpiPeriod;
using DMS.Services.MOrganization;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpiRoute : Root
    {
        public const string Master = Module + "/general-kpi/general-kpi-master";
        public const string Detail = Module + "/general-kpi/general-kpi-detail";
        private const string Default = Rpc + Module + "/general-kpi";
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
        
        
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        
        public const string FilterListOrganization = Default + "/filter-list-organization";
        

        
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        
        public const string SingleListOrganization = Default + "/single-list-organization";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(GeneralKpiFilter.Id), FieldType.ID },
            { nameof(GeneralKpiFilter.OrganizationId), FieldType.ID },
            { nameof(GeneralKpiFilter.EmployeeId), FieldType.ID },
            { nameof(GeneralKpiFilter.KpiPeriodId), FieldType.ID },
            { nameof(GeneralKpiFilter.StatusId), FieldType.ID },
            { nameof(GeneralKpiFilter.CreatorId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListKpiPeriod, FilterListOrganization, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListKpiPeriod, FilterListOrganization, 
                Detail, Create, 
                 SingleListKpiPeriod, SingleListOrganization, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListKpiPeriod, FilterListOrganization, 
                Detail, Update, 
                 SingleListKpiPeriod, SingleListOrganization, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListKpiPeriod, FilterListOrganization, 
                Detail, Delete, 
                 SingleListKpiPeriod, SingleListOrganization, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListKpiPeriod, FilterListOrganization, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListKpiPeriod, FilterListOrganization, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListKpiPeriod, FilterListOrganization, 
                ExportTemplate, Import } },
        };
    }
}
