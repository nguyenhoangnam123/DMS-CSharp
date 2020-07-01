using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_SaleEmployeeItemDTO : DataDTO
    {
        public long ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string? OrganizationName { get; set; }
        public long OrganizationId { get; set; }
        public decimal IndirectOutputOfKeyItem { get; set; }
        public decimal IndirectOutputOfkeyItemPlanned { get; set; }
        public decimal IndirectOutputOfkeyItemRatio { get; set; }
        public decimal IndirectSalesOfKeyItem { get; set; }
        public decimal IndirectSalesOfKeyItemPlanned { get; set; }
        public decimal IndirectSalesOfKeyItemRatio { get; set; }
        public decimal IndirectOrdersOfKeyItem { get; set; }
        public decimal IndirectOrdersOfKeyItemPlanned { get; set; }
        public decimal IndirectOrdersOfKeyItemRatio { get; set; }
        public decimal IndirectStoresOfKeyItem { get; set; }
        public decimal IndirectStoresOfKeyItemPlanned { get; set; }
        public decimal IndirectStoresOfKeyItemRatio { get; set; }
        //public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM { get; set; }
        //public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM_PLANNED { get; set; }
        //public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM_RATIO { get; set; }
        //public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM { get; set; }
        //public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM_PLANNED { get; set; }
        //public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM_RATIO { get; set; }
        //public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM { get; set; }
        //public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM_PLANNED { get; set; }
        //public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM_RATIO { get; set; }
        //public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM { get; set; }
        //public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM_PLANNED { get; set; }
        //public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM_RATIO { get; set; }
    }
}
