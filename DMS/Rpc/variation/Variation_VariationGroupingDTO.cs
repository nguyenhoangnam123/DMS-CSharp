using Common;
using DMS.Entities;

namespace DMS.Rpc.variation
{
    public class Variation_VariationGroupingDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public long ProductId { get; set; }


        public Variation_VariationGroupingDTO() { }
        public Variation_VariationGroupingDTO(VariationGrouping VariationGrouping)
        {

            this.Id = VariationGrouping.Id;

            this.Name = VariationGrouping.Name;

            this.ProductId = VariationGrouping.ProductId;

        }
    }

    public class Variation_VariationGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ProductId { get; set; }

        public VariationGroupingOrder OrderBy { get; set; }
    }
}