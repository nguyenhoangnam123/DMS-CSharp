using Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.problem
{
    public class Problem_StoreCheckingDTO : DataDTO
    {

        public long Id { get; set; }

        public long StoreId { get; set; }

        public long SaleEmployeeId { get; set; }

        public decimal? Longtitude { get; set; }

        public decimal? Latitude { get; set; }

        public DateTime? CheckInAt { get; set; }

        public DateTime? CheckOutAt { get; set; }

        public long? IndirectSalesOrderCounter { get; set; }

        public long? ImageCounter { get; set; }


        public Problem_StoreCheckingDTO() { }
        public Problem_StoreCheckingDTO(StoreChecking StoreChecking)
        {

            this.Id = StoreChecking.Id;

            this.StoreId = StoreChecking.StoreId;

            this.SaleEmployeeId = StoreChecking.SaleEmployeeId;

            this.Longtitude = StoreChecking.Longtitude;

            this.Latitude = StoreChecking.Latitude;

            this.CheckInAt = StoreChecking.CheckInAt;

            this.CheckOutAt = StoreChecking.CheckOutAt;

            this.Errors = StoreChecking.Errors;
        }
    }

    public class Problem_StoreCheckingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter StoreId { get; set; }

        public IdFilter SaleEmployeeId { get; set; }

        public DecimalFilter Longtitude { get; set; }

        public DecimalFilter Latitude { get; set; }

        public DateFilter CheckInAt { get; set; }

        public DateFilter CheckOutAt { get; set; }

        public LongFilter IndirectSalesOrderCounter { get; set; }

        public LongFilter ImageCounter { get; set; }

        public StoreCheckingOrder OrderBy { get; set; }
    }
}