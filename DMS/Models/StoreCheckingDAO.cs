﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreCheckingDAO
    {
        public StoreCheckingDAO()
        {
            Problems = new HashSet<ProblemDAO>();
            StoreCheckingImageMappings = new HashSet<StoreCheckingImageMappingDAO>();
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public long? IndirectSalesOrderCounter { get; set; }
        public long? ImageCounter { get; set; }
        public bool Planned { get; set; }

        public virtual AppUserDAO SaleEmployee { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual ICollection<ProblemDAO> Problems { get; set; }
        public virtual ICollection<StoreCheckingImageMappingDAO> StoreCheckingImageMappings { get; set; }
    }
}
