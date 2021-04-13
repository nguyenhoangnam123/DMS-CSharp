using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.mobile
{
    public class DashboardMobile_TopRevenueByItemDTO : DataDTO
    {
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardMobile_TopRevenueByItemFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
