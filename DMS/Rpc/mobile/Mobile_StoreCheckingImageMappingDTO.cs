using Common;
using DMS.Entities;
using System;

namespace DMS.Rpc.mobile
{
    public class Mobile_StoreCheckingImageMappingDTO : DataDTO
    {
        public long ImageId { get; set; }
        public long StoreCheckingId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime ShootingAt { get; set; }
        public long? Distance { get; set; }
        public Mobile_AlbumDTO Album { get; set; }
        public Mobile_AppUserDTO SaleEmployee { get; set; }
        public Mobile_ImageDTO Image { get; set; }
        public Mobile_StoreDTO Store { get; set; }

        public Mobile_StoreCheckingImageMappingDTO() { }
        public Mobile_StoreCheckingImageMappingDTO(StoreCheckingImageMapping StoreCheckingImageMapping)
        {
            this.ImageId = StoreCheckingImageMapping.ImageId;
            this.StoreCheckingId = StoreCheckingImageMapping.StoreCheckingId;
            this.AlbumId = StoreCheckingImageMapping.AlbumId;
            this.StoreId = StoreCheckingImageMapping.StoreId;
            this.SaleEmployeeId = StoreCheckingImageMapping.SaleEmployeeId;
            this.ShootingAt = StoreCheckingImageMapping.ShootingAt;
            this.Distance = StoreCheckingImageMapping.Distance;
            this.Album = StoreCheckingImageMapping.Album == null ? null : new Mobile_AlbumDTO(StoreCheckingImageMapping.Album);
            this.SaleEmployee = StoreCheckingImageMapping.SaleEmployee == null ? null : new Mobile_AppUserDTO(StoreCheckingImageMapping.SaleEmployee);
            this.Image = StoreCheckingImageMapping.Image == null ? null : new Mobile_ImageDTO(StoreCheckingImageMapping.Image);
            this.Store = StoreCheckingImageMapping.Store == null ? null : new Mobile_StoreDTO(StoreCheckingImageMapping.Store);
            this.Errors = StoreCheckingImageMapping.Errors;
        }
    }

    public class Mobile_ImageStoreCheckingMappingFilterDTO : FilterDTO
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