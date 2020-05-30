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
using DMS.Services.MTotalItemSpecificCriteria;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.total_item_specific_criteria
{
    public class TotalItemSpecificCriteriaRoute : Root
    {
        public const string Master = Module + "/total-item-specific-criteria/total-item-specific-criteria-master";
        public const string Detail = Module + "/total-item-specific-criteria/total-item-specific-criteria-detail";
        private const string Default = Rpc + Module + "/total-item-specific-criteria";
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
        

        
        public const string SingleListItemSpecificKpi = Default + "/single-list-item-specific-kpi";
        
        public const string CountItemSpecificKpi = Default + "/count-item-specific-kpi";
        public const string ListItemSpecificKpi = Default + "/list-item-specific-kpi";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(TotalItemSpecificCriteriaFilter.Id), FieldType.ID },
            { nameof(TotalItemSpecificCriteriaFilter.Code), FieldType.STRING },
            { nameof(TotalItemSpecificCriteriaFilter.Name), FieldType.STRING },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, 
                Detail, Create, 
                 SingleListItemSpecificKpi, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, 
                Detail, Update, 
                 SingleListItemSpecificKpi, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpi, 
                Detail, Delete, 
                 SingleListItemSpecificKpi, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpi, 
                ExportTemplate, Import } },
        };
    }
}
