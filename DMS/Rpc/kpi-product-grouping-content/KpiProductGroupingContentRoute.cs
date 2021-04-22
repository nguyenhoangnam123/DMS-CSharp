using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using OfficeOpenXml;
using DMS.Entities;
using DMS.Services.MKpiProductGroupingContent;
using DMS.Services.MKpiProductGrouping;

namespace DMS.Rpc.kpi_product_grouping_content
{
    public class KpiProductGroupingContentRoute : Root
    {
        public const string Parent = Module + "/kpi-product-grouping-content";
        public const string Master = Module + "/kpi-product-grouping-content/kpi-product-grouping-content-master";
        public const string Detail = Module + "/kpi-product-grouping-content/kpi-product-grouping-content-detail";
        public const string Preview = Module + "/kpi-product-grouping-content/kpi-product-grouping-content-preview";
        private const string Default = Rpc + Module + "/kpi-product-grouping-content";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListKpiProductGrouping = Default + "/filter-list-kpi-product-grouping";

        public const string SingleListKpiProductGrouping = Default + "/single-list-kpi-product-grouping";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(KpiProductGroupingContentFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingContentFilter.KpiProductGroupingId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingContentFilter.ProductGroupingId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingContentFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListKpiProductGrouping,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListKpiProductGrouping, 
        };
        private static List<string> CountList = new List<string> { 
            
        };
        
        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List,
                    Get,  
                }.Concat(FilterList)
            },
            { "Thêm", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Detail, Create, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Sửa", new List<string> { 
                    Parent,            
                    Master, Preview, Count, List, Get,
                    Detail, Update, 
                }.Concat(SingleList).Concat(FilterList).Concat(CountList)
            },

            { "Xoá", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Delete, 
                }.Concat(SingleList).Concat(FilterList) 
            },

            { "Xoá nhiều", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    BulkDelete 
                }.Concat(FilterList) 
            },

            { "Xuất excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    Export 
                }.Concat(FilterList) 
            },

            { "Nhập excel", new List<string> { 
                    Parent,
                    Master, Preview, Count, List, Get,
                    ExportTemplate, Import 
                }.Concat(FilterList) 
            },
        };
    }
}
