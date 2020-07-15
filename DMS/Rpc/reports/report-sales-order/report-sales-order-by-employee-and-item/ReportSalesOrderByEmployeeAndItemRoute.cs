using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_employee_and_item
{
    public class ReportSalesOrderByEmployeeAndItemRoute : Root
    {
        public const string Master = Module + "/report-sales-order-by-employee-and-items/report-sales-order-by-employee-and-item-master";

        private const string Default = Rpc + Module + "/report-sales-order-by-employee-and-item";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportSalesOrderByEmployeeAndItem_ReportSalesOrderByEmployeeAndItemFilterDTO.SaleEmployeeId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Total, Export, FilterListOrganization, FilterListAppUser } },

        };
    }
}
