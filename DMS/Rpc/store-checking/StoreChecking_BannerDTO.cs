using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_BannerDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public long? Priority { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public long? ImageId { get; set; }
        public long StatusId { get; set; }
        public List<StoreChecking_ImageDTO> Images { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public StoreChecking_BannerDTO() { }
        public StoreChecking_BannerDTO(Banner Banner)
        {
            this.Id = Banner.Id;
            this.Code = Banner.Code;
            this.Title = Banner.Title;
            this.Priority = Banner.Priority;
            this.Content = Banner.Content;
            this.CreatorId = Banner.CreatorId;
            this.ImageId = Banner.ImageId;
            this.StatusId = Banner.StatusId;
            this.Images = new List<StoreChecking_ImageDTO> { Banner.Image == null ? null : new StoreChecking_ImageDTO(Banner.Image) };
            this.CreatedAt = Banner.CreatedAt;
            this.UpdatedAt = Banner.UpdatedAt;
            this.Errors = Banner.Errors;
        }
    }

    public class StoreChecking_BannerFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Title { get; set; }
        public LongFilter Priority { get; set; }
        public StringFilter Content { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter ImageId { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public BannerOrder OrderBy { get; set; }
    }
}
