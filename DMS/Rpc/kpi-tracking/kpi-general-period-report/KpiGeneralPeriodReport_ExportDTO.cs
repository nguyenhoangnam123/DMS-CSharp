﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiGeneralPeriodReport_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiGeneralPeriodReport_LineDTO> Lines { get; set; }
        public KpiGeneralPeriodReport_ExportDTO(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO)
        {
            this.OrganizationName = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.OrganizationName;
            this.Lines = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees?.Select(x => new KpiGeneralPeriodReport_LineDTO(x)).ToList();
        }
    }

    public class KpiGeneralPeriodReport_LineDTO : DataDTO
    {
        public long STT { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }

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

        //SKU
        public string SkuIndirectOrderPlanned { get; set; }
        public string SkuIndirectOrder { get; set; }
        public string SkuIndirectOrderRatio { get; set; }

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

        public KpiGeneralPeriodReport_LineDTO(KpiGeneralPeriodReport_SaleEmployeeDTO KpiGeneralPeriodReport_SaleEmployeeDTO)
        {
            this.STT = KpiGeneralPeriodReport_SaleEmployeeDTO.STT;
            this.Username = KpiGeneralPeriodReport_SaleEmployeeDTO.Username;
            this.DisplayName = KpiGeneralPeriodReport_SaleEmployeeDTO.DisplayName;
            this.OrganizationName = KpiGeneralPeriodReport_SaleEmployeeDTO.OrganizationName;
            this.TotalIndirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned.ToString();
            this.TotalIndirectOrders = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrders == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrders.ToString();
            this.TotalIndirectOrdersRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersRatio.ToString();
            this.TotalIndirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityPlanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityPlanned.ToString();
            this.TotalIndirectQuantity = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantity == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantity.ToString();
            this.TotalIndirectQuantityRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityRatio.ToString();
            this.TotalIndirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned.ToString();
            this.TotalIndirectSalesAmount = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmount == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmount.ToString();
            this.TotalIndirectSalesAmountRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountRatio.ToString();
            this.SkuIndirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderPlanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderPlanned.ToString();
            this.SkuIndirectOrder = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrder == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrder.ToString();
            this.SkuIndirectOrderRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderRatio.ToString();
            this.StoresVisitedPLanned = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedPLanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedPLanned.ToString();
            this.StoresVisited = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisited == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisited.ToString();
            this.StoresVisitedRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedRatio.ToString();
            this.NewStoreCreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedPlanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedPlanned.ToString();
            this.NewStoreCreated = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreated == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreated.ToString();
            this.NewStoreCreatedRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedRatio.ToString();
            this.NumberOfStoreVisitsPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsPlanned.ToString();
            this.NumberOfStoreVisits = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisits == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisits.ToString();
            this.NumberOfStoreVisitsRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsRatio.ToString();
        }
    }
}