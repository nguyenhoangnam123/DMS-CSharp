using Common;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_VariationGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ProductId { get; set; }

        public Product_VariationGroupingDTO() { }
        public Product_VariationGroupingDTO(VariationGrouping VariationGrouping)
        {
            this.Id = VariationGrouping.Id;
            this.Name = VariationGrouping.Name;
            this.ProductId = VariationGrouping.ProductId;
        }
    }

    public class Product_VariationGroupingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter ProductId { get; set; }

        public VariationGroupingOrder OrderBy { get; set; }
    }
}