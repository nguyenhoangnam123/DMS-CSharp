using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreCheckingDAO
    {
        public StoreCheckingDAO()
        {
            ImageStoreCheckingMappings = new HashSet<ImageStoreCheckingMappingDAO>();
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? CountIndirectSalesOrder { get; set; }
        public long? CountImage { get; set; }

        public virtual AppUserDAO SaleEmployee { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual ICollection<ImageStoreCheckingMappingDAO> ImageStoreCheckingMappings { get; set; }
    }
}
