using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.album
{
    public class AlbumRoute : Root
    {
        public const string Master = Module + "/album/album-master";
        public const string Detail = Module + "/album/album-detail";
        private const string Default = Rpc + Module + "/album";
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

        public const string FilterListStatus = Default + "/filter-list-status";
        public const string SingleListStatus = Default + "/single-list-status";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(AlbumFilter.Name), FieldType.STRING },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { Master, Count, List, Get, FilterListStatus, } },
            { "Thêm", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Create,  SingleListStatus, } },
            { "Sửa", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Update,  SingleListStatus, } },
            { "Xoá", new List<string> { Master, Count, List, Get,  FilterListStatus, Detail, Delete,  } },
            { "Xoá nhiều", new List<string> { Master, Count, List, Get, FilterListStatus, BulkDelete } },
            { "Xuất excel", new List<string> { Master, Count, List, Get, FilterListStatus, Export } },
            { "Nhập excel", new List<string> { Master, Count, List, Get, FilterListStatus, ExportTemplate, Import } },
        };
    }
}
