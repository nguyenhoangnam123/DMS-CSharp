using DMS.Common;
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
        public long OrganizationId { get; set; }
        public long? Distance { get; set; }
        public long? SaleEmployeeId { get; set; }
        public Mobile_AlbumDTO Album { get; set; }
        public Mobile_OrganizationDTO Organization { get; set; }
        public Mobile_AppUserDTO SaleEmployee { get; set; }
        public Mobile_ImageDTO Image { get; set; }
        public DateTime ShootingAt { get; set; }
        public Mobile_AlbumImageMappingDTO() { }
        public Mobile_AlbumImageMappingDTO(AlbumImageMapping AlbumImageMapping)
        {
            this.AlbumId = AlbumImageMapping.AlbumId;
            this.StoreId = AlbumImageMapping.StoreId;
            this.ImageId = AlbumImageMapping.ImageId;
            this.OrganizationId = AlbumImageMapping.OrganizationId;
            this.ShootingAt = AlbumImageMapping.ShootingAt;
            this.Distance = AlbumImageMapping.Distance;
            this.SaleEmployeeId = AlbumImageMapping.SaleEmployeeId;
            this.Album = AlbumImageMapping.Album == null ? null : new Mobile_AlbumDTO(AlbumImageMapping.Album);
            this.Organization = AlbumImageMapping.Organization == null ? null : new Mobile_OrganizationDTO(AlbumImageMapping.Organization);
            this.SaleEmployee = AlbumImageMapping.SaleEmployee == null ? null : new Mobile_AppUserDTO(AlbumImageMapping.SaleEmployee);
            this.Image = AlbumImageMapping.Image == null ? null : new Mobile_ImageDTO(AlbumImageMapping.Image);
        }
    }
}
