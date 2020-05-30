using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi_content
{
    public class ItemSpecificKpiContent_ItemSpecificKpiContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long ItemSpecificKpiId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long ItemId { get; set; }
        public long Value { get; set; }
        public ItemSpecificKpiContent_ItemDTO Item { get; set; }
        public ItemSpecificKpiContent_ItemSpecificCriteriaDTO ItemSpecificCriteria { get; set; }
        public ItemSpecificKpiContent_ItemSpecificKpiDTO ItemSpecificKpi { get; set; }
        public ItemSpecificKpiContent_ItemSpecificKpiContentDTO() {}
        public ItemSpecificKpiContent_ItemSpecificKpiContentDTO(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            this.Id = ItemSpecificKpiContent.Id;
            this.ItemSpecificKpiId = ItemSpecificKpiContent.ItemSpecificKpiId;
            this.ItemSpecificCriteriaId = ItemSpecificKpiContent.ItemSpecificCriteriaId;
            this.ItemId = ItemSpecificKpiContent.ItemId;
            this.Value = ItemSpecificKpiContent.Value;
            this.Item = ItemSpecificKpiContent.Item == null ? null : new ItemSpecificKpiContent_ItemDTO(ItemSpecificKpiContent.Item);
            this.ItemSpecificCriteria = ItemSpecificKpiContent.ItemSpecificCriteria == null ? null : new ItemSpecificKpiContent_ItemSpecificCriteriaDTO(ItemSpecificKpiContent.ItemSpecificCriteria);
            this.ItemSpecificKpi = ItemSpecificKpiContent.ItemSpecificKpi == null ? null : new ItemSpecificKpiContent_ItemSpecificKpiDTO(ItemSpecificKpiContent.ItemSpecificKpi);
            this.Errors = ItemSpecificKpiContent.Errors;
        }
    }

    public class ItemSpecificKpiContent_ItemSpecificKpiContentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter ItemSpecificKpiId { get; set; }
        public IdFilter ItemSpecificCriteriaId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter Value { get; set; }
        public ItemSpecificKpiContentOrder OrderBy { get; set; }
    }
}
