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
using DMS.Services.MOrganization;
using DMS.Services.MStatus;
using DMS.Services.MAppUser;
using DMS.Services.MProvince;
using DMS.Services.MSex;
using DMS.Services.MStore;
using DMS.Services.MDistrict;
using DMS.Services.MReseller;
using DMS.Services.MStoreGrouping;
using DMS.Services.MStoreType;
using DMS.Services.MWard;

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
