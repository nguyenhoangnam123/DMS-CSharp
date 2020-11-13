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
using DMS.Services.MCategory;
using DMS.Services.MImage;
using DMS.Services.MStatus;

namespace DMS.Rpc.category
{
    public class CategoryRoute : Root
    {
        public const string Master = Module + "/product-category/category/category-master";
        public const string Detail = Module + "/product-category/category/category-detail/*";
        private const string Default = Rpc + Module + "/category";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string GetPreview = Default + "/get-preview";
        public const string Get = Default + "/get";

        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListCategory = Default + "/filter-list-category";
        public const string FilterListStatus = Default + "/filter-list-status";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CategoryFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.Code), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.Name), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.ParentId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.Path), FieldTypeEnum.STRING.Id },
            { nameof(CategoryFilter.Level), FieldTypeEnum.LONG.Id },
            { nameof(CategoryFilter.StatusId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.ImageId), FieldTypeEnum.ID.Id },
            { nameof(CategoryFilter.RowId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List,
                Get, GetPreview }}
        };
    }
}
