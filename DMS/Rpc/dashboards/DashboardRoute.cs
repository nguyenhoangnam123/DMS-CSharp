using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class DashboardRoute : Root
    {
        private const string Default = Rpc + Module + "/dashboards";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string StoreChecking = Default + "/store-checking";
        public const string StoreCheckingCoverage = Default + "/store-checking-coverage";
        public const string StatisticIndirectSalesOrder = Default + "/statistic-indirect-sales-order";
        public const string ImageStoreCheking = Default + "/image-store-checking";
        public const string FilterListTime = Default + "/filter-list-time";
        public const string TopSaleEmployeeStoreChecking = Default + "/top-sale-employee-store-checking";
    }
}
