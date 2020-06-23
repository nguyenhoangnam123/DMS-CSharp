﻿using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_SaleEmployeeItemDTO : DataDTO
    {
        public long ItemId { get; set; }
        public long SaleEmployeeId { get; set; }
        public long INDIRECT_OUTPUT_OF_KEY_ITEM { get; set; }
        public long INDIRECT_OUTPUT_OF_KEY_ITEM_PlANNED { get; set; }
        public long INDIRECT_OUTPUT_OF_KEY_ITEM_RATIO { get; set; }
        public long INDIRECT_SALES_OF_KEY_ITEM { get; set; }
        public long INDIRECT_SALES_OF_KEY_ITEM_PLANNED { get; set; }
        public long INDIRECT_SALES_OF_KEY_ITEM_RATIO { get; set; }
        public long INDIRECT_ORDERS_OF_KEY_ITEM { get; set; }
        public long INDIRECT_ORDERS_OF_KEY_ITEM_PLANNED { get; set; }
        public long INDIRECT_ORDERS_OF_KEY_ITEM_RATIO { get; set; }
        public long INDIRECT_STORES_OF_KEY_ITEM { get; set; }
        public long INDIRECT_STORES_OF_KEY_ITEM_PLANNED { get; set; }
        public long INDIRECT_STORES_OF_KEY_ITEM_RATIO { get; set; }
        public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM { get; set; }
        public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM_PLANNED { get; set; }
        public long TOTAL_INDIRECT_OUTPUT_OF_KEY_ITEM_RATIO { get; set; }
        public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM { get; set; }
        public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM_PLANNED { get; set; }
        public long TOTAL_INDIRECT_SALES_OF_KEY_ITEM_RATIO { get; set; }
        public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM { get; set; }
        public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM_PLANNED { get; set; }
        public long TOTAL_INDIRECT_ORDERS_OF_KEY_ITEM_RATIO { get; set; }
        public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM { get; set; }
        public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM_PLANNED { get; set; }
        public long TOTAL_INDIRECT_STORES_OF_KEY_ITEM_RATIO { get; set; }
    }
}