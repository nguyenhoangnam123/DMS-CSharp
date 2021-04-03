using DMS.ABE.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Rpc.direct_sales_order
{
    [DisplayName("Đơn hàng bán trực tiếp")]
    public class DirectSalesOrderRoute : Root
    {
        private const string Default = Rpc + Module + "/direct-sales-order";
        public const string List = Default + "/list";
        public const string Count = Default + "/count";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Get = Default + "/get";

        public const string DMSCreateRoute = "/rpc/dms/direct-sales-order/create";
        public const string DMSUpdateRoute = "/rpc/dms/direct-sales-order/update";

        public const string SingleListAppUser = Default + "/single-list-app-user"; // chọn nhân viên kinh doanh
        public const string ListAppUser = Default + "/list-app-user";
        public const string CountAppUser = Default + "/count-app-user";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                //SingleListCategory,
                //CountBanner, ListBanner, GetBanner, GetItem, CountProduct, ListProduct, GetProduct,
                //} 
            } },
        };
    }
}
