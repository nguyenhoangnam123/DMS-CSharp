using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_direct_sales_order_general
{
    public class ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportDirectSalesOrderGeneral_DirectSalesOrderDTO> SalesOrders { get; set; }
    }

    public class ReportDirectSalesOrderGeneral_ReportDirectSalesOrderGeneralFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public IdFilter StoreStatusId { get; set; }
        public DateFilter OrderDate { get; set; }
        internal bool HasValue => (OrganizationId != null && OrganizationId.HasValue) ||
            (AppUserId != null && AppUserId.HasValue) ||
            (BuyerStoreId != null && BuyerStoreId.HasValue) ||
            (StoreStatusId != null && StoreStatusId.HasValue) ||
            (OrderDate != null && OrderDate.HasValue);
    }
}
