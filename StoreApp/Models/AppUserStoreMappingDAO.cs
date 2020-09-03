using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class AppUserStoreMappingDAO
    {
        public long AppUserId { get; set; }
        public long StoreId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual StoreDAO Store { get; set; }
    }
}
