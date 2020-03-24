using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.variation_grouping
{
    public class VariationGrouping_VariationGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ProductId { get; set; }
        public VariationGrouping_ProductDTO Product { get; set; }
        public VariationGrouping_VariationGroupingDTO() {}
        public VariationGrouping_VariationGroupingDTO(VariationGrouping VariationGrouping)
        {
            this.Id = VariationGrouping.Id;
            this.Name = VariationGrouping.Name;
            this.ProductId = VariationGrouping.ProductId;
            this.Product = VariationGrouping.Product == null ? null : new VariationGrouping_ProductDTO(VariationGrouping.Product);
        }
    }

    public class VariationGrouping_VariationGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ProductId { get; set; }
        public VariationGroupingOrder OrderBy { get; set; }
    }
}
