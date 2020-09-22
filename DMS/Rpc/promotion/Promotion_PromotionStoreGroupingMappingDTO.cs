using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionStoreGroupingMappingDTO : DataDTO
    {
        public long PromotionId { get; set; }
        public long StoreGroupingId { get; set; }
        public Promotion_StoreGroupingDTO StoreGrouping { get; set; }   
        
        public Promotion_PromotionStoreGroupingMappingDTO() {}
        public Promotion_PromotionStoreGroupingMappingDTO(PromotionStoreGroupingMapping PromotionStoreGroupingMapping)
        {
            this.PromotionId = PromotionStoreGroupingMapping.PromotionId;
            this.StoreGroupingId = PromotionStoreGroupingMapping.StoreGroupingId;
            this.StoreGrouping = PromotionStoreGroupingMapping.StoreGrouping == null ? null : new Promotion_StoreGroupingDTO(PromotionStoreGroupingMapping.StoreGrouping);
            this.Errors = PromotionStoreGroupingMapping.Errors;
        }
    }

    public class Promotion_PromotionStoreGroupingMappingFilterDTO : FilterDTO
    {
        
        public IdFilter PromotionId { get; set; }
        
        public IdFilter StoreGroupingId { get; set; }
        
        public PromotionStoreGroupingMappingOrder OrderBy { get; set; }
    }
}