using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ItemSpecificKpiContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long ItemSpecificKpiId { get; set; }
        public long ItemId { get; set; }
        public ItemSpecificKpi_ItemDTO Item { get; set; }   
        public List<ItemSpecificKpi_ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDTO> ItemSpecificKpiContentItemSpecificKpiCriteriaMappings { get; set; }
        public ItemSpecificKpi_ItemSpecificKpiContentDTO() {}
        public ItemSpecificKpi_ItemSpecificKpiContentDTO(ItemSpecificKpiContent ItemSpecificKpiContent)
        {
            this.Id = ItemSpecificKpiContent.Id;
            this.ItemSpecificKpiId = ItemSpecificKpiContent.ItemSpecificKpiId;
            this.ItemId = ItemSpecificKpiContent.ItemId;
            this.Item = ItemSpecificKpiContent.Item == null ? null : new ItemSpecificKpi_ItemDTO(ItemSpecificKpiContent.Item);
            this.ItemSpecificKpiContentItemSpecificKpiCriteriaMappings = ItemSpecificKpiContent.ItemSpecificKpiContentItemSpecificKpiCriteriaMappings?.Select(x => new ItemSpecificKpi_ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDTO(x)).ToList();
            this.Errors = ItemSpecificKpiContent.Errors;
        }
    }

    public class ItemSpecificKpi_ItemSpecificKpiContentFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ItemSpecificKpiId { get; set; }
        
        public IdFilter ItemId { get; set; }
        
        public ItemSpecificKpiContentOrder OrderBy { get; set; }
    }
}