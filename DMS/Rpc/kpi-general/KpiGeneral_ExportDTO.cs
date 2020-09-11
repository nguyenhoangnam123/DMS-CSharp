using Common;
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
        public KpiGeneral_ExportCriterialDTO TotalIndirectOrders { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalIndirectQuantity { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalIndirectSalesAmount { get; set; }
        public KpiGeneral_ExportCriterialDTO SKUIndirectOrder { get; set; }
        public KpiGeneral_ExportCriterialDTO StoresVisited { get; set; }
        public KpiGeneral_ExportCriterialDTO NewStoresCreated { get; set; }
        public KpiGeneral_ExportCriterialDTO NumberOfStoreVisits { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalDirectOrders { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalDirectQuantity { get; set; }
        public KpiGeneral_ExportCriterialDTO TotalDirectSalesAmount { get; set; }
        public KpiGeneral_ExportCriterialDTO SKUDirectOrder { get; set; }
    }

    public class KpiGeneral_ExportCriterialDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public decimal? M1Value { get; set; }
        public string M1 => M1Value == null ? "" : M1Value.ToString().Replace(".0000", string.Empty);
        public decimal? M2Value { get; set; }
        public string M2 => M2Value == null ? "" : M2Value.ToString().Replace(".0000", string.Empty);
        public decimal? M3Value { get; set; }
        public string M3 => M3Value == null ? "" : M3Value.ToString().Replace(".0000", string.Empty);
        public decimal? M4Value { get; set; }
        public string M4 => M4Value == null ? "" : M4Value.ToString().Replace(".0000", string.Empty);
        public decimal? M5Value { get; set; }
        public string M5 => M5Value == null ? "" : M5Value.ToString().Replace(".0000", string.Empty);
        public decimal? M6Value { get; set; }
        public string M6 => M6Value == null ? "" : M6Value.ToString().Replace(".0000", string.Empty);
        public decimal? M7Value { get; set; }
        public string M7 => M7Value == null ? "" : M7Value.ToString().Replace(".0000", string.Empty);
        public decimal? M8Value { get; set; }
        public string M8 => M8Value == null ? "" : M8Value.ToString().Replace(".0000", string.Empty);
        public decimal? M9Value { get; set; }
        public string M9 => M9Value == null ? "" : M9Value.ToString().Replace(".0000", string.Empty);
        public decimal? M10Value { get; set; }
        public string M10 => M10Value == null ? "" : M10Value.ToString().Replace(".0000", string.Empty);
        public decimal? M11Value { get; set; }
        public string M11 => M11Value == null ? "" : M11Value.ToString().Replace(".0000", string.Empty);
        public decimal? M12Value { get; set; }
        public string M12 => M12Value == null ? "" : M12Value.ToString().Replace(".0000", string.Empty);

        public decimal? Q1Value { get; set; }
        public string Q1 => Q1Value == null ? "" : Q1Value.ToString().Replace(".0000", string.Empty);
        public decimal? Q2Value { get; set; }
        public string Q2 => Q2Value == null ? "" : Q2Value.ToString().Replace(".0000", string.Empty);
        public decimal? Q3Value { get; set; }
        public string Q3 => Q3Value == null ? "" : Q3Value.ToString().Replace(".0000", string.Empty);
        public decimal? Q4Value { get; set; }
        public string Q4 => Q4Value == null ? "" : Q4Value.ToString().Replace(".0000", string.Empty);

        public decimal? YValue { get; set; }
        public string Y => YValue == null ? "" : YValue.ToString().Replace(".0000", string.Empty);
    }
}
