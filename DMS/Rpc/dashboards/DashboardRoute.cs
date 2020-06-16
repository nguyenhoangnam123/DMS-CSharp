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
        public const string StoreChecking = Default + "/store-checking";
        public const string IndirectSalesOrder = Default + "/indirect-sales-order";
        public const string ImageStoreCheking = Default + "/image-store-checking";
    }
}
