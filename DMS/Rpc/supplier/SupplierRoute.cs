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
using DMS.Services.MSupplier;
using DMS.Services.MDistrict;
using DMS.Services.MAppUser;
using DMS.Services.MProvince;
using DMS.Services.MStatus;
using DMS.Services.MWard;

namespace DMS.Rpc.supplier
{
    public class SupplierRoute : Root
    {
        public const string Master = Module + "/supplier/supplier-master";
        public const string Detail = Module + "/supplier/supplier-detail";
        private const string Default = Rpc + Module + "/supplier";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListStatus = Default + "/filter-list-status";


        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListPersonInCharge = Default + "/single-list-person-in-charge";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListWard = Default + "/single-list-ward";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(SupplierFilter.Code), FieldType.STRING },
            { nameof(SupplierFilter.Name), FieldType.STRING },
            { nameof(SupplierFilter.TaxCode), FieldType.STRING },
            { nameof(SupplierFilter.Phone), FieldType.STRING },
            { nameof(SupplierFilter.Email), FieldType.STRING },
            { nameof(SupplierFilter.Address), FieldType.STRING },
            { nameof(SupplierFilter.ProvinceId), FieldType.ID },
            { nameof(SupplierFilter.DistrictId), FieldType.ID },
            { nameof(SupplierFilter.WardId), FieldType.ID },
            { nameof(SupplierFilter.OwnerName), FieldType.STRING },
            { nameof(SupplierFilter.PersonInChargeId), FieldType.ID },
            { nameof(SupplierFilter.StatusId), FieldType.ID },
            { nameof(SupplierFilter.Description), FieldType.STRING },
            { nameof(SupplierFilter.UpdatedTime), FieldType.DATE },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, FilterListStatus} },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, FilterListStatus,
                Detail, Create,  
                SingleListDistrict, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Sửa", new List<string> { 
                Master, Count, List, Get, FilterListStatus,
                Detail, Update,  
                SingleListDistrict, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Xoá", new List<string> { 
                Master, Count, List, Get, FilterListStatus,
                Detail, Delete,  
                SingleListDistrict, SingleListProvince, SingleListStatus, SingleListWard, } },
            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, FilterListStatus,
                BulkDelete } },
        };
    }
}
