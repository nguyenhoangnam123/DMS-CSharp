using Common;
using DMS.Entities;
using Html2Markdown;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.mobile
{
    public class Mobile_BannerDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public long? Priority { get; set; }
        public string Content { get; set; }
        public long CreatorId { get; set; }
        public long? ImageId { get; set; }
        public long StatusId { get; set; }
        public List<Mobile_ImageDTO> Images { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Mobile_BannerDTO() { }
        public Mobile_BannerDTO(Banner Banner)
        {
            var converter = new Converter();
            this.Id = Banner.Id;
            this.Code = Banner.Code;
            this.Title = Banner.Title;
            this.Priority = Banner.Priority;
            this.Content = converter.Convert(Banner.Content);
            this.CreatorId = Banner.CreatorId;
            this.ImageId = Banner.ImageId;
            this.StatusId = Banner.StatusId;
            this.Images = new List<Mobile_ImageDTO> { Banner.Image == null ? null : new Mobile_ImageDTO(Banner.Image) };
            this.CreatedAt = Banner.CreatedAt;
            this.UpdatedAt = Banner.UpdatedAt;
            this.Errors = Banner.Errors;
        }
    }

    public class Mobile_BannerFilterDTO : FilterDTO
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
