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
using DMS.Services.MStoreScoutingType;

namespace DMS.Rpc.store_scouting_type
{
    public class StoreScoutingTypeRoute : Root
    {
        public const string Master = Module + "/location/store-scouting-type/store-scouting-type-master";
        public const string Detail = Module + "/location/store-scouting-type/store-scouting-type-detail/*";
        private const string Default = Rpc + Module + "/store-scouting-type";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";
        
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(StoreScoutingTypeFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(StoreScoutingTypeFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(StoreScoutingTypeFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(StoreScoutingTypeFilter.StatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                Get, GetPreview,
                FilterListStatus, 
                 } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                Detail, Create,
                SingleListStatus,
                 } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                Detail, Update,
                SingleListStatus,
                 } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                Delete, 
                 } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListStatus,
                ExportTemplate, Import } },
        };
    }
}
