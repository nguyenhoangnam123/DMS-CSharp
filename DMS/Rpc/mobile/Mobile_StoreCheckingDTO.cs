using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreCheckingDTO : DataDTO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? CheckOutLongitude { get; set; }
        public decimal? CheckOutLatitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? CheckInDistance { get; set; }
        public long? CheckOutDistance { get; set; }
        public long? CountIndirectSalesOrder { get; set; }
        public long? CountImage { get; set; }
        public bool IsOpenedStore { get; set; }
        public Mobile_AppUserDTO SaleEmployee { get; set; }
        public Mobile_StoreDTO Store { get; set; }
        public List<Mobile_StoreCheckingImageMappingDTO> StoreCheckingImageMappings { get; set; }
        public Mobile_StoreCheckingDTO() { }
        public Mobile_StoreCheckingDTO(StoreChecking StoreChecking)
        {
            this.Id = StoreChecking.Id;
            this.StoreId = StoreChecking.StoreId;
            this.SaleEmployeeId = StoreChecking.SaleEmployeeId;
            this.Longitude = StoreChecking.Longitude;
            this.Latitude = StoreChecking.Latitude;
            this.CheckOutLongitude = StoreChecking.CheckOutLongitude;
            this.CheckOutLatitude = StoreChecking.CheckOutLatitude;
            this.CheckInAt = StoreChecking.CheckInAt;
            this.CheckOutAt = StoreChecking.CheckOutAt;
            this.CheckInDistance = StoreChecking.CheckInDistance;
            this.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            this.CountIndirectSalesOrder = StoreChecking.CountIndirectSalesOrder;
            this.CountImage = StoreChecking.ImageCounter;
            this.IsOpenedStore = StoreChecking.IsOpenedStore;
            this.SaleEmployee = StoreChecking.SaleEmployee == null ? null : new Mobile_AppUserDTO(StoreChecking.SaleEmployee);
            this.Store = StoreChecking.Store == null ? null : new Mobile_StoreDTO(StoreChecking.Store);
            this.StoreCheckingImageMappings = StoreChecking.StoreCheckingImageMappings?.Select(x => new Mobile_StoreCheckingImageMappingDTO(x)).ToList();
            this.Errors = StoreChecking.Errors;
        }
    }

    public class Mobile_StoreCheckingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DecimalFilter Longitude { get; set; }
        public DecimalFilter Latitude { get; set; }
        public DateFilter CheckInAt { get; set; }
        public DateFilter CheckOutAt { get; set; }
        public LongFilter CountIndirectSalesOrder { get; set; }
        public LongFilter CountImage { get; set; }
        public StoreCheckingOrder OrderBy { get; set; }
    }
}
