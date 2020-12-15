using DMS.Common;
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
            this.TotalIndirectOrdersPLanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersPLanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersPLanned).ToString();
            this.TotalIndirectOrders = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrders == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrders.ToString();
            this.TotalIndirectOrdersRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectOrdersRatio.ToString();
            this.TotalIndirectQuantityPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityPlanned).ToString();
            this.TotalIndirectQuantity = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantity == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantity.ToString();
            this.TotalIndirectQuantityRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectQuantityRatio.ToString();
            this.TotalIndirectSalesAmountPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountPlanned).ToString();
            this.TotalIndirectSalesAmount = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmount == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmount.ToString();
            this.TotalIndirectSalesAmountRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalIndirectSalesAmountRatio.ToString();
            this.SkuIndirectOrderPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderPlanned == null ? "" : (Math.Round(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderPlanned.Value, 1)).ToString();
            this.SkuIndirectOrder = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrder == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrder.ToString();
            this.SkuIndirectOrderRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuIndirectOrderRatio.ToString();
            this.StoresVisitedPLanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedPLanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedPLanned).ToString();
            this.StoresVisited = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisited == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisited.ToString();
            this.StoresVisitedRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.StoresVisitedRatio.ToString();
            this.NewStoreCreatedPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedPlanned).ToString();
            this.NewStoreCreated = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreated == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreated.ToString();
            this.NewStoreCreatedRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NewStoreCreatedRatio.ToString();
            this.NumberOfStoreVisitsPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsPlanned).ToString();
            this.NumberOfStoreVisits = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisits == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisits.ToString();
            this.NumberOfStoreVisitsRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.NumberOfStoreVisitsRatio.ToString();
            this.TotalDirectOrdersPLanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrdersPLanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrdersPLanned).ToString();
            this.TotalDirectOrders = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrders == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrders.ToString();
            this.TotalDirectOrdersRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrdersRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectOrdersRatio.ToString();
            this.TotalDirectQuantityPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantityPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantityPlanned).ToString();
            this.TotalDirectQuantity = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantity == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantity.ToString();
            this.TotalDirectQuantityRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantityRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectQuantityRatio.ToString();
            this.TotalDirectSalesAmountPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmountPlanned == null ? "" : ((long)KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmountPlanned).ToString();
            this.TotalDirectSalesAmount = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmount == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmount.ToString();
            this.TotalDirectSalesAmountRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmountRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.TotalDirectSalesAmountRatio.ToString();
            this.SkuDirectOrderPlanned = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrderPlanned == null ? "" : (Math.Round(KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrderPlanned.Value, 1)).ToString();
            this.SkuDirectOrder = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrder == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrder.ToString();
            this.SkuDirectOrderRatio = KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrderRatio == null ? "" : KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO.SkuDirectOrderRatio.ToString();
        }
    }
}
