using Common;
using DMS.Entities;

namespace DMS.RpcPublic
{
    public class Public_ProductProductGroupingMappingDTO : DataDTO
    {
        public long ProductId { get; set; }
        public long ProductGroupingId { get; set; }
        public Public_ProductGroupingDTO ProductGrouping { get; set; }

        public Public_ProductProductGroupingMappingDTO() { }
        public Public_ProductProductGroupingMappingDTO(ProductProductGroupingMapping ProductProductGroupingMapping)
        {
            this.ProductId = ProductProductGroupingMapping.ProductId;
            this.ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId;
            this.ProductGrouping = ProductProductGroupingMapping.ProductGrouping == null ? null : new Public_ProductGroupingDTO(ProductProductGroupingMapping.ProductGrouping);
        }
    }

    public class Public_ProductProductGroupingMappingFilterDTO : FilterDTO
    {

        public IdFilter ProductId { get; set; }

        public IdFilter ProductGroupingId { get; set; }

        public ProductProductGroupingMappingOrder OrderBy { get; set; }
    }
}
