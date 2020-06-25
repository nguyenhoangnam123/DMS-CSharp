using Common;
using DMS.Entities;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReport_KpiPeriodDTO : DataDTO
    {

        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public KpiGeneralEmployeeReport_KpiPeriodDTO() { }
        public KpiGeneralEmployeeReport_KpiPeriodDTO(KpiPeriod KpiPeriod)
        {

            this.Id = KpiPeriod.Id;

            this.Code = KpiPeriod.Code;

            this.Name = KpiPeriod.Name;

            this.Errors = KpiPeriod.Errors;
        }
    }

    public class KpiGeneralEmployeeReport_KpiPeriodFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public KpiPeriodOrder OrderBy { get; set; }
    }
}