using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_general
{
    public class ReportSalesOrderGeneral_ReportSalesOrderGeneralDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportSalesOrderGeneral_IndirectSalesOrderDTO> IndirectSalesOrders { get; set; }
    }

    public class ReportSalesOrderGeneral_ReportSalesOrderGeneralFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public IdFilter SellerStoreId { get; set; }
        public DateFilter OrderDate { get; set; }
        internal bool HasValue => OrganizationId.HasValue || SaleEmployeeId.HasValue || BuyerStoreId.HasValue || SellerStoreId.HasValue || OrderDate.HasValue;
    }
}
