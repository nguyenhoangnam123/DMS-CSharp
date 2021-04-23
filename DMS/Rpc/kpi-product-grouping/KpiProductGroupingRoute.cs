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
using DMS.Services.MKpiProductGrouping;
using DMS.Services.MAppUser;
using DMS.Services.MKpiPeriod;
using DMS.Services.MKpiProductGroupingType;
using DMS.Services.MStatus;

namespace DMS.Rpc.kpi_product_grouping
{
    public class KpiProductGroupingRoute : Root
    {
        public const string Parent = Module + "/kpi-product-grouping";
        public const string Master = Module + "/kpi-product-grouping/kpi-product-grouping-master";
        public const string Detail = Module + "/kpi-product-grouping/kpi-product-grouping-detail";
        public const string Preview = Module + "/kpi-product-grouping/kpi-product-grouping-preview";
        private const string Default = Rpc + Module + "/kpi-product-grouping";
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
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListKpiPeriod = Default + "/filter-list-kpi-period";
        public const string FilterListKpiYear = Default + "/filter-list-kpi-year";
        public const string FilterListKpiProductGroupingType = Default + "/filter-list-kpi-product-grouping-type";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";
        public const string FilterListBrand = Default + "/filter-list-brand";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListKpiPeriod = Default + "/single-list-kpi-period";
        public const string SingleListKpiProductGroupingType = Default + "/single-list-kpi-product-grouping-type";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string ListAppUser = Default + "list-app-user";
        public const string CountAppUser = Default + "count-app-user";
        public const string ListItem = Default + "list-item";
        public const string CountItem = Default + "count-item";


        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(KpiProductGroupingFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.KpiYearId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.KpiPeriodId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.KpiProductGroupingTypeId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.EmployeeId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(KpiProductGroupingFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListAppUser,FilterListOrganization, FilterListKpiPeriod,FilterListKpiYear, FilterListKpiProductGroupingType, 
            FilterListStatus, FilterListCategory, FilterListProductType, FilterListProductGrouping, FilterListBrand
        };
        private static List<string> SingleList = new List<string> { 
            SingleListAppUser, SingleListKpiPeriod, SingleListKpiProductGroupingType, SingleListStatus, 
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
