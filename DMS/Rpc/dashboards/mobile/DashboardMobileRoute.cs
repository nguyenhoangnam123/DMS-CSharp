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

        public const string CountStoreChecking = Default + "/count-store-checking";
        public const string SalesQuantity = Default + "/sales-quantity";
        public const string KpiGeneral = Default + "/kpi-general";

        public const string CountIndirectSalesOrder = Default + "/count-indirect-sales-order"; // đơn gián tiếp
        public const string IndirectSalesOrderRevenue = Default + "/indirect-sales-order-revenue"; // doanh thu đơn gián tiếp

        public const string CountDirectSalesOrder = Default + "/count-direct-sales-order"; // đơn trục tiếp
        public const string DirectSalesOrderRevenue = Default + "/direct-sales-order-revenue"; // doanh thu đơn trực tiếp

        public const string TopIndirectSaleEmployeeRevenue = Default + "/top-indirect-sale-employee-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên
        public const string TopIndirecItemRevenue = Default + "/top-indirect-item-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên

        public const string TopDirectSaleEmployeeRevenue = Default + "/top-direct-sale-employee-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên
        public const string TopDirecItemRevenue = Default + "/top-direct-item-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên

        public const string IndirectRevenueGrowth = Default + "/indirect-revenue-growth"; // bieu do tang truong doanh thu gian tiep
        public const string IndirectQuantityGrowth = Default + "/indirect-quantity-growth"; // bieu do tang truong so luong gian tiep

        public const string DirectRevenueGrowth = Default + "/direct-revenue-growth"; // bieu do tang truong doanh thu gian tiep
        public const string DirectQuantityGrowth = Default + "/direct-quantity-growth"; // bieu do tang truong so luong gian tiep

        public const string SingleListPeriod = Default + "/single-list-period";
    }
}
