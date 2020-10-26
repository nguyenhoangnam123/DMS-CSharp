using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_general
{
    public class ReportSalesOrderGeneral_TotalDTO : DataDTO
    {
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
