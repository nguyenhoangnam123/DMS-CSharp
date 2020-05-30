using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.total_item_specific_criteria
{
    public class TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }
        public TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO() {}
        public TotalItemSpecificCriteria_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            this.Id = TotalItemSpecificCriteria.Id;
            this.Code = TotalItemSpecificCriteria.Code;
            this.Name = TotalItemSpecificCriteria.Name;
            this.ItemSpecificKpiTotalItemSpecificCriteriaMappings = TotalItemSpecificCriteria.ItemSpecificKpiTotalItemSpecificCriteriaMappings?.Select(x => new TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO(x)).ToList();
            this.Errors = TotalItemSpecificCriteria.Errors;
        }
    }

    public class TotalItemSpecificCriteria_TotalItemSpecificCriteriaFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public TotalItemSpecificCriteriaOrder OrderBy { get; set; }
    }
}
