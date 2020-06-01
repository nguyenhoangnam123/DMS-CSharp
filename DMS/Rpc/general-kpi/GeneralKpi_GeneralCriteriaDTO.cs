using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpi_GeneralCriteriaDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public GeneralKpi_GeneralCriteriaDTO() { }
        public GeneralKpi_GeneralCriteriaDTO(GeneralCriteria GeneralCriteria)
        {
            this.Id = GeneralCriteria.Id;
            this.Code = GeneralCriteria.Code;
            this.Name = GeneralCriteria.Name;
        }
    }

    public class GeneralKpi_GeneralCriteriaDTOFilter : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public GeneralCriteriaOrder OrderBy { get; set; }
    }
}
