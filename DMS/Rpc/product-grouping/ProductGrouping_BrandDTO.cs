using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product_grouping
{
    public class ProductGrouping_BrandDTO : DataDTO
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long StatusId { get; set; }


        public ProductGrouping_BrandDTO() { }
        public ProductGrouping_BrandDTO(Brand Brand)
        {

            this.Id = Brand.Id;

            this.Code = Brand.Code;

            this.Name = Brand.Name;

            this.StatusId = Brand.StatusId;

        }
    }

    public class ProductGrouping_BrandFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Code { get; set; }

        public StringFilter Name { get; set; }

        public IdFilter StatusId { get; set; }

        public BrandOrder OrderBy { get; set; }
    }
}
