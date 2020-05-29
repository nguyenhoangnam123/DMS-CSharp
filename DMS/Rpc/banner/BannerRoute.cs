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
using DMS.Services.MBanner;
using DMS.Services.MAppUser;
using DMS.Services.MImage;
using DMS.Services.MStatus;

namespace DMS.Rpc.banner
{
    public class BannerRoute : Root
    {
        public const string Master = Module + "/banner/banner-master";
        public const string Detail = Module + "/banner/banner-detail";
        private const string Default = Rpc + Module + "/banner";
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
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListStatus = Default + "/single-list-status";

        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(BannerFilter.Id), FieldType.ID },
            { nameof(BannerFilter.Code), FieldType.STRING },
            { nameof(BannerFilter.Title), FieldType.STRING },
            { nameof(BannerFilter.Priority), FieldType.LONG },
            { nameof(BannerFilter.Content), FieldType.STRING },
            { nameof(BannerFilter.CreatorId), FieldType.ID },
            { nameof(BannerFilter.ImageId), FieldType.ID },
            { nameof(BannerFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {Master,Detail,Count,List,Get, FilterListAppUser, FilterListStatus}},
            { "Thêm mới", new List<string> {Master, Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Create, SingleListAppUser, SingleListAppUser,}},
            { "Sửa", new List<string> { Master, Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Update, SingleListAppUser, SingleListAppUser}},
            { "Xoá", new List<string> { Master, Detail, Count, List, Get, FilterListAppUser, FilterListStatus, Detail, Delete, SingleListAppUser, SingleListAppUser}},
            { "Xoá nhiều", new List<string> {Master,Detail,Count,List,Get, BulkDelete, FilterListAppUser, FilterListStatus}},
            { "Xuất excel", new List<string> {Master,Detail,Count,List,Get, Export, FilterListAppUser, FilterListStatus}},
            { "Nhập excel", new List<string> {Master,Detail,Count,List,Get, ExportTemplate, Import, FilterListAppUser, FilterListStatus}},
        };
    }
}
