﻿using Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.product_grouping
{
    public class ProductGrouping_ProductTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public DateTime UpdatedTime { get; set; }
        public ProductGrouping_ProductTypeDTO() { }
        public ProductGrouping_ProductTypeDTO(ProductType ProductType)
        {
            this.Id = ProductType.Id;
            this.Code = ProductType.Code;
            this.Name = ProductType.Name;
            this.Description = ProductType.Description;
            this.StatusId = ProductType.StatusId;
            this.UpdatedTime = ProductType.UpdatedAt;
            this.Errors = ProductType.Errors;
        }
    }

    public class ProductGrouping_ProductTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedTime { get; set; }
        public ProductTypeOrder OrderBy { get; set; }
    }
}
