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
using DMS.Services.MStore;
using DMS.Services.MDistrict;
using DMS.Services.MOrganization;
using DMS.Services.MProvince;
using DMS.Services.MReseller;
using DMS.Services.MStatus;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MWard;

namespace DMS.Rpc.store
{
    public class StoreRoute : Root
    {
        public const string Master = Module + "/store/store-master";
        public const string Detail = Module + "/store/store-detail";
        private const string Default = Rpc + Module + "/store";
        public const string Mobile = Default + "/master-data.stores";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Approve = Default + "/approve";
        public const string Reject = Default + "/reject";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListParentStore = Default + "/filter-list-parent-store";
        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListWard = Default + "/filter-list-ward";

        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListParentStore = Default + "/single-list-parent-store";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListWard = Default + "/single-list-ward";

        public const string ListReseller = Default + "/list-reseller";
        public const string CountReseller = Default + "/count-reseller";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(StoreFilter.Code), FieldType.STRING },
            { nameof(StoreFilter.Name), FieldType.STRING },
            { nameof(StoreFilter.ParentStoreId), FieldType.ID },
            { nameof(StoreFilter.OrganizationId), FieldType.ID },
            { nameof(StoreFilter.StoreTypeId), FieldType.ID },
            { nameof(StoreFilter.StoreGroupingId), FieldType.ID },
            { nameof(StoreFilter.ResellerId), FieldType.ID },
            { nameof(StoreFilter.Telephone), FieldType.STRING },
            { nameof(StoreFilter.ProvinceId), FieldType.ID },
            { nameof(StoreFilter.DistrictId), FieldType.ID },
            { nameof(StoreFilter.WardId), FieldType.ID },
            { nameof(StoreFilter.Address), FieldType.STRING },
            { nameof(StoreFilter.DeliveryAddress), FieldType.STRING },
            { nameof(StoreFilter.Latitude), FieldType.DECIMAL },
            { nameof(StoreFilter.Longitude), FieldType.DECIMAL },
            { nameof(StoreFilter.DeliveryLatitude), FieldType.DECIMAL },
            { nameof(StoreFilter.DeliveryLongitude), FieldType.DECIMAL },
            { nameof(StoreFilter.OwnerName), FieldType.STRING },
            { nameof(StoreFilter.OwnerPhone), FieldType.STRING },
            { nameof(StoreFilter.OwnerEmail), FieldType.STRING },
            { nameof(StoreFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, Create,  
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, } },
            { "Sửa", new List<string> { 
                Master, Count, List, Get,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, Update, 
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, } },
            { "Xoá", new List<string> { 
                Master, Count, List, Get,  FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, Delete, 
                SingleListDistrict, SingleListOrganization, SingleListProvince, SingleListStatus, SingleListStoreGrouping, SingleListStoreType, SingleListWard, } },
            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, BulkDelete } },
            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, Export } },
            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, FilterListDistrict, FilterListOrganization, FilterListProvince, FilterListWard, 
                Detail, ExportTemplate, Import } },
        };
    }
}
