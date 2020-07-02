using Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReport_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string KpiPeriodName { get; set; }
        public long KpiPeriodId { get; set; }
        public string? OrganizationName { get; set; }
        public long? OrganizationId { get; set; }
        public decimal TotalIndirectOrders { get; set; }
        public decimal TotalIndirectOrdersPLanned { get; set; }
        public decimal TotalIndirectOrdersRatio { get; set; }
        public decimal TotalIndirectOutput { get; set; }
        public decimal TotalIndirectOutputPlanned { get; set; }
        public decimal TotalIndirectOutputRatio { get; set; }
        public decimal TotalIndirectSalesAmount { get; set; }
        public decimal TotalIndirectSalesAmountPlanned { get; set; }
        public decimal TotalIndirectSalesAmountRatio { get; set; }
        public decimal SkuIndirectOrder { get; set; }
        public decimal SkuIndirectOrderPlanned { get; set; }
        public decimal SkuIndirectOrderRatio { get; set; }
        public decimal StoresVisited { get; set; }
        public decimal StoresVisitedPLanned { get; set; }
        public decimal StoresVisitedRatio { get; set; }
        public decimal NewStoreCreated { get; set; }
        public decimal NewStoreCreatedPlanned { get; set; }
        public decimal NewStoreCreatedRatio { get; set; }
        public decimal NumberOfStoreVisits { get; set; }
        public decimal NumberOfStoreVisitsPlanned { get; set; }
        public decimal NumberOfStoreVisitsRatio { get; set; }
        public KpiGeneralEmployeeReport_SaleEmployeeDTO()
        {

        }
        //public List<MonitorStoreChecker_StoreCheckingDTO> StoreCheckings { get; set; }
    }
}
