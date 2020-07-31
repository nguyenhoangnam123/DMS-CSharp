using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile
{
    public class Mobile_AlbumImageMappingDTO : DataDTO
    {
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long ImageId { get; set; }
        public DateTime ShootingAt { get; set; }
        public Mobile_AlbumImageMappingDTO() { }
        public Mobile_AlbumImageMappingDTO(AlbumImageMapping AlbumImageMapping)
        {
            this.AlbumId = AlbumImageMapping.AlbumId;
            this.StoreId = AlbumImageMapping.StoreId;
            this.ImageId = AlbumImageMapping.ImageId;
            this.ShootingAt = AlbumImageMapping.ShootingAt;
        }
    }
}
