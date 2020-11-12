using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobileRoute : Root
    {
        public const string Parent = Module + "/dashboards";
        public const string Master = Module + "/dashboards/mobile";
        private const string Default = Rpc + Module + "/dashboards/mobile";

        public const string CountIndirectSalesOrder = Default + "/count-indirect-sales-order";
        public const string CountStoreChecking = Default + "/count-store-checking";
        public const string Revenue = Default + "/revenue";
        public const string SalesQuantity = Default + "/sales-quantity";
        public const string KpiGeneral = Default + "/kpi-general";
        public const string SingleListPeriod = Default + "/single-list-period";
    }
}
