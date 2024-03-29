﻿using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_general
{
    public class ReportDirectSalesOrderGeneral_DirectSalesOrderDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public string Code { get; set; }
        public string BuyerStoreName { get; set; }
        public string BuyerStoreStatusName { get; set; }
        public string SaleEmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public string eOrderDate { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxValue { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal PromotionValue { get; set; }
        public decimal TotalAfterPromotion { get; set; }

    }
}
