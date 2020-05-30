using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.general_criteria
{
    public class GeneralCriteria_GeneralCriteriaDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<GeneralCriteria_GeneralKpiCriteriaMappingDTO> GeneralKpiCriteriaMappings { get; set; }
        public GeneralCriteria_GeneralCriteriaDTO() {}
        public GeneralCriteria_GeneralCriteriaDTO(GeneralCriteria GeneralCriteria)
        {
            this.Id = GeneralCriteria.Id;
            this.Code = GeneralCriteria.Code;
            this.Name = GeneralCriteria.Name;
            this.GeneralKpiCriteriaMappings = GeneralCriteria.GeneralKpiCriteriaMappings?.Select(x => new GeneralCriteria_GeneralKpiCriteriaMappingDTO(x)).ToList();
            this.Errors = GeneralCriteria.Errors;
        }
    }

    public class GeneralCriteria_GeneralCriteriaFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public GeneralCriteriaOrder OrderBy { get; set; }
    }
}
