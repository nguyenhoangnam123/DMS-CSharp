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
using DMS.Services.MItemSpecificKpiContent;
using DMS.Services.MItem;
using DMS.Services.MItemSpecificCriteria;
using DMS.Services.MItemSpecificKpi;

namespace DMS.Rpc.item_specific_kpi_content
{
    public class ItemSpecificKpiContentRoute : Root
    {
        public const string Master = Module + "/item-specific-kpi-content/item-specific-kpi-content-master";
        public const string Detail = Module + "/item-specific-kpi-content/item-specific-kpi-content-detail";
        private const string Default = Rpc + Module + "/item-specific-kpi-content";
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
        
        
        public const string FilterListItem = Default + "/filter-list-item";
        
        public const string FilterListItemSpecificCriteria = Default + "/filter-list-item-specific-criteria";
        
        public const string FilterListItemSpecificKpi = Default + "/filter-list-item-specific-kpi";
        

        
        public const string SingleListItem = Default + "/single-list-item";
        
        public const string SingleListItemSpecificCriteria = Default + "/single-list-item-specific-criteria";
        
        public const string SingleListItemSpecificKpi = Default + "/single-list-item-specific-kpi";
        
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ItemSpecificKpiContentFilter.Id), FieldType.ID },
            { nameof(ItemSpecificKpiContentFilter.ItemSpecificKpiId), FieldType.ID },
            { nameof(ItemSpecificKpiContentFilter.ItemSpecificCriteriaId), FieldType.ID },
            { nameof(ItemSpecificKpiContentFilter.ItemId), FieldType.ID },
            { nameof(ItemSpecificKpiContentFilter.Value), FieldType.LONG },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                Detail, Create, 
                 SingleListItem, SingleListItemSpecificCriteria, SingleListItemSpecificKpi, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                Detail, Update, 
                 SingleListItem, SingleListItemSpecificCriteria, SingleListItemSpecificKpi, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                Detail, Delete, 
                 SingleListItem, SingleListItemSpecificCriteria, SingleListItemSpecificKpi, } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListItem, FilterListItemSpecificCriteria, FilterListItemSpecificKpi, 
                ExportTemplate, Import } },
        };
    }
}
