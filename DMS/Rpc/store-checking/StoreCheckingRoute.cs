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
using DMS.Services.MStoreChecking;
using DMS.Services.MAppUser;
using DMS.Services.MStore;
using DMS.Services.MAlbum;
using DMS.Services.MImage;

namespace DMS.Rpc.store_checking
{
    public class StoreCheckingRoute : Root
    {
        public const string Master = Module + "/store-checking/store-checking-master";
        public const string Detail = Module + "/store-checking/store-checking-detail";
        private const string Default = Rpc + Module + "/store-checking";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string CreateIndirectSalesOrder = Default + "/create-indirect-sales-order";
        public const string CreateProblem = Default + "/create-problem";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";

        public const string SingleListAlbum = Default + "/single-list-album";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListEroute = Default + "/single-list-e-route";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListProblemType = Default + "/single-list-problem-type";


        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string ListStore = Default + "/list-store";
        public const string CountStore = Default + "/count-store";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StoreCheckingFilter.Longtitude), FieldType.DECIMAL },
            { nameof(StoreCheckingFilter.Latitude), FieldType.DECIMAL },
            { nameof(StoreCheckingFilter.CheckInAt), FieldType.DATE },
            { nameof(StoreCheckingFilter.CheckOutAt), FieldType.DATE },
            { nameof(StoreCheckingFilter.CountIndirectSalesOrder), FieldType.LONG },
            { nameof(StoreCheckingFilter.CountImage), FieldType.LONG },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListStore, } },
            { "Checkin", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListStore, 
                Detail, Create, Update, CreateIndirectSalesOrder, CreateProblem, SaveImage, 
                SingleListAlbum, SingleListAppUser, SingleListStore, SingleListTaxType, SingleListUnitOfMeasure, SingleListProblemType, } },
        };
    }
}
