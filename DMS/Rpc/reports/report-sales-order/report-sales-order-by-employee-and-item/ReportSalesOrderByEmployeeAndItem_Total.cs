﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItem_TotalDTO : DataDTO
    {
        public long TotalSalesStock { get; set; }
        public long TotalPromotionStock { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
