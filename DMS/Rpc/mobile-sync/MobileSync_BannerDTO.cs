using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_BannerDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public long? Priority { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public long? ImageId { get; set; }
        public long StatusId { get; set; }
        public MobileSync_AppUserDTO Creator { get; set; }
        public List<MobileSync_ImageDTO> Images { get; set; }
        public MobileSync_StatusDTO Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MobileSync_BannerDTO() { }
        public MobileSync_BannerDTO(BannerDAO BannerDAO)
        {
            this.Id = BannerDAO.Id;
            this.Code = BannerDAO.Code;
            this.Title = BannerDAO.Title;
            this.Priority = BannerDAO.Priority;
            this.Content = BannerDAO.Content;
            this.CreatorId = BannerDAO.CreatorId;
            this.ImageId = BannerDAO.ImageId;
            this.StatusId = BannerDAO.StatusId;
            this.Creator = BannerDAO.Creator == null ? null : new MobileSync_AppUserDTO(BannerDAO.Creator);
            this.Images = new List<MobileSync_ImageDTO> { BannerDAO.Image == null ? null : new MobileSync_ImageDTO(BannerDAO.Image) };
            this.Status = BannerDAO.Status == null ? null : new MobileSync_StatusDTO(BannerDAO.Status);
            this.CreatedAt = BannerDAO.CreatedAt;
            this.UpdatedAt = BannerDAO.UpdatedAt;
        }
    }
}
