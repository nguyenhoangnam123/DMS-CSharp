using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product
{
    public class Product_VariationDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long VariationId { get; set; }
        public Product_VariationDTO() { }
        public Product_VariationDTO(Variation Variation)
        {
            this.Id = Variation.Id;
            this.Code = Variation.Code;
            this.Name = Variation.Name;
            this.VariationId = Variation.Id;
        }
    }

    public class Product_VariationFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter VariationId { get; set; }
        public VariationOrder OrderBy { get; set; }
    }
}
