﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_general
{
    public class ReportSalesOrderGeneral_IndirectSalesOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string BuyerStoreName { get; set; }
        public string SellerStoreName { get; set; }
        public string SaleEmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxValue { get; set; }
        public decimal Total { get; set; }
    }
}