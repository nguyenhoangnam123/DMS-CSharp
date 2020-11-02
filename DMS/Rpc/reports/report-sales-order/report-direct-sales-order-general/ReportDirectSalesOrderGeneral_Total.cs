using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_general
{
    public class ReportDirectSalesOrderGeneral_TotalDTO : DataDTO
    {
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAfterTax { get; set; }
        public decimal PromotionValue { get; set; }
        public decimal Total { get; set; }
    }
}
