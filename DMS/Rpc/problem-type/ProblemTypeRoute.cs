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
using DMS.Services.MProblemType;
using System.ComponentModel;

namespace DMS.Rpc.problem_type
{
    [DisplayName("Loại vấn đề")]
    public class ProblemTypeRoute : Root
    {
        public const string Master = Module + "/problem-type/problem-type-master";
        public const string Detail = Module + "/problem-type/problem-type-detail/*";
        private const string Default = Rpc + Module + "/problem-type";
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
            { nameof(ProblemTypeFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ProblemTypeFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(ProblemTypeFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(ProblemTypeFilter.StatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
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
