using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.monitor.monitor_salesman
{
    public class MonitorSalesmanRoute : Root
    {
        public const string Parent = Module + "/monitor";
        public const string Master = Module + "/monitor/monitor-salesman/monitor-salesman-master";

        private const string Default = Rpc + Module + "/monitor-salesman";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string ListImage = Default + "/list-image";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";
        public const string ExportUnchecking = Default + "/export-unchecking";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(MonitorSalesman_MonitorSalesmanFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(MonitorSalesman_MonitorSalesmanFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, ListImage, Get, Export, ExportUnchecking,
                FilterListOrganization, FilterListAppUser, } },
        };
    }
}
