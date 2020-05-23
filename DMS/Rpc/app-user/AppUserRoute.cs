using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.app_user
{
    public class AppUserRoute : Root
    {
        public const string Master = Module + "/app-user/app-user-master";
        public const string Detail = Module + "/app-user/app-user-detail";
        private const string Default = Rpc + Module + "/app-user";
        public const string Mobile = Default + "/master-data.profile";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListSex = Default + "/single-list-sex";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListRole = Default + "/single-list-role";
        public const string CountRole = Default + "/count-role";
        public const string ListRole = Default + "/list-role";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(AppUserFilter.Username), FieldType.STRING },
            { nameof(AppUserFilter.Password), FieldType.STRING },
            { nameof(AppUserFilter.DisplayName), FieldType.STRING },
            { nameof(AppUserFilter.Address), FieldType.STRING },
            { nameof(AppUserFilter.Email), FieldType.STRING },
            { nameof(AppUserFilter.Phone), FieldType.STRING },
            { nameof(AppUserFilter.Birthday), FieldType.DATE },
            { nameof(AppUserFilter.Position), FieldType.STRING },
            { nameof(AppUserFilter.Department), FieldType.STRING },
            { nameof(AppUserFilter.OrganizationId), FieldType.ID },
            { nameof(AppUserFilter.SexId), FieldType.ID },
            { nameof(AppUserFilter.StatusId), FieldType.ID },
            { nameof(AppUserFilter.ProvinceId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {Master,Detail,Count,List,Get, SingleListOrganization, SingleListSex, SingleListStatus, SingleListRole, CountRole, ListRole}},
        };
    }
}
