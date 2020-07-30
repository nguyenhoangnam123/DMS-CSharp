using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AlbumImageMappingDAO
    {
        public long ImageId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }

        public virtual AlbumDAO Album { get; set; }
        public virtual ImageDAO Image { get; set; }
        public virtual StoreDAO Store { get; set; }
    }
}
