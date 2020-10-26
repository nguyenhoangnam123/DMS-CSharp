using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_ExportDTO : DataDTO
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public List<KpiItemReport_LineDTO> Lines { get; set; }
        public KpiItemReport_ExportDTO(KpiItemReport_KpiItemReportDTO KpiItemReport_KpiItemReportDTO)
        {
            this.Username = KpiItemReport_KpiItemReportDTO.Username;
            this.DisplayName = KpiItemReport_KpiItemReportDTO.DisplayName;
            this.Lines = KpiItemReport_KpiItemReportDTO.ItemContents?.Select(x => new KpiItemReport_LineDTO(x)).ToList();
        }
    }

    public class KpiItemReport_LineDTO : DataDTO
    {
        public long STT { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

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

        public KpiItemReport_LineDTO(KpiItemReport_KpiItemContentDTO KpiItemReport_KpiItemContentDTO)
        {
            this.STT = KpiItemReport_KpiItemContentDTO.STT;
            this.ItemCode = KpiItemReport_KpiItemContentDTO.ItemCode;
            this.ItemName = KpiItemReport_KpiItemContentDTO.ItemName;
            this.IndirectQuantityPlanned = KpiItemReport_KpiItemContentDTO.IndirectQuantityPlanned == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectQuantityPlanned.ToString();
            this.IndirectQuantity = KpiItemReport_KpiItemContentDTO.IndirectQuantity == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectQuantity.ToString();
            this.IndirectQuantityRatio = KpiItemReport_KpiItemContentDTO.IndirectQuantityRatio == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectQuantityRatio.ToString();
            this.IndirectRevenuePlanned = KpiItemReport_KpiItemContentDTO.IndirectRevenuePlanned == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectRevenuePlanned.ToString();
            this.IndirectRevenue = KpiItemReport_KpiItemContentDTO.IndirectRevenue == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectRevenue.ToString();
            this.IndirectRevenueRatio = KpiItemReport_KpiItemContentDTO.IndirectRevenueRatio == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectRevenueRatio.ToString();
            this.IndirectAmountPlanned = KpiItemReport_KpiItemContentDTO.IndirectAmountPlanned == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectAmountPlanned.ToString();
            this.IndirectAmount = KpiItemReport_KpiItemContentDTO.IndirectAmount == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectAmount.ToString();
            this.IndirectAmountRatio = KpiItemReport_KpiItemContentDTO.IndirectAmountRatio == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectAmountRatio.ToString();
            this.IndirectStorePlanned = KpiItemReport_KpiItemContentDTO.IndirectStorePlanned == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectStorePlanned.ToString();
            this.IndirectStore = KpiItemReport_KpiItemContentDTO.IndirectStore == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectStore.ToString();
            this.IndirectStoreRatio = KpiItemReport_KpiItemContentDTO.IndirectStoreRatio == null ? "" : KpiItemReport_KpiItemContentDTO.IndirectStoreRatio.ToString();

            this.DirectQuantityPlanned = KpiItemReport_KpiItemContentDTO.DirectQuantityPlanned == null ? "" : KpiItemReport_KpiItemContentDTO.DirectQuantityPlanned.ToString();
            this.DirectQuantity = KpiItemReport_KpiItemContentDTO.DirectQuantity == null ? "" : KpiItemReport_KpiItemContentDTO.DirectQuantity.ToString();
            this.DirectQuantityRatio = KpiItemReport_KpiItemContentDTO.DirectQuantityRatio == null ? "" : KpiItemReport_KpiItemContentDTO.DirectQuantityRatio.ToString();
            this.DirectRevenuePlanned = KpiItemReport_KpiItemContentDTO.DirectRevenuePlanned == null ? "" : KpiItemReport_KpiItemContentDTO.DirectRevenuePlanned.ToString();
            this.DirectRevenue = KpiItemReport_KpiItemContentDTO.DirectRevenue == null ? "" : KpiItemReport_KpiItemContentDTO.DirectRevenue.ToString();
            this.DirectRevenueRatio = KpiItemReport_KpiItemContentDTO.DirectRevenueRatio == null ? "" : KpiItemReport_KpiItemContentDTO.DirectRevenueRatio.ToString();
            this.DirectAmountPlanned = KpiItemReport_KpiItemContentDTO.DirectAmountPlanned == null ? "" : KpiItemReport_KpiItemContentDTO.DirectAmountPlanned.ToString();
            this.DirectAmount = KpiItemReport_KpiItemContentDTO.DirectAmount == null ? "" : KpiItemReport_KpiItemContentDTO.DirectAmount.ToString();
            this.DirectAmountRatio = KpiItemReport_KpiItemContentDTO.DirectAmountRatio == null ? "" : KpiItemReport_KpiItemContentDTO.DirectAmountRatio.ToString();
            this.DirectStorePlanned = KpiItemReport_KpiItemContentDTO.DirectStorePlanned == null ? "" : KpiItemReport_KpiItemContentDTO.DirectStorePlanned.ToString();
            this.DirectStore = KpiItemReport_KpiItemContentDTO.DirectStore == null ? "" : KpiItemReport_KpiItemContentDTO.DirectStore.ToString();
            this.DirectStoreRatio = KpiItemReport_KpiItemContentDTO.DirectStoreRatio == null ? "" : KpiItemReport_KpiItemContentDTO.DirectStoreRatio.ToString();
        }
    }
}
