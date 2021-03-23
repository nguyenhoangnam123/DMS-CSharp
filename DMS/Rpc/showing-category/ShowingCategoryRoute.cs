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
using DMS.Services.MShowingCategory;
using DMS.Services.MImage;
using DMS.Services.MCategory;
using DMS.Services.MStatus;

namespace DMS.Rpc.showing_category
{
    public class ShowingCategoryRoute : Root
    {
        public const string Parent = Module + "/showing-category";
        public const string Master = Module + "/showing-category/showing-category-master";
        public const string Detail = Module + "/showing-category/showing-category-detail";
        public const string Preview = Module + "/showing-category/showing-category-preview";
        private const string Default = Rpc + Module + "/showing-category";
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
        
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListCategory = Default + "/single-list-category";
        public const string SingleListStatus = Default + "/single-list-status";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ShowingCategoryFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ShowingCategoryFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ShowingCategoryFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ShowingCategoryFilter.ParentId), FieldTypeEnum.ID.Id },
            { nameof(ShowingCategoryFilter.Path), FieldTypeEnum.STRING.Id },
            { nameof(ShowingCategoryFilter.Level), FieldTypeEnum.LONG.Id },
            { nameof(ShowingCategoryFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(ShowingCategoryFilter.ImageId), FieldTypeEnum.ID.Id },
            { nameof(ShowingCategoryFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListImage,FilterListCategory,FilterListStatus,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListImage, SingleListCategory, SingleListStatus, 
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
