using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_store_and_item
{
    public class ReportSalesOrderByStoreAndItem_ItemDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitOfMeasureName { get; set; }
        public long SaleStock { get; set; }
        public long PromotionStock { get; set; }
        public decimal SalePriceAverage { get; set; }
        public decimal Discount { get; set; }
        public decimal Revenue { get; set; }
        public long IndirecSalesOrderCounter => IndirectSalesOrderIds?.Count() ?? 0;
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
    }
}
