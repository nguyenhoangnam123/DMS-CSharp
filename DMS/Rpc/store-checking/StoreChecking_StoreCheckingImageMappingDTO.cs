using Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_StoreCheckingImageMappingDTO : DataDTO
    {
        public long ImageId { get; set; }
        public long StoreCheckingId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime ShootingAt { get; set; }
        public StoreChecking_AlbumDTO Album { get; set; }
        public StoreChecking_AppUserDTO SaleEmployee { get; set; }
        public StoreChecking_ImageDTO Image { get; set; }
        public StoreChecking_StoreDTO Store { get; set; }

        public StoreChecking_StoreCheckingImageMappingDTO() { }
        public StoreChecking_StoreCheckingImageMappingDTO(StoreCheckingImageMapping StoreCheckingImageMapping)
        {
            this.ImageId = StoreCheckingImageMapping.ImageId;
            this.StoreCheckingId = StoreCheckingImageMapping.StoreCheckingId;
            this.AlbumId = StoreCheckingImageMapping.AlbumId;
            this.StoreId = StoreCheckingImageMapping.StoreId;
            this.SaleEmployeeId = StoreCheckingImageMapping.SaleEmployeeId;
            this.ShootingAt = StoreCheckingImageMapping.ShootingAt;
            this.Album = StoreCheckingImageMapping.Album == null ? null : new StoreChecking_AlbumDTO(StoreCheckingImageMapping.Album);
            this.SaleEmployee = StoreCheckingImageMapping.SaleEmployee == null ? null : new StoreChecking_AppUserDTO(StoreCheckingImageMapping.SaleEmployee);
            this.Image = StoreCheckingImageMapping.Image == null ? null : new StoreChecking_ImageDTO(StoreCheckingImageMapping.Image);
            this.Store = StoreCheckingImageMapping.Store == null ? null : new StoreChecking_StoreDTO(StoreCheckingImageMapping.Store);
            this.Errors = StoreCheckingImageMapping.Errors;
        }
    }

    public class StoreChecking_ImageStoreCheckingMappingFilterDTO : FilterDTO
    {

        public IdFilter ImageId { get; set; }

        public IdFilter StoreCheckingId { get; set; }

        public IdFilter AlbumId { get; set; }

        public IdFilter StoreId { get; set; }

        public IdFilter SaleEmployeeId { get; set; }

        public DateFilter ShootingAt { get; set; }

        public ImageStoreCheckingMappingOrder OrderBy { get; set; }
    }
}