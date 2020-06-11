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
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime ShootingAt { get; set; }
        public MonitorStoreAlbum_AppUserDTO SaleEmployee { get; set; }
        public MonitorStoreAlbum_ImageDTO Image { get; set; }
        public MonitorStoreAlbum_StoreDTO Store { get; set; }
        public MonitorStoreAlbum_StoreCheckingDTO StoreChecking { get; set; }
        public MonitorStoreAlbum_StoreCheckingImageMappingDTO() { }
        public MonitorStoreAlbum_StoreCheckingImageMappingDTO(StoreCheckingImageMapping StoreCheckingImageMapping)
        {
            this.ImageId = StoreCheckingImageMapping.ImageId;
            this.StoreCheckingId = StoreCheckingImageMapping.StoreCheckingId;
            this.AlbumId = StoreCheckingImageMapping.AlbumId;
            this.StoreId = StoreCheckingImageMapping.StoreId;
            this.SaleEmployeeId = StoreCheckingImageMapping.SaleEmployeeId;
            this.ShootingAt = StoreCheckingImageMapping.ShootingAt;
            this.SaleEmployee = StoreCheckingImageMapping.SaleEmployee == null ? null : new MonitorStoreAlbum_AppUserDTO(StoreCheckingImageMapping.SaleEmployee);
            this.Image = StoreCheckingImageMapping.Image == null ? null : new MonitorStoreAlbum_ImageDTO(StoreCheckingImageMapping.Image);
            this.Store = StoreCheckingImageMapping.Store == null ? null : new MonitorStoreAlbum_StoreDTO(StoreCheckingImageMapping.Store);
            this.StoreChecking = StoreCheckingImageMapping.StoreChecking == null ? null : new MonitorStoreAlbum_StoreCheckingDTO(StoreCheckingImageMapping.StoreChecking);
        }
    }
}
