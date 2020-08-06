using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUserRoute : Root
    {
        public const string Parent = Module + "/dashboards";
        public const string Master = Module + "/dashboards/user";
        private const string Default = Rpc + Module + "/dashboards/user";

        public const string SalesQuantity = Default + "/sales-quantity";
        public const string StoreChecking = Default + "/store-checking";
        public const string Revenue = Default + "/revenue";
        public const string StatisticIndirectSalesOrder = Default + "/statistic-indirect-sales-order";

        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string ListComment = Default + "/list-comment";

        public const string FilterListTime = Default + "/filter-list-time";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Hiển thị", new List<string> {
                Parent,
                Master,
                SalesQuantity, StoreChecking, Revenue, StatisticIndirectSalesOrder, ListIndirectSalesOrder, ListComment,
                FilterListTime
            } },
        };
    }
}
