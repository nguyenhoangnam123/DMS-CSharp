using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportSalesOrderByEmployeeAndItem_SaleEmployeeDTO> SaleEmployees { get; set; }
    }

    public class ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter OrderDate { get; set; }
    }
}
