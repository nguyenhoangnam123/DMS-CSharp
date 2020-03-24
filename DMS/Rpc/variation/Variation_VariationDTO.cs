using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.variation
{
    public class Variation_VariationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long VariationGroupingId { get; set; }
        public Variation_VariationGroupingDTO VariationGrouping { get; set; }
        public Variation_VariationDTO() {}
        public Variation_VariationDTO(Variation Variation)
        {
            this.Id = Variation.Id;
            this.Code = Variation.Code;
            this.Name = Variation.Name;
            this.VariationGroupingId = Variation.VariationGroupingId;
            this.VariationGrouping = Variation.VariationGrouping == null ? null : new Variation_VariationGroupingDTO(Variation.VariationGrouping);
        }
    }

    public class Variation_VariationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter VariationGroupingId { get; set; }
        public VariationOrder OrderBy { get; set; }
    }
}
