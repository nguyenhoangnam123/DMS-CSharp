using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUserRoute
    {
        private const string Default = "rpc/dms/dashboards/user";

        public const string SalesQuantity = Default + "/sales-quantity";
        public const string StoreChecking = Default + "/store-checking";
        public const string Revenue = Default + "/revenue";
        public const string StatisticIndirectSalesOrder = Default + "/statistic-indirect-sales-order";

        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string ListComment = Default + "/list-comment";

        public const string FilterListTime = Default + "/filter-list-time";
    }
}
