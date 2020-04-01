using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class RoutingKeyEnum
    {
        public static GenericEnum AppUserSync = new GenericEnum { Id = 1, Code = "AppUser.Sync", Name = "Đồng bộ AppUser" };
        public static GenericEnum OrganizationSync = new GenericEnum { Id = 2, Code = "Organization.Sync", Name = "Đồng bộ Organization" };
    }
}
