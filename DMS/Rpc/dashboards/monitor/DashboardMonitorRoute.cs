using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitorRoute : Root
    {
        private const string Default = Rpc + Module + "/dashboards/monitor";
        public const string StoreChecking = Default + "/store-checking";
        public const string SaleEmployeeOnline = Default + "/sale-employee-online";
        public const string StatisticIndirectSalesOrder = Default + "/statistic-indirect-sales-order";
        public const string ImageStoreCheking = Default + "/image-store-checking";
        public const string StoreCheckingCoverage = Default + "/store-checking-coverage";
        public const string SaleEmployeeLocation = Default + "/sale-employee-location";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string TopSaleEmployeeStoreChecking = Default + "/top-sale-employee-store-checking";

        public const string FilterListTime = Default + "/filter-list-time";
    }
}
