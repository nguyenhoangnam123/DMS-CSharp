using Common;
using System.Collections.Generic;

namespace DMS.Rpc.organization
{
    public class OrganizationRoute : Root
    {
        public const string Master = Module + "/organization/organization-master";
        public const string Detail = Module + "/organization/organization-detail";
        private const string Default = Rpc + Module + "/organization";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListStatus, FilterListAppUser, } },
            { "Xuất excel", new List<string> { Master, Count, List, Get, FilterListOrganization, FilterListStatus, FilterListAppUser, Export } },
        };
    }
}
