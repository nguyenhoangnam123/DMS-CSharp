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
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.item_specific_criteria
{
    public class ItemSpecificCriteriaRoute : Root
    {
        public const string Master = Module + "/item-specific-criteria/item-specific-criteria-master";
        public const string Detail = Module + "/item-specific-criteria/item-specific-criteria-detail";
        private const string Default = Rpc + Module + "/item-specific-criteria";
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
        
        
        public const string FilterListItemSpecificKpiContent = Default + "/filter-list-item-specific-kpi-content";
        
        public const string FilterListItem = Default + "/filter-list-item";
        
        public const string FilterListItemSpecificKpi = Default + "/filter-list-item-specific-kpi";
        

        
        public const string SingleListItemSpecificKpiContent = Default + "/single-list-item-specific-kpi-content";
        
        public const string SingleListItem = Default + "/single-list-item";
        
        public const string SingleListItemSpecificKpi = Default + "/single-list-item-specific-kpi";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ItemSpecificCriteriaFilter.Id), FieldType.ID },
            { nameof(ItemSpecificCriteriaFilter.Code), FieldType.STRING },
            { nameof(ItemSpecificCriteriaFilter.Name), FieldType.STRING },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                Detail, Create, 
                 SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificKpi, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                Detail, Update, 
                 SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificKpi, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                Detail, Delete, 
                 SingleListItemSpecificKpiContent, SingleListItem, SingleListItemSpecificKpi, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListItemSpecificKpiContent, FilterListItem, FilterListItemSpecificKpi, 
                ExportTemplate, Import } },
        };
    }
}
