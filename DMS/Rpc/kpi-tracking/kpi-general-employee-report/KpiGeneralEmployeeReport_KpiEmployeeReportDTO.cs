using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string KpiPeriodName { get; set; }
        public long KpiPeriodId { get; set; }
        public string KpiYearName { get; set; }
        public long KpiYearId { get; set; }
        //Số đơn hàng gián tiếp
        public decimal TotalIndirectOrdersPLanned { get; set; }
        public decimal TotalIndirectOrders { get; set; }
        public decimal TotalIndirectOrdersRatio { get; set; }
        //Tổng sản lượng theo đơn hàng gián tiếp
        public decimal TotalIndirectQuantityPlanned { get; set; }
        public decimal TotalIndirectQuantity { get; set; }
        public decimal TotalIndirectQuantityRatio { get; set; }
        //Doanh thu theo đơn hàng gián tiếp
        public decimal TotalIndirectSalesAmountPlanned { get; set; }
        public decimal TotalIndirectSalesAmount { get; set; }
        public decimal TotalIndirectSalesAmountRatio { get; set; }

        //SKU
        public decimal SkuIndirectOrderPlanned { get; set; }
        internal HashSet<long> SKUItems { get; set; }
        public decimal SkuIndirectOrder { get; set; }
        public decimal SkuIndirectOrderRatio { get; set; }

        //Số cửa hàng viếng thăm
        public decimal StoresVisitedPLanned { get; set; }
        internal HashSet<long> StoreIds { get; set; }
        public decimal StoresVisited => StoreIds.Count;
        public decimal StoresVisitedRatio { get; set; }

        public decimal NewStoreCreatedPlanned { get; set; }
        public decimal NewStoreCreated { get; set; }
        public decimal NewStoreCreatedRatio { get; set; }

        public decimal NumberOfStoreVisits { get; set; }
        public decimal NumberOfStoreVisitsPlanned { get; set; }
        public decimal NumberOfStoreVisitsRatio { get; set; }
    }


    public class KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter KpiYearId { get; set; }
    }
}
