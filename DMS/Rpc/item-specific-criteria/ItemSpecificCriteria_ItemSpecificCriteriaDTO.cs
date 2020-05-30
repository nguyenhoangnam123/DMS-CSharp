using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_criteria
{
    public class ItemSpecificCriteria_ItemSpecificCriteriaDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ItemSpecificCriteria_ItemSpecificKpiContentDTO> ItemSpecificKpiContents { get; set; }
        public ItemSpecificCriteria_ItemSpecificCriteriaDTO() {}
        public ItemSpecificCriteria_ItemSpecificCriteriaDTO(ItemSpecificCriteria ItemSpecificCriteria)
        {
            this.Id = ItemSpecificCriteria.Id;
            this.Code = ItemSpecificCriteria.Code;
            this.Name = ItemSpecificCriteria.Name;
            this.ItemSpecificKpiContents = ItemSpecificCriteria.ItemSpecificKpiContents?.Select(x => new ItemSpecificCriteria_ItemSpecificKpiContentDTO(x)).ToList();
            this.Errors = ItemSpecificCriteria.Errors;
        }
    }

    public class ItemSpecificCriteria_ItemSpecificCriteriaFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public ItemSpecificCriteriaOrder OrderBy { get; set; }
    }
}
