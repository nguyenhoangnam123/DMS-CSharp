﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreHistoryDAO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long AppUserId { get; set; }
        public DateTime? PreviousCreatedAt { get; set; }
        public long? PreviousStoreStatusId { get; set; }
        public long StoreStatusId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual StoreStatusDAO PreviousStoreStatus { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual StoreStatusDAO StoreStatus { get; set; }
    }
}
