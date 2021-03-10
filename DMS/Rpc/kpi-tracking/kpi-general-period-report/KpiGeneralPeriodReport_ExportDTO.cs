using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiGeneralPeriodReport_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiGeneralPeriodReport_SaleEmployeeDTO> Lines { get; set; }
        public KpiGeneralPeriodReport_ExportDTO(KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO)
        {
            this.OrganizationName = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.OrganizationName;
            this.Lines = KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO.SaleEmployees;
        }
    }

    public class KpiGeneralPeriodReport_LineDTO : DataDTO
    {
        public long STT { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }

        #region các chỉ tiêu tạm ẩn
        ////Số đơn hàng gián tiếp
        //public string TotalIndirectOrdersPLanned { get; set; }
        //public string TotalIndirectOrders { get; set; }
        //public string TotalIndirectOrdersRatio { get; set; }

        ////Tổng sản lượng theo đơn hàng gián tiếp
        //public string TotalIndirectQuantityPlanned { get; set; }
        //public string TotalIndirectQuantity { get; set; }
        //public string TotalIndirectQuantityRatio { get; set; }

        ////SKU gián tiếp
        //public string SkuIndirectOrderPlanned { get; set; }
        //public string SkuIndirectOrder { get; set; }
        //public string SkuIndirectOrderRatio { get; set; }

        ////Số đơn hàng trực tiếp
        //public string TotalDirectOrdersPLanned { get; set; }
        //public string TotalDirectOrders { get; set; }
        //public string TotalDirectOrdersRatio { get; set; }

        ////Tổng sản lượng theo đơn hàng trực tiếp
        //public string TotalDirectQuantityPlanned { get; set; }
        //public string TotalDirectQuantity { get; set; }
        //public string TotalDirectQuantityRatio { get; set; }

        ////Doanh thu theo đơn hàng trực tiếp
        //public string TotalDirectSalesAmountPlanned { get; set; }
        //public string TotalDirectSalesAmount { get; set; }
        //public string TotalDirectSalesAmountRatio { get; set; }

        ////SKU trực tiếp
        //public string SkuDirectOrderPlanned { get; set; }
        //public string SkuDirectOrder { get; set; }
        //public string SkuDirectOrderRatio { get; set; }
        #endregion

        //Tổng doanh thu đơn hàng
        public string TotalIndirectSalesAmountPlanned { get; set; }
        public string TotalIndirectSalesAmount { get; set; }
        public string TotalIndirectSalesAmountRatio { get; set; }

        //Doanh thu C2 Trọng điểm
        public string RevenueC2TDPlanned { get; set; }
        public string RevenueC2TD { get; set; }
        public string RevenueC2TDRatio { get; set; }

        //Doanh thu C2 Siêu lớn
        public string RevenueC2SLPlanned { get; set; }
        public string RevenueC2SL { get; set; }
        public string RevenueC2SLRatio { get; set; }

        //Doanh thu C2
        public string RevenueC2Planned { get; set; }
        public string RevenueC2 { get; set; }
        public string RevenueC2Ratio { get; set; }

        //Số đại lý trọng điểm mở mới
        public string NewStoreC2CreatedPlanned { get; set; }
        public string NewStoreC2Created { get; set; }
        public string NewStoreC2CreatedRatio { get; set; }

        //Số thông tin phản ánh
        public string TotalProblemPlanned { get; set; }
        public string TotalProblem { get; set; }
        public string TotalProblemRatio { get; set; }

        //Số hình ảnh chụp
        public string TotalImagePlanned { get; set; }
        public string TotalImage { get; set; }
        public string TotalImageRatio { get; set; }

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

            #region các chỉ tiêu tạm ẩn
            //this.TotalIndirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersPLanned).ToString();
            //this.TotalIndirectOrders = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrders == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrders.ToString();
            //this.TotalIndirectOrdersRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectOrdersRatio.ToString();
            //this.TotalIndirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityPlanned).ToString();
            //this.TotalIndirectQuantity = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantity == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantity.ToString();
            //this.TotalIndirectQuantityRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectQuantityRatio.ToString();
            //this.SkuIndirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderPlanned == null ? "" : (Math.Round(KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderPlanned.Value, 1)).ToString();
            //this.SkuIndirectOrder = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrder == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrder.ToString();
            //this.SkuIndirectOrderRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuIndirectOrderRatio.ToString();
            //this.TotalDirectOrdersPLanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrdersPLanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrdersPLanned).ToString();
            //this.TotalDirectOrders = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrders == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrders.ToString();
            //this.TotalDirectOrdersRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrdersRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectOrdersRatio.ToString();
            //this.TotalDirectQuantityPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantityPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantityPlanned).ToString();
            //this.TotalDirectQuantity = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantity == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantity.ToString();
            //this.TotalDirectQuantityRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantityRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectQuantityRatio.ToString();
            //this.TotalDirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmountPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmountPlanned).ToString();
            //this.TotalDirectSalesAmount = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmount == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmount.ToString();
            //this.TotalDirectSalesAmountRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmountRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalDirectSalesAmountRatio.ToString();
            //this.SkuDirectOrderPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrderPlanned == null ? "" : (Math.Round(KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrderPlanned.Value, 1)).ToString();
            //this.SkuDirectOrder = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrder == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrder.ToString();
            //this.SkuDirectOrderRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrderRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.SkuDirectOrderRatio.ToString();
            #endregion
            this.TotalIndirectSalesAmountPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountPlanned).ToString();
            this.TotalIndirectSalesAmount = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmount == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmount.ToString();
            this.TotalIndirectSalesAmountRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountRatio.ToString();

            this.RevenueC2TDPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TDPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TDPlanned).ToString();
            this.RevenueC2TD = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TD == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TD.ToString();
            this.RevenueC2TDRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TDRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2TDRatio.ToString();

            this.RevenueC2SLPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2SLPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2SLPlanned).ToString();
            this.RevenueC2SL = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2SL == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2SL.ToString();
            this.RevenueC2SLRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2SLRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalIndirectSalesAmountRatio.ToString();

            this.RevenueC2Planned = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2Planned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2Planned).ToString();
            this.RevenueC2 = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2 == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2.ToString();
            this.RevenueC2Ratio = KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2Ratio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.RevenueC2Ratio.ToString();

            this.NewStoreC2CreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2CreatedPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2CreatedPlanned).ToString();
            this.NewStoreC2Created = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2Created == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2Created.ToString();
            this.NewStoreC2CreatedRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2CreatedRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreC2CreatedRatio.ToString();

            this.TotalProblemPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblemPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblemPlanned).ToString();
            this.TotalProblem = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblem == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblem.ToString();
            this.TotalProblemRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblemRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalProblemRatio.ToString();

            this.TotalImagePlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImagePlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImagePlanned).ToString();
            this.TotalImage = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImage == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImage.ToString();
            this.TotalImageRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImageRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.TotalImageRatio.ToString();

            this.StoresVisitedPLanned = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedPLanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedPLanned).ToString();
            this.StoresVisited = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisited == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisited.ToString();
            this.StoresVisitedRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.StoresVisitedRatio.ToString();

            this.NewStoreCreatedPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedPlanned).ToString();
            this.NewStoreCreated = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreated == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreated.ToString();
            this.NewStoreCreatedRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NewStoreCreatedRatio.ToString();

            this.NumberOfStoreVisitsPlanned = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsPlanned == null ? "" : ((long)KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsPlanned).ToString();
            this.NumberOfStoreVisits = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisits == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisits.ToString();
            this.NumberOfStoreVisitsRatio = KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsRatio == null ? "" : KpiGeneralPeriodReport_SaleEmployeeDTO.NumberOfStoreVisitsRatio.ToString();
        }
    }
}
