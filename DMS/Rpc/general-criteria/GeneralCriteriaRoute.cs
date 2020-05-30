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
using DMS.Services.MGeneralCriteria;
using DMS.Services.MGeneralKpi;
using DMS.Services.MStatus;

namespace DMS.Rpc.general_criteria
{
    public class GeneralCriteriaRoute : Root
    {
        public const string Master = Module + "/general-criteria/general-criteria-master";
        public const string Detail = Module + "/general-criteria/general-criteria-detail";
        private const string Default = Rpc + Module + "/general-criteria";
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
        
        
        public const string FilterListGeneralKpi = Default + "/filter-list-general-kpi";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        

        
        public const string SingleListGeneralKpi = Default + "/single-list-general-kpi";
        
        public const string SingleListStatus = Default + "/single-list-status";
        
        public const string CountGeneralKpi = Default + "/count-general-kpi";
        public const string ListGeneralKpi = Default + "/list-general-kpi";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(GeneralCriteriaFilter.Id), FieldType.ID },
            { nameof(GeneralCriteriaFilter.Code), FieldType.STRING },
            { nameof(GeneralCriteriaFilter.Name), FieldType.STRING },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListGeneralKpi, FilterListStatus, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListGeneralKpi, FilterListStatus, 
                Detail, Create, 
                 SingleListGeneralKpi, SingleListStatus, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListGeneralKpi, FilterListStatus, 
                Detail, Update, 
                 SingleListGeneralKpi, SingleListStatus, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListGeneralKpi, FilterListStatus, 
                Detail, Delete, 
                 SingleListGeneralKpi, SingleListStatus, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListGeneralKpi, FilterListStatus, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListGeneralKpi, FilterListStatus, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListGeneralKpi, FilterListStatus, 
                ExportTemplate, Import } },
        };
    }
}
