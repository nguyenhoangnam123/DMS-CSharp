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
using DMS.Services.MShowingWarehouse;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;
using DMS.Services.MShowingInventory;
using DMS.Services.MAppUser;
using DMS.Services.MShowingItem;
using System.ComponentModel;

namespace DMS.Rpc.posm.showing_warehouse
{
    [DisplayName("Tồn kho sản phẩm trưng bày")]
    public class ShowingWarehouseRoute : Root
    {
        public const string Parent = Module + "/posm";
        public const string Master = Module + "/posm/showing-item/showing-warehouse-master";
        public const string Detail = Module + "/posm/showing-item/showing-warehouse-detail/*";
        public const string Preview = Module + "/showing-warehouse/showing-warehouse-preview";
        private const string Default = Rpc + Module + "/showing-warehouse";
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
        
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListWard = Default + "/filter-list-ward";
        public const string FilterListShowingInventory = Default + "/filter-list-showing-inventory";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListShowingItem = Default + "/filter-list-showing-item";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListShowingInventory = Default + "/single-list-showing-inventory";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListShowingItem = Default + "/single-list-showing-item";

        public const string ListHistory = Default + "/list-history";
        public const string CountHistory = Default + "/count-history";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ShowingWarehouseFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ShowingWarehouseFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ShowingWarehouseFilter.Address), FieldTypeEnum.STRING.Id },
            { nameof(ShowingWarehouseFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.ProvinceId), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.DistrictId), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.WardId), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(ShowingWarehouseFilter.RowId), FieldTypeEnum.ID.Id },
        };

        private static List<string> FilterList = new List<string> { 
            FilterListDistrict,FilterListOrganization,FilterListProvince,FilterListStatus,FilterListWard,FilterListShowingInventory,FilterListAppUser,FilterListShowingItem,
        };
        private static List<string> SingleList = new List<string> { 
            SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListWard, SingleListShowingInventory, SingleListAppUser, SingleListShowingItem, 
        };
        private static List<string> CountList = new List<string> {
            CountHistory, ListHistory, 
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
