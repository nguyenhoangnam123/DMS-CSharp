using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_StoreCheckingDTO : DataDTO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? CountIndirectSalesOrder { get; set; }
        public long? CountImage { get; set; }
        public StoreChecking_AppUserDTO SaleEmployee { get; set; }
        public StoreChecking_StoreDTO Store { get; set; }
        public List<StoreChecking_StoreCheckingImageMappingDTO> StoreCheckingImageMappings { get; set; }
        public StoreChecking_StoreCheckingDTO() { }
        public StoreChecking_StoreCheckingDTO(StoreChecking StoreChecking)
        {
            this.Id = StoreChecking.Id;
            this.StoreId = StoreChecking.StoreId;
            this.SaleEmployeeId = StoreChecking.SaleEmployeeId;
            this.Longtitude = StoreChecking.Longtitude;
            this.Latitude = StoreChecking.Latitude;
            this.CheckInAt = StoreChecking.CheckInAt;
            this.CheckOutAt = StoreChecking.CheckOutAt;
            this.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            this.CountImage = StoreChecking.CountImage;
            this.SaleEmployee = StoreChecking.SaleEmployee == null ? null : new StoreChecking_AppUserDTO(StoreChecking.SaleEmployee);
            this.Store = StoreChecking.Store == null ? null : new StoreChecking_StoreDTO(StoreChecking.Store);
            this.StoreCheckingImageMappings = StoreChecking.StoreCheckingImageMappings?.Select(x => new StoreChecking_StoreCheckingImageMappingDTO(x)).ToList();
            this.Errors = StoreChecking.Errors;
        }
    }

    public class StoreChecking_StoreCheckingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DecimalFilter Longtitude { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DateFilter CheckInAt { get; set; }
        public DateFilter CheckOutAt { get; set; }
        public LongFilter CountIndirectSalesOrder { get; set; }
        public LongFilter CountImage { get; set; }
        public StoreCheckingOrder OrderBy { get; set; }
    }
}
