using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.product_grouping
{
    public class ProductGrouping_ProductGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public ProductGrouping_ProductGroupingDTO Parent { get; set; }
        public List<ProductGrouping_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public ProductGrouping_ProductGroupingDTO() {}
        public ProductGrouping_ProductGroupingDTO(ProductGrouping ProductGrouping)
        {
            this.Id = ProductGrouping.Id;
            this.Code = ProductGrouping.Code;
            this.Name = ProductGrouping.Name;
            this.ParentId = ProductGrouping.ParentId;
            this.Path = ProductGrouping.Path;
            this.Description = ProductGrouping.Description;
            this.Parent = ProductGrouping.Parent == null ? null : new ProductGrouping_ProductGroupingDTO(ProductGrouping.Parent);
            this.ProductProductGroupingMappings = ProductGrouping.ProductProductGroupingMappings?.Select(x => new ProductGrouping_ProductProductGroupingMappingDTO(x)).ToList();
        }
    }

    public class ProductGrouping_ProductGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public StringFilter Description { get; set; }
        public ProductGroupingOrder OrderBy { get; set; }
    }
}
