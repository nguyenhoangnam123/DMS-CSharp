using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.role
{
    public class RoleRoute : Root
    {
        public const string Master = Module + "/role/role-master";
        public const string Detail = Module + "/role/role-detail";
        private const string Default = Rpc + Module + "/role";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string AssignAppUser = Default + "/assign-app-user";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-template";
        public const string GetMenu = Default + "/get-menu";
        public const string CreatePermission = Default + "/create-permission";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListMenu = Default + "/single-list-menu";

        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(RoleFilter.Code), FieldType.STRING },
            { nameof(RoleFilter.Name), FieldType.STRING },
            { nameof(RoleFilter.StatusId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { 
                Master, Count, List, Get} },
            { "Thêm", new List<string> { 
                Master, Count, List, Get, 
                Detail, Create, 
                SingleListStatus } },
            { "Sửa", new List<string> { 
                Master, Count, List, Get, 
                Detail, Update,
                SingleListAppUser, SingleListMenu, SingleListStatus } },
             { "Gán người dùng", new List<string> {
                Master, Count, List, Get,
                Detail, AssignAppUser,
                SingleListAppUser, SingleListMenu, SingleListStatus } },
             { "Tạo nhanh quyền", new List<string> {
                Master, Count, List, Get,
                Detail, CreatePermission,
                SingleListAppUser, SingleListMenu, SingleListStatus } },
            { "Xoá", new List<string> { 
                Master, Count, List, Get, 
                Detail, Delete, 
                SingleListStatus } },
            { "Xuất excel", new List<string> { 
                Master, Count, List, Get,
                Detail, Export,
                SingleListAppUser, SingleListMenu, SingleListStatus} },
            { "Nhập excel", new List<string> { 
                Master, Count, List, Get,
                Detail, ExportTemplate, Import,
                SingleListAppUser, SingleListMenu, SingleListStatus} },
        };
    }
}
