using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_sales_order.report_sales_order_by_store_and_item
{
    public class ReportSalesOrderByStoreAndItemRoute : Root
    {
        public const string Parent = Module + "/sales-order-report";
        public const string Master = Module + "/sales-order-report/sales-order-by-store-and-items-report-master";

        private const string Default = Rpc + Module + "/sales-order-by-store-and-items-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreType = Default + "/filter-list-store-type";
        public const string FilterListStoreGrouping = Default + "/filter-list-store-grouping";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreId), FieldTypeEnum.ID.Id },
            { nameof(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreGroupingId), FieldTypeEnum.ID.Id },
            { nameof(ReportSalesOrderByStoreAndItem_ReportSalesOrderByStoreAndItemFilterDTO.StoreTypeId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Total, Export, 
                FilterListOrganization, FilterListStore, FilterListStoreType, FilterListStoreGrouping  } },

        };
    }
}
