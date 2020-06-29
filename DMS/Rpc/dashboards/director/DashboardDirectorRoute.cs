using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirectorRoute : Root
    {
        private const string Default = Rpc + Module + "/dashboards/director";
        public const string IndirectSalesOrderCounter = Default + "/indirect-sales-order-counter";
        public const string RevenueTotal = Default + "/revenue-total";
        public const string ItemSalesTotal = Default + "/item-sales-total";
        public const string StoreCheckingCounter = Default + "/store-checking-couter";
    }
}
