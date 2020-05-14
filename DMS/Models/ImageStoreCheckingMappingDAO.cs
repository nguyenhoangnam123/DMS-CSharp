using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ImageStoreCheckingMappingDAO
    {
        public long ImageId { get; set; }
        public long StoreCheckingId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long AppUserId { get; set; }
        public DateTime ShootingAt { get; set; }
    }
}
