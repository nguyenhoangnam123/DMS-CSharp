using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_general
{
    public class KpiGeneral_ExportDTO : DataDTO
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalIndirectSalesAmount { get; set; }
        public KpiGeneral_ExportCriterialDTO StoresVisited { get; set; }
        public KpiGeneral_ExportCriterialDTO NewStoresCreated { get; set; }
        public KpiGeneral_ExportCriterialDTO NumberOfStoreVisits { get; set; }
        public KpiGeneral_ExportCriterialDTO RevenueC2TD { get; set; }
        public KpiGeneral_ExportCriterialDTO RevenueC2SL { get; set; }
        public KpiGeneral_ExportCriterialDTO RevenueC2 { get; set; }
        public KpiGeneral_ExportCriterialDTO NewStoresC2Created { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalProblem { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalImage { get; set; }

        #region các chỉ tiêu tạm ẩn
        //public KpiGeneral_ExportCriterialDTO TotalIndirectOrders { get; set; }
        //public KpiGeneral_ExportCriterialDTO TotalIndirectQuantity { get; set; }
        //public KpiGeneral_ExportCriterialDTO SKUIndirectOrder { get; set; }
        //public KpiGeneral_ExportCriterialDTO TotalDirectOrders { get; set; }
        //public KpiGeneral_ExportCriterialDTO TotalDirectQuantity { get; set; }
        //public KpiGeneral_ExportCriterialDTO TotalDirectSalesAmount { get; set; }
        //public KpiGeneral_ExportCriterialDTO SKUDirectOrder { get; set; }
        #endregion
    }

    public class KpiGeneral_ExportCriterialDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public decimal? M1Value { get; set; }
        public decimal? M2Value { get; set; }
        public decimal? M3Value { get; set; }
        public decimal? M4Value { get; set; }
        public decimal? M5Value { get; set; }
        public decimal? M6Value { get; set; }
        public decimal? M7Value { get; set; }
        public decimal? M8Value { get; set; }
        public decimal? M9Value { get; set; }
        public decimal? M10Value { get; set; }
        public decimal? M11Value { get; set; }
        public decimal? M12Value { get; set; }

        public decimal? Q1Value { get; set; }
        public decimal? Q2Value { get; set; }
        public decimal? Q3Value { get; set; }
        public decimal? Q4Value { get; set; }

        public decimal? YValue { get; set; }
    }
}
