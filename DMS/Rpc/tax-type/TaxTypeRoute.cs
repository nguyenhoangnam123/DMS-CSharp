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
using DMS.Services.MTaxType;
using DMS.Services.MStatus;

namespace DMS.Rpc.tax_type
{
    public class TaxTypeRoute : Root
    {
        public const string Master = Module + "/tax-type/tax-type-master";
        public const string Detail = Module + "/tax-type/tax-type-detail";
        private const string Default = Rpc + Module + "/tax-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(TaxTypeFilter.Code), FieldType.STRING },
            { nameof(TaxTypeFilter.Name), FieldType.STRING },
            { nameof(TaxTypeFilter.Percentage), FieldType.DECIMAL },
            { nameof(TaxTypeFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, 
                Detail, Create,  
                SingleListStatus, } },
            { "Sửa", new List<string> { 
                Master, Count, List, Get, 
                Detail, Update,  
                SingleListStatus, } },
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