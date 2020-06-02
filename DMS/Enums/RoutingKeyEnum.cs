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
        public static GenericEnum StorenSync = new GenericEnum { Id = 3, Code = "Store.Sync", Name = "Đồng bộ Store" };
        public static GenericEnum SendMail = new GenericEnum { Id = 4, Code = "Mail.Send", Name = "Gửi Mail" };
        public static GenericEnum AuditLog = new GenericEnum { Id = 5, Code = "AuditLog", Name = "Audit Log" };
        public static GenericEnum SystemLog = new GenericEnum { Id = 6, Code = "SystemLog", Name = "System Log" };

        public static List<GenericEnum> RoutingKeyEnumList = new List<GenericEnum>()
        {
            AppUserSync, OrganizationSync, StorenSync, SendMail, AuditLog, SystemLog
        };
    }
}
