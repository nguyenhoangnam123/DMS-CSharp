using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_Total
{
    public class KpiTotal_KpiCriteriaTotalDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }


        public KpiTotal_KpiCriteriaTotalDTO() { }
        public KpiTotal_KpiCriteriaTotalDTO(KpiCriteriaTotal KpiCriteriaTotal)
        {

            this.Id = KpiCriteriaTotal.Id;

            this.Code = KpiCriteriaTotal.Code;

            this.Name = KpiCriteriaTotal.Name;

            this.Errors = KpiCriteriaTotal.Errors;
        }
    }
}
