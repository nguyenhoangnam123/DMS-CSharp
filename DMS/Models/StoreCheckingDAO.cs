using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreCheckingDAO
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
    }
}
