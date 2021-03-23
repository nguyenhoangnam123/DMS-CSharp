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
using DMS.Services.MShowingItem;
using DMS.Services.MCategory;
using DMS.Services.MStatus;
using DMS.Services.MUnitOfMeasure;
using System.ComponentModel;

namespace DMS.Rpc.showing_item
{
    [DisplayName("Sản phẩm trưng bày")]
    public class ShowingItemRoute : Root
    {
        public const string Parent = Module + "/posm";
        public const string Master = Module + "/posm/showing-item/showing-item-master";
        public const string Detail = Module + "/posm/showing-item/showing-item-detail";
        public const string Preview = Module + "/showing-item/showing-item-preview";
        private const string Default = Rpc + Module + "/showing-item";
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
        
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListUnitOfMeasure = Default + "/filter-list-unit-of-measure";

        public const string SingleListCategory = Default + "/single-list-category";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ShowingItemFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ShowingItemFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ShowingItemFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ShowingItemFilter.CategoryId), FieldTypeEnum.ID.Id },
            { nameof(ShowingItemFilter.UnitOfMeasureId), FieldTypeEnum.ID.Id },
            { nameof(ShowingItemFilter.SalePrice), FieldTypeEnum.DECIMAL.Id },
            { nameof(ShowingItemFilter.Desception), FieldTypeEnum.STRING.Id },
            { nameof(ShowingItemFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(ShowingItemFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListCategory,FilterListStatus,FilterListUnitOfMeasure,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListCategory, SingleListStatus, SingleListUnitOfMeasure, 
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
