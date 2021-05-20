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
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-EN");
            this.STT = KpiProductGroupingReport_KpiProductGroupingContentDTO.STT;
            this.ProductGroupingCode = KpiProductGroupingReport_KpiProductGroupingContentDTO.ProductGroupingCode;
            this.ProductGroupingName = KpiProductGroupingReport_KpiProductGroupingContentDTO.ProductGroupingName;
            this.IndirectQuantityPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityPlanned)
                .ToString("N0", culture);
            this.IndirectQuantity = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantity == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantity)
                .ToString("N0", culture);
            this.IndirectQuantityRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectQuantityRatio)
                .ToString("N0", culture);
            this.IndirectRevenuePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenuePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenuePlanned)
                .ToString("N0", culture);
            this.IndirectRevenue = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenue == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenue)
                .ToString("N0", culture);
            this.IndirectRevenueRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenueRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectRevenueRatio)
                .ToString("N0", culture);
            this.IndirectAmountPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountPlanned)
                .ToString("N0", culture);
            this.IndirectAmount = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmount == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmount)
                .ToString("N0", culture);
            this.IndirectAmountRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectAmountRatio)
                .ToString("N0", culture);
            this.IndirectStorePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStorePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStorePlanned)
                .ToString("N0", culture);
            this.IndirectStore = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStore == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStore)
                .ToString("N0", culture);
            this.IndirectStoreRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStoreRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.IndirectStoreRatio)
                .ToString("N0", culture);

            this.DirectQuantityPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityPlanned)
                .ToString("N0", culture);
            this.DirectQuantity = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantity == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantity)
                .ToString("N0", culture);
            this.DirectQuantityRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectQuantityRatio)
                .ToString("N0", culture);
            this.DirectRevenuePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenuePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenuePlanned)
                .ToString("N0", culture);
            this.DirectRevenue = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenue == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenue)
                .ToString("N0", culture);
            this.DirectRevenueRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenueRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectRevenueRatio)
                .ToString("N0", culture);
            this.DirectAmountPlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountPlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountPlanned)
                .ToString("N0", culture);
            this.DirectAmount = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmount == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmount)
                .ToString("N0", culture);
            this.DirectAmountRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectAmountRatio)
                .ToString("N0", culture);
            this.DirectStorePlanned = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStorePlanned == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStorePlanned)
                .ToString("N0", culture);
            this.DirectStore = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStore == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStore)
                .ToString("N0", culture);
            this.DirectStoreRatio = KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStoreRatio == null ? "" : ((long)KpiProductGroupingReport_KpiProductGroupingContentDTO.DirectStoreRatio)
                .ToString("N0", culture);
        }
    }
}
