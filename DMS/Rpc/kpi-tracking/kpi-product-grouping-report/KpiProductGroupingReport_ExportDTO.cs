using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_product_grouping_report
{
    public class KpiProductGroupingReport_ExportDTO : DataDTO
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<KpiProductGroupingReport_KpiSaleEmployeeReportDTO> SaleEmployees { get; set; }
        public KpiProductGroupingReport_ExportDTO(KpiProductGroupingReport_KpiProductGroupingReportDTO KpiProductGroupingReport_KpiProductGroupingReportDTO)
        {
            this.OrganizationId = KpiProductGroupingReport_KpiProductGroupingReportDTO.OrganizationId;
            this.OrganizationName = KpiProductGroupingReport_KpiProductGroupingReportDTO.OrganizationName;
            this.SaleEmployees = KpiProductGroupingReport_KpiProductGroupingReportDTO.SaleEmployees == null ? null : KpiProductGroupingReport_KpiProductGroupingReportDTO.SaleEmployees
                .Select(x => new KpiProductGroupingReport_KpiSaleEmployeeReportDTO(x)).ToList();
        }
    }

    public class KpiProductGroupingReport_KpiSaleEmployeeReportDTO : DataDTO
    {
        public long STT { get; set; }
        public long OrganizationId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public List<KpiProductGroupingReport_KpiProductGroupingContentExportDTO> Contents { get; set; }
        public KpiProductGroupingReport_KpiSaleEmployeeReportDTO(KpiProductGroupingReport_KpiSaleEmployeetDTO KpiProductGroupingReport_KpiSaleEmployeetDTO)
        {
            this.STT = KpiProductGroupingReport_KpiSaleEmployeetDTO.STT;
            this.UserName = KpiProductGroupingReport_KpiSaleEmployeetDTO.UserName;
            this.DisplayName = KpiProductGroupingReport_KpiSaleEmployeetDTO.DisplayName;
            this.Contents = KpiProductGroupingReport_KpiSaleEmployeetDTO.Contents == null ? null : KpiProductGroupingReport_KpiSaleEmployeetDTO.Contents
                .Select(x => new KpiProductGroupingReport_KpiProductGroupingContentExportDTO(x)).ToList();
        }
    }

    public class KpiProductGroupingReport_KpiProductGroupingContentExportDTO : DataDTO
    {
        public long STT { get; set; }
        public string ProductGroupingCode { get; set; }
        public string ProductGroupingName { get; set; }

        //Sản lượng theo đơn hàng gián tiếp
        public string IndirectQuantityPlanned { get; set; }
        public string IndirectQuantity { get; set; }
        public string IndirectQuantityRatio { get; set; }

        //Doanh số theo đơn hàng gián tiếp
        public string IndirectRevenuePlanned { get; set; }
        public string IndirectRevenue { get; set; }
        public string IndirectRevenueRatio { get; set; }

        //Số đơn hàng gián tiếp
        public string IndirectAmountPlanned { get; set; }
        public string IndirectAmount { get; set; }
        public string IndirectAmountRatio { get; set; }

        //Số đại lý theo đơn gián tiếp
        public string IndirectStorePlanned { get; set; }
        public string IndirectStore { get; set; }
        public string IndirectStoreRatio { get; set; }

        //Sản lượng theo đơn hàng trực tiếp
        public string DirectQuantityPlanned { get; set; }
        public string DirectQuantity { get; set; }
        public string DirectQuantityRatio { get; set; }

        //Doanh số theo đơn hàng trực tiếp
        public string DirectRevenuePlanned { get; set; }
        public string DirectRevenue { get; set; }
        public string DirectRevenueRatio { get; set; }

        //Số đơn hàng trực tiếp
        public string DirectAmountPlanned { get; set; }
        public string DirectAmount { get; set; }
        public string DirectAmountRatio { get; set; }

        //Số đại lý theo đơn trực tiếp
        public string DirectStorePlanned { get; set; }
        public string DirectStore { get; set; }
        public string DirectStoreRatio { get; set; }

        public KpiProductGroupingReport_KpiProductGroupingContentExportDTO(KpiProductGroupingReport_KpiProductGroupingContentDTO KpiProductGroupingReport_KpiProductGroupingContentDTO)
        {
            this.STT = KpiProductGroupingReport_KpiProductGroupingContentDTO.STT;
            this.ProductGroupingCode = KpiProductGroupingReport_KpiProductGroupingContentDTO.ProductGroupingCode;
            this.ProductGroupingName = KpiProductGroupingReport_KpiProductGroupingContentDTO.ProductGroupingName;
            this.IndirectQuantityPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityPlanned).ToString();
            this.IndirectQuantity = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantity == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantity.ToString();
            this.IndirectQuantityRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityRatio.ToString();
            this.IndirectRevenuePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenuePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenuePlanned).ToString();
            this.IndirectRevenue = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenue == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenue.ToString();
            this.IndirectRevenueRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenueRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenueRatio.ToString();
            this.IndirectAmountPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountPlanned).ToString();
            this.IndirectAmount = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmount == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmount.ToString();
            this.IndirectAmountRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountRatio.ToString();
            this.IndirectStorePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStorePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStorePlanned).ToString();
            this.IndirectStore = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStore == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStore.ToString();
            this.IndirectStoreRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStoreRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStoreRatio.ToString();

            this.DirectQuantityPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityPlanned).ToString();
            this.DirectQuantity = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantity == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantity.ToString();
            this.DirectQuantityRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityRatio.ToString();
            this.DirectRevenuePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenuePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenuePlanned).ToString();
            this.DirectRevenue = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenue == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenue.ToString();
            this.DirectRevenueRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenueRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenueRatio.ToString();
            this.DirectAmountPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountPlanned).ToString();
            this.DirectAmount = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmount == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmount.ToString();
            this.DirectAmountRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountRatio.ToString();
            this.DirectStorePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStorePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStorePlanned).ToString();
            this.DirectStore = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStore == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStore.ToString();
            this.DirectStoreRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStoreRatio == null ? "" : KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStoreRatio.ToString();
        }
    }
}
