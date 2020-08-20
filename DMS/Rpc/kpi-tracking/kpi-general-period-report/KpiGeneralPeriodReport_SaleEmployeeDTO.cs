using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiGeneralPeriodReport_SaleEmployeeDTO : DataDTO
    {
        public long STT { get; set; }
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public long? OrganizationId { get; set; }

        //Số đơn hàng gián tiếp
        public decimal? TotalIndirectOrdersPLanned { get; set; }
        public decimal? TotalIndirectOrders { get; set; }
        public decimal? TotalIndirectOrdersRatio { get; set; }

        //Tổng sản lượng theo đơn hàng gián tiếp
        public decimal? TotalIndirectQuantityPlanned { get; set; }
        public decimal? TotalIndirectQuantity { get; set; }
        public decimal? TotalIndirectQuantityRatio { get; set; }

        //Doanh thu theo đơn hàng gián tiếp
        public decimal? TotalIndirectSalesAmountPlanned { get; set; }
        public decimal? TotalIndirectSalesAmount { get; set; }
        public decimal? TotalIndirectSalesAmountRatio { get; set; }

        //SKU
        public decimal? SkuIndirectOrderPlanned { get; set; }
        internal HashSet<long> SKUItems { get; set; }
        public decimal? SkuIndirectOrder { get; set; }
        public decimal? SkuIndirectOrderRatio { get; set; }

        //Số cửa hàng viếng thăm
        public decimal? StoresVisitedPLanned { get; set; }
        internal HashSet<long> StoreIds { get; set; }
        public decimal? StoresVisited
        {
            get
            {
                if (StoreIds == null) return null;
                return StoreIds.Count;
            }
        }
        public decimal? StoresVisitedRatio { get; set; }

        //Số cửa hàng tạo mới
        public decimal? NewStoreCreatedPlanned { get; set; }
        public decimal? NewStoreCreated { get; set; }
        public decimal? NewStoreCreatedRatio { get; set; }

        //Số lần viếng thăm
        public decimal? NumberOfStoreVisitsPlanned { get; set; }
        public decimal? NumberOfStoreVisits { get; set; }
        public decimal? NumberOfStoreVisitsRatio { get; set; }
    }
}
