﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_item
{
    public class ReportSalesOrderByItemRoute : Root
    {
        public const string Master = Module + "/report-sales-order-by-item/report-sales-order-by-item-master";

        private const string Default = Rpc + Module + "/report-sales-order-by-item";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListItem = Default + "/filter-list-item";
        public const string FilterListProductType = Default + "/filter-list-product-type";
        public const string FilterListProductGrouping = Default + "/filter-list-product-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Total, Export, FilterListItem, FilterListProductType, FilterListProductGrouping  } },

        };
    }
}
