using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreCheckerRoute : Root
    {
        public const string Master = Module + "/monitor-store-checker/monitor-store-checker-master";

        private const string Default = Rpc + Module + "/monitor-store-checker";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListChecking = Default + "/filter-list-checking";
        public const string FilterListImage = Default + "/filter-list-image";
        public const string FilterListSalesOrder = Default + "/filter-list-sales-order";

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, Export, FilterListOrganization,FilterListAppUser,FilterListChecking, FilterListImage,FilterListSalesOrder  } },

        };
    }
}
