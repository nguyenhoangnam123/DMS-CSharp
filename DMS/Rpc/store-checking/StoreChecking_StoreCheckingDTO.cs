using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_StoreCheckingDTO : DataDTO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long AppUserId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? CountIndirectSalesOrder { get; set; }
        public long? CountImage { get; set; }
        public List<StoreChecking_ImageStoreCheckingMappingDTO> ImageStoreCheckingMappings { get; set; }
        public StoreChecking_StoreCheckingDTO() {}
        public StoreChecking_StoreCheckingDTO(StoreChecking StoreChecking)
        {
            this.Id = StoreChecking.Id;
            this.StoreId = StoreChecking.StoreId;
            this.AppUserId = StoreChecking.AppUserId;
            this.Longtitude = StoreChecking.Longtitude;
            this.Latitude = StoreChecking.Latitude;
            this.CheckInAt = StoreChecking.CheckInAt;
            this.CheckOutAt = StoreChecking.CheckOutAt;
            this.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            this.CountImage = StoreChecking.CountImage;
            this.ImageStoreCheckingMappings = StoreChecking.ImageStoreCheckingMappings?.Select(x => new StoreChecking_ImageStoreCheckingMappingDTO(x)).ToList();
            this.Errors = StoreChecking.Errors;
        }
    }

    public class StoreChecking_StoreCheckingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DateFilter CheckInAt { get; set; }
        public DateFilter CheckOutAt { get; set; }
        public LongFilter CountIndirectSalesOrder { get; set; }
        public LongFilter CountImage { get; set; }
        public StoreCheckingOrder OrderBy { get; set; }
    }
}
