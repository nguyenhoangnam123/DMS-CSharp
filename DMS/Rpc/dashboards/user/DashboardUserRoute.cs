﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUserRoute : Root
    {
        public const string Master = Module + "/dashboards/user";
        private const string Default = Rpc + Module + "/dashboards/user";
        public const string StoreChecking = Default + "/store-checking";
        public const string SaleEmployeeOnline = Default + "/sale-employee-online";
        public const string StatisticIndirectSalesOrder = Default + "/statistic-indirect-sales-order";
        public const string ImageStoreCheking = Default + "/image-store-checking";
        public const string StoreCoverage = Default + "/store-coverage";
        public const string SaleEmployeeLocation = Default + "/sale-employee-location";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string TopSaleEmployeeStoreChecking = Default + "/top-sale-employee-store-checking";

        public const string FilterListTime = Default + "/filter-list-time";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Hiển thị", new List<string> {
                Master,
                StoreChecking, SaleEmployeeOnline, StatisticIndirectSalesOrder, ImageStoreCheking, StoreCoverage, SaleEmployeeLocation,
                ListIndirectSalesOrder, TopSaleEmployeeStoreChecking, FilterListTime
            } },
        };
    }
}