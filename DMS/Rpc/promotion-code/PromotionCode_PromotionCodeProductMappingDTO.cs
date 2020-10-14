using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion_code
{
    public class PromotionCode_PromotionCodeProductMappingDTO : DataDTO
    {
        public long PromotionCodeId { get; set; }
        public long ProductId { get; set; }
        public PromotionCode_ProductDTO Product { get; set; }   

        public PromotionCode_PromotionCodeProductMappingDTO() {}
        public PromotionCode_PromotionCodeProductMappingDTO(PromotionCodeProductMapping PromotionCodeProductMapping)
        {
            this.PromotionCodeId = PromotionCodeProductMapping.PromotionCodeId;
            this.ProductId = PromotionCodeProductMapping.ProductId;
            this.Product = PromotionCodeProductMapping.Product == null ? null : new PromotionCode_ProductDTO(PromotionCodeProductMapping.Product);
            this.Errors = PromotionCodeProductMapping.Errors;
        }
    }

    public class PromotionCode_PromotionCodeProductMappingFilterDTO : FilterDTO
    {
        
        public IdFilter PromotionCodeId { get; set; }
        
        public IdFilter ProductId { get; set; }
        
        public PromotionCodeProductMappingOrder OrderBy { get; set; }
    }
}