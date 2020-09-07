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
        public Mobile_AlbumDTO Album { get; set; }
        public Mobile_ImageDTO Image { get; set; }
        public DateTime ShootingAt { get; set; }
        public Mobile_AlbumImageMappingDTO() { }
        public Mobile_AlbumImageMappingDTO(AlbumImageMapping AlbumImageMapping)
        {
            this.AlbumId = AlbumImageMapping.AlbumId;
            this.StoreId = AlbumImageMapping.StoreId;
            this.ImageId = AlbumImageMapping.ImageId;
            this.ShootingAt = AlbumImageMapping.ShootingAt;
            this.Album = AlbumImageMapping.Album == null ? null : new Mobile_AlbumDTO(AlbumImageMapping.Album);
            this.Image = AlbumImageMapping.Image == null ? null : new Mobile_ImageDTO(AlbumImageMapping.Image);
        }
    }
}
