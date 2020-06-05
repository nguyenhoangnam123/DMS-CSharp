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
using DMS.Services.MStoreGrouping;
using DMS.Services.MStatus;
using DMS.Services.MStore;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MReseller;
using DMS.Services.MStoreType;
using DMS.Services.MWard;

namespace DMS.Rpc.store_grouping
{
    public class StoreGroupingRoute : Root
    {
        public const string Master = Module + "/store-grouping/store-grouping-master";
        public const string Detail = Module + "/store-grouping/store-grouping-detail";
        private const string Default = Rpc + Module + "/store-grouping";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListParentStoreGrouping = Default + "/single-list-parent-store-store";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, Detail, Create, SingleListStatus, } },
            { "Sửa", new List<string> { Master, Count, List, Get, 
                Detail, Update, SingleListStatus, } },
            { "Xoá", new List<string> { 
                Master, Count, List, Get, 
                Detail, Delete, 
                SingleListStatus, } },
            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, 
                BulkDelete } },
            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, 
                Export } },
            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, 
                Import } },
        };
    }
}
