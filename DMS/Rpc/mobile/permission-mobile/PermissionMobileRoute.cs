using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile.permission_mobile
{
    public class PermissionMobileRoute : Root
    {
        private const string Default = Rpc + Module + "/permission-mobile";

        // dashboard kpi
        public const string CountAppUser = Default + "/count-app-user";
        public const string ListAppUser = Default + "/list-app-user";

        public const string ListCurrentKpiGeneral = Default + "/list-current-kpi-general";
        public const string ListCurrentKpiItem = Default + "/list-current-kpi-item";
        public const string ListCurrentKpiNewItem = Default + "/list-current-kpi-new-item";

        public const string SalesQuantity = Default + "/sales-quantity";
        public const string KpiGeneral = Default + "/kpi-general";


        // dashboard order
        public const string CountStoreChecking = Default + "/count-store-checking"; // số lượt viếng thăm 
        public const string CountStore = Default + "/count-store"; // số đại lý viếng thăm

        public const string CountIndirectSalesOrder = Default + "/count-indirect-sales-order"; // đơn gián tiếp
        public const string IndirectSalesOrderRevenue = Default + "/indirect-sales-order-revenue"; // doanh thu đơn gián tiếp

        public const string CountDirectSalesOrder = Default + "/count-direct-sales-order"; // đơn trục tiếp
        public const string DirectSalesOrderRevenue = Default + "/direct-sales-order-revenue"; // doanh thu đơn trực tiếp

        public const string TopIndirectSaleEmployeeRevenue = Default + "/top-indirect-sale-employee-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên
        public const string TopIndirecProductRevenue = Default + "/top-indirect-product-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên

        public const string TopDirectSaleEmployeeRevenue = Default + "/top-direct-sale-employee-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên
        public const string TopDirecProductRevenue = Default + "/top-direct-product-revenue"; // top 5 doanh thu đơn gián tiếp theo nhân viên

        public const string IndirectRevenueGrowth = Default + "/indirect-revenue-growth"; // bieu do tang truong doanh thu gian tiep
        public const string IndirectQuantityGrowth = Default + "/indirect-quantity-growth"; // bieu do tang truong so luong gian tiep

        public const string DirectRevenueGrowth = Default + "/direct-revenue-growth"; // bieu do tang truong doanh thu gian tiep
        public const string DirectQuantityGrowth = Default + "/direct-quantity-growth"; // bieu do tang truong so luong gian tiep

        public const string SingleListPeriod = Default + "/single-list-period";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { "OrganizationId", FieldTypeEnum.ID.Id },
            { "AppUserId", FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Thống kê Kpi nhân viên theo tháng", new List<string>{
                ListCurrentKpiGeneral, ListCurrentKpiItem, ListCurrentKpiNewItem, CountAppUser, ListAppUser,
            } },
            { "Quyền Dashboard nhân viên theo đơn gián tiếp", new List<string> {
                CountStoreChecking, CountStore, CountIndirectSalesOrder, IndirectSalesOrderRevenue,
                SingleListPeriod,
            } },
            { "Quyền Dashboard nhân viên theo đơn trực tiếp", new List<string> {
                CountStoreChecking, CountStore, CountDirectSalesOrder, DirectSalesOrderRevenue,
                SingleListPeriod,
            } },
            { "Quyền Dashboard quản lý theo đơn gián tiếp", new List<string> {
                CountStoreChecking, CountStore, CountIndirectSalesOrder, IndirectSalesOrderRevenue,
                TopIndirectSaleEmployeeRevenue, TopIndirecProductRevenue,
                IndirectRevenueGrowth, IndirectQuantityGrowth,
                SingleListPeriod,
            } },
            { "Quyền Dashboard quản lý theo đơn trực tiếp", new List<string> {
                CountStoreChecking, CountStore, CountDirectSalesOrder, DirectSalesOrderRevenue,
                TopDirectSaleEmployeeRevenue, TopDirecProductRevenue,
                DirectRevenueGrowth, DirectQuantityGrowth,
                SingleListPeriod,
            } },
        };
    }
}
