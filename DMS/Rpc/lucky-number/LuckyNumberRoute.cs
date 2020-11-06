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
using DMS.Services.MLuckyNumber;
using DMS.Services.MRewardStatus;

namespace DMS.Rpc.lucky_number
{
    public class LuckyNumberRoute : Root
    {
        public const string Master = Module + "/lucky-number/lucky-number-master";
        public const string Detail = Module + "/lucky-number/lucky-number-detail";
        private const string Default = Rpc + Module + "/lucky-number";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string BulkDelete = Default + "/bulk-delete";
        
        
        public const string FilterListRewardStatus = Default + "/filter-list-reward-status";
        

        
        public const string SingleListRewardStatus = Default + "/single-list-reward-status";
        
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(LuckyNumberFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(LuckyNumberFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(LuckyNumberFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(LuckyNumberFilter.RewardStatusId), FieldTypeEnum.ID.Id },
            { nameof(LuckyNumberFilter.RowId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List,
                Get, GetPreview,
                
                 FilterListRewardStatus, } },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                Detail, Create, 
                SingleListRewardStatus, 
                 } },

            { "Sửa", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                Detail, Update, 
                SingleListRewardStatus,  
                 } },

            { "Xoá", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                Delete, 
                SingleListRewardStatus,  } },

            { "Xoá nhiều", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                BulkDelete } },

            { "Xuất excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                Export } },

            { "Nhập excel", new List<string> { 
                Master, Count, List, Get, GetPreview,
                FilterListRewardStatus,  
                ExportTemplate, Import } },
        };
    }
}
