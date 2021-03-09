using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirectorRoute : Root
    {
        public const string Parent = Module + "/dashboards";
        public const string Master = Module + "/dashboards/director";
        private const string Default = Rpc + Module + "/dashboards/director";
        public const string CountStore = Default + "/count-store";
        public const string CountIndirectSalesOrder = Default + "/indirect-sales-order-counter";
        public const string RevenueTotal = Default + "/revenue-total";
        public const string StoreHasCheckedCounter = Default + "/store-has-checked-counter";
        public const string CountStoreChecking = Default + "/store-checking-couter";
        public const string StatisticToday = Default + "/statistic-today";
        public const string StatisticYesterday = Default + "/statistic-yesterday";
        public const string StoreCoverage = Default + "/store-coverage";
        public const string SaleEmployeeLocation = Default + "/sale-employee-location";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string Top5RevenueByProduct = Default + "/top-5-revenue-by-product";
        public const string Top5RevenueByEmployee = Default + "/top-5-revenue-by-employee";
        public const string RevenueFluctuation = Default + "/revenue-fluctuation";
        public const string SaledItemFluctuation = Default + "/saled-item-fluctuation";
        public const string IndirectSalesOrderFluctuation = Default + "/indirect-sales-order-fluctuation";

        public const string FilterListTime1 = Default + "/filter-list-time-1";
        public const string FilterListTime2 = Default + "/filter-list-time-2";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {

            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
            { nameof(DashboardDirector_StoreFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Hiển thị", new List<string> {
                Parent,
                Master,
                CountStore, CountIndirectSalesOrder, RevenueTotal, StoreHasCheckedCounter, CountStoreChecking, StatisticToday, StatisticYesterday,
                StoreCoverage, SaleEmployeeLocation, ListIndirectSalesOrder, Top5RevenueByProduct, Top5RevenueByEmployee, RevenueFluctuation, SaledItemFluctuation,
                IndirectSalesOrderFluctuation, FilterListTime1, FilterListTime2, FilterListAppUser, FilterListOrganization,
            } },
        };
    }
}
