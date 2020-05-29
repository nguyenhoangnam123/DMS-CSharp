using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ImageStoreCheckingMappingDTO : DataDTO
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
        
        public StoreChecking_ImageStoreCheckingMappingDTO() {}
        public StoreChecking_ImageStoreCheckingMappingDTO(StoreCheckingImageMapping ImageStoreCheckingMapping)
        {
            this.ImageId = ImageStoreCheckingMapping.ImageId;
            this.StoreCheckingId = ImageStoreCheckingMapping.StoreCheckingId;
            this.AlbumId = ImageStoreCheckingMapping.AlbumId;
            this.StoreId = ImageStoreCheckingMapping.StoreId;
            this.SaleEmployeeId = ImageStoreCheckingMapping.SaleEmployeeId;
            this.ShootingAt = ImageStoreCheckingMapping.ShootingAt;
            this.Album = ImageStoreCheckingMapping.Album == null ? null : new StoreChecking_AlbumDTO(ImageStoreCheckingMapping.Album);
            this.SaleEmployee = ImageStoreCheckingMapping.SaleEmployee == null ? null : new StoreChecking_AppUserDTO(ImageStoreCheckingMapping.SaleEmployee);
            this.Image = ImageStoreCheckingMapping.Image == null ? null : new StoreChecking_ImageDTO(ImageStoreCheckingMapping.Image);
            this.Store = ImageStoreCheckingMapping.Store == null ? null : new StoreChecking_StoreDTO(ImageStoreCheckingMapping.Store);
            this.Errors = ImageStoreCheckingMapping.Errors;
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