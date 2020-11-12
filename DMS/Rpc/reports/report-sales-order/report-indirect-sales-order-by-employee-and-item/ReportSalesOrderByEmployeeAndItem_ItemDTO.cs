using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_indirect_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItem_ItemDTO : DataDTO
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
        public long BuyerStoreCounter => StoreIds?.Count() ?? 0;
        public long SalesOrderCounter => IndirectSalesOrderIds?.Count() ?? 0;
        internal HashSet<long> StoreIds { get; set; }
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
    }
}
