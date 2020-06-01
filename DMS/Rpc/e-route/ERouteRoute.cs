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
using DMS.Services.MERoute;
using DMS.Services.MAppUser;
using DMS.Services.MERouteType;
using DMS.Services.MRequestState;
using DMS.Services.MStatus;
using DMS.Services.MERouteContent;
using DMS.Services.MStore;

namespace DMS.Rpc.e_route
{
    public class ERouteRoute : Root
    {
        public const string Master = Module + "/e-route/e-route-master";
        public const string Detail = Module + "/e-route/e-route-detail";
        private const string Default = Rpc + Module + "/e-route";
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

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListERouteType = Default + "/filter-list-eroute-type";
        public const string FilterListRequestState = Default + "/filter-list-request-state";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListERouteType = Default + "/single-list-eroute-type";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListRequestState = Default + "/single-list-request-state";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ERouteFilter.Code), FieldType.STRING },
            { nameof(ERouteFilter.Name), FieldType.STRING },
            { nameof(ERouteFilter.SaleEmployeeId), FieldType.ID },
            { nameof(ERouteFilter.StartDate), FieldType.DATE },
            { nameof(ERouteFilter.EndDate), FieldType.DATE },
            { nameof(ERouteFilter.RequestStateId), FieldType.ID },
            { nameof(ERouteFilter.StatusId), FieldType.ID },
            { nameof(ERouteFilter.CreatorId), FieldType.ID },
        };


        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore,
                Detail, Create,  
                SingleListAppUser, SingleListERouteType, SingleListRequestState, SingleListStatus,  SingleListStore, SingleListOrganization, SingleListStoreType,
                CountStore, ListStore, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, 
                Detail, Update, 
                SingleListAppUser, SingleListERouteType, SingleListRequestState, SingleListStatus,  SingleListStore,  SingleListOrganization, SingleListStoreType,
                CountStore, ListStore, } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, 
                Detail, Delete,  } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, 
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, 
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListERouteType, FilterListRequestState, FilterListStatus,  FilterListStore, ExportTemplate, 
                Import } },
        };
    }
}
