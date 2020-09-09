using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReport_ExportDTO : DataDTO
    {
        public long STT { get; set; }
        public string KpiPeriod { get; set; }
        public string KpiYear { get; set; }

        //Số đơn hàng gián tiếp
        public string TotalIndirectOrdersPLanned { get; set; }
        public string TotalIndirectOrders { get; set; }
        public string TotalIndirectOrdersRatio { get; set; }

        //Tổng sản lượng theo đơn hàng gián tiếp
        public string TotalIndirectQuantityPlanned { get; set; }
        public string TotalIndirectQuantity { get; set; }
        public string TotalIndirectQuantityRatio { get; set; }

        //Doanh thu theo đơn hàng gián tiếp
        public string TotalIndirectSalesAmountPlanned { get; set; }
        public string TotalIndirectSalesAmount { get; set; }
        public string TotalIndirectSalesAmountRatio { get; set; }

        //SKU gián tiếp
        public string SkuIndirectOrderPlanned { get; set; }
        public string SkuIndirectOrder { get; set; }
        public string SkuIndirectOrderRatio { get; set; }

        //Số đơn hàng trực tiếp
        public string TotalDirectOrdersPLanned { get; set; }
        public string TotalDirectOrders { get; set; }
        public string TotalDirectOrdersRatio { get; set; }

        //Tổng sản lượng theo đơn hàng trực tiếp
        public string TotalDirectQuantityPlanned { get; set; }
        public string TotalDirectQuantity { get; set; }
        public string TotalDirectQuantityRatio { get; set; }

        //Doanh thu theo đơn hàng trực tiếp
        public string TotalDirectSalesAmountPlanned { get; set; }
        public string TotalDirectSalesAmount { get; set; }
        public string TotalDirectSalesAmountRatio { get; set; }

        //SKU trực tiếp
        public string SkuDirectOrderPlanned { get; set; }
        public string SkuDirectOrder { get; set; }
        public string SkuDirectOrderRatio { get; set; }

        //Số cửa hàng viếng thăm
        public string StoresVisitedPLanned { get; set; }
        public string StoresVisited { get; set; }
        public string StoresVisitedRatio { get; set; }

        //Số cửa hàng tạo mới
        public string NewStoreCreatedPlanned { get; set; }
        public string NewStoreCreated { get; set; }
        public string NewStoreCreatedRatio { get; set; }

        //Số lần viếng thăm
        public string NumberOfStoreVisitsPlanned { get; set; }
        public string NumberOfStoreVisits { get; set; }
        public string NumberOfStoreVisitsRatio { get; set; }

        public KpiGeneralEmployeeReport_ExportDTO(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO)
        {
            this.STT = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.STT;
            this.KpiPeriod = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiPeriodName;
            this.KpiYear = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.KpiYearName;
            this.TotalIndirectOrdersPLanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersPLanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersPLanned.ToString();
            this.TotalIndirectOrders = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrders == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrders.ToString();
            this.TotalIndirectOrdersRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersRatio.ToString();
            this.TotalIndirectQuantityPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityPlanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityPlanned.ToString();
            this.TotalIndirectQuantity = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantity == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantity.ToString();
            this.TotalIndirectQuantityRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityRatio.ToString();
            this.TotalIndirectSalesAmountPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountPlanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountPlanned.ToString();
            this.TotalIndirectSalesAmount = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmount == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmount.ToString();
            this.TotalIndirectSalesAmountRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountRatio.ToString();
            this.SkuIndirectOrderPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderPlanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderPlanned.ToString();
            this.SkuIndirectOrder = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrder == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrder.ToString();
            this.SkuIndirectOrderRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderRatio.ToString();
            this.StoresVisitedPLanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedPLanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedPLanned.ToString();
            this.StoresVisited = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisited == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisited.ToString();
            this.StoresVisitedRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedRatio.ToString();
            this.NewStoreCreatedPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedPlanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedPlanned.ToString();
            this.NewStoreCreated = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreated == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreated.ToString();
            this.NewStoreCreatedRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedRatio.ToString();
            this.NumberOfStoreVisitsPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsPlanned == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsPlanned.ToString();
            this.NumberOfStoreVisits = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisits == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisits.ToString();
            this.NumberOfStoreVisitsRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsRatio.ToString();
        }
    }
}
