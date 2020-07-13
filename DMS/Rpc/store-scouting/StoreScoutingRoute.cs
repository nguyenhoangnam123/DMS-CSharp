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
using DMS.Services.MStoreScouting;
using DMS.Services.MAppUser;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MStore;
using DMS.Services.MStoreScoutingStatus;
using DMS.Services.MWard;

namespace DMS.Rpc.store_scouting
{
    public class StoreScoutingRoute : Root
    {
        public const string Master = Module + "/location/store-scouting/store-scouting-master";
        public const string Detail = Module + "/location/store-scouting/store-scouting-detail/*";
        private const string Default = Rpc + Module + "/store-scouting";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Reject = Default + "/reject";
        
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreScoutingStatus = Default + "/filter-list-store-scouting-status";
        public const string FilterListWard = Default + "/filter-list-ward";
        
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreScoutingStatus = Default + "/single-list-store-scouting-status";
        public const string SingleListWard = Default + "/single-list-ward";
        
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(StoreScoutingFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.ProvinceId), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.DistrictId), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.WardId), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingFilter.StoreScoutingStatusId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListStore, FilterListStoreScoutingStatus, FilterListWard, } },

            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListStore, FilterListStoreScoutingStatus, FilterListWard, 
                Detail, Create, 
                 SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStore, SingleListStoreScoutingStatus, SingleListWard, } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListStore, FilterListStoreScoutingStatus, FilterListWard, 
                Detail, Update, 
                 SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStore, SingleListStoreScoutingStatus, SingleListWard, } },

            { "Từ chối", new List<string> { 
                Master, Count, List, Get,  FilterListAppUser, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListStore, FilterListStoreScoutingStatus, FilterListWard, 
                Detail, Reject, 
                 SingleListAppUser, SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStore, SingleListStoreScoutingStatus, SingleListWard, } },

        };
    }
}
