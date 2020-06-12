using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_albums
{
    public class MonitorStoreAlbum_StoreCheckingImageMappingDTO : DataDTO
    {
        public long ImageId { get; set; }
        public long StoreCheckingId { get; set; }
        public long AlbumId { get; set; }
        public DateTime ShootingAt { get; set; }
        public MonitorStoreAlbum_ImageDTO Image { get; set; }
        public MonitorStoreAlbum_StoreCheckingDTO StoreChecking { get; set; }
        public MonitorStoreAlbum_StoreCheckingImageMappingDTO() { }
        public MonitorStoreAlbum_StoreCheckingImageMappingDTO(StoreCheckingImageMapping StoreCheckingImageMapping)
        {
            this.ImageId = StoreCheckingImageMapping.ImageId;
            this.StoreCheckingId = StoreCheckingImageMapping.StoreCheckingId;
            this.AlbumId = StoreCheckingImageMapping.AlbumId;
            this.ShootingAt = StoreCheckingImageMapping.ShootingAt;
            this.Image = StoreCheckingImageMapping.Image == null ? null : new MonitorStoreAlbum_ImageDTO(StoreCheckingImageMapping.Image);
            this.StoreChecking = StoreCheckingImageMapping.StoreChecking == null ? null : new MonitorStoreAlbum_StoreCheckingDTO(StoreCheckingImageMapping.StoreChecking);
        }
    }
}
