using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public class PermissionMobileRoute : Root
    {
        private const string Default = Rpc + Module + "/permission-mobile";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";

        public const string ListCurrentKpiGeneral = Default + "/list-current-kpi-general";
        public const string ListCurrentKpiItem = Default + "/list-current-kpi-item";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "OrganizationId", FieldTypeEnum.ID.Id },
            { "AppUserId", FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Thống kê Kpi nhân viên theo tháng", new List<string>{
                ListCurrentKpiGeneral, ListCurrentKpiItem, FilterListAppUser
            } }
        };
    }
}
