using Common;
using DMS.Entities;
using DMS.Models;
using System;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StoreCheckingDTO : DataDTO
    {

        public long Id { get; set; }

        public long StoreId { get; set; }

        public long SaleEmployeeId { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public DateTime? CheckInAt { get; set; }

        public DateTime? CheckOutAt { get; set; }

        public long? IndirectSalesOrderCounter { get; set; }

        public long? ImageCounter { get; set; }


        public MobileSync_StoreCheckingDTO() { }
        public MobileSync_StoreCheckingDTO(StoreCheckingDAO StoreCheckingDAO)
        {
            this.Id = StoreCheckingDAO.Id;
            this.StoreId = StoreCheckingDAO.StoreId;
            this.SaleEmployeeId = StoreCheckingDAO.SaleEmployeeId;
            this.Longitude = StoreCheckingDAO.Longitude;
            this.Latitude = StoreCheckingDAO.Latitude;
            this.CheckInAt = StoreCheckingDAO.CheckInAt;
            this.CheckOutAt = StoreCheckingDAO.CheckOutAt;
        }
    }

    public class MonitorStoreProblem_StoreCheckingFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public IdFilter StoreId { get; set; }

        public IdFilter SaleEmployeeId { get; set; }

        public DecimalFilter Longitude { get; set; }

        public DecimalFilter Latitude { get; set; }

        public DateFilter CheckInAt { get; set; }

        public DateFilter CheckOutAt { get; set; }

        public LongFilter IndirectSalesOrderCounter { get; set; }

        public LongFilter ImageCounter { get; set; }

        public StoreCheckingOrder OrderBy { get; set; }
    }
}