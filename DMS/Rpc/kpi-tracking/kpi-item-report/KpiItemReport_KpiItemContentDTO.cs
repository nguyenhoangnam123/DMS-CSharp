using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_KpiItemContentDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public long ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        //Sản lượng theo đơn hàng gián tiếp
        public decimal IndirectQuantityPlanned { get; set; }
        public decimal IndirectQuantity { get; set; }
        public decimal IndirectQuantityRatio { get; set; }

        //Doanh số theo đơn hàng gián tiếp
        public decimal IndirectRevenuePlanned { get; set; }
        public decimal IndirectRevenue { get; set; }
        public decimal IndirectRevenueRatio { get; set; }

        //Số đơn hàng gián tiếp
        public decimal IndirectAmountPlanned { get; set; }
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
        public decimal IndirectAmount => IndirectSalesOrderIds.Count;
        public decimal IndirectAmountRatio { get; set; }

        //Số đại lý theo đơn gián tiếp
        public decimal IndirectStorePlanned { get; set; }
        internal HashSet<long> StoreIds { get; set; }
        public decimal IndirectStore => StoreIds.Count;
        public decimal IndirectStoreRatio { get; set; }
    }
}
