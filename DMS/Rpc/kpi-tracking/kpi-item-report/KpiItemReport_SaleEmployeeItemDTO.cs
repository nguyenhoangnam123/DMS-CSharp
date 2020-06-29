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
        public decimal INDIRECT_OUTPUT_OF_KEY_ITEM { get; set; }
        public decimal INDIRECT_OUTPUT_OF_KEY_ITEM_PlANNED { get; set; }
        public decimal INDIRECT_OUTPUT_OF_KEY_ITEM_RATIO { get; set; }
        public decimal INDIRECT_SALES_OF_KEY_ITEM { get; set; }
        public decimal INDIRECT_SALES_OF_KEY_ITEM_PLANNED { get; set; }
        public decimal INDIRECT_SALES_OF_KEY_ITEM_RATIO { get; set; }
        public decimal INDIRECT_ORDERS_OF_KEY_ITEM { get; set; }
        public decimal INDIRECT_ORDERS_OF_KEY_ITEM_PLANNED { get; set; }
        public decimal INDIRECT_ORDERS_OF_KEY_ITEM_RATIO { get; set; }
        public decimal INDIRECT_STORES_OF_KEY_ITEM { get; set; }
        public decimal INDIRECT_STORES_OF_KEY_ITEM_PLANNED { get; set; }
        public decimal INDIRECT_STORES_OF_KEY_ITEM_RATIO { get; set; }
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
