using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ImageDAO
    {
        public ImageDAO()
        {
            Banners = new HashSet<BannerDAO>();
            ItemImageMappings = new HashSet<ItemImageMappingDAO>();
            ProblemImageMappings = new HashSet<ProblemImageMappingDAO>();
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
            StoreCheckingImageMappings = new HashSet<StoreCheckingImageMappingDAO>();
            StoreImageMappings = new HashSet<StoreImageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<BannerDAO> Banners { get; set; }
        public virtual ICollection<ItemImageMappingDAO> ItemImageMappings { get; set; }
        public virtual ICollection<ProblemImageMappingDAO> ProblemImageMappings { get; set; }
        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
        public virtual ICollection<StoreCheckingImageMappingDAO> StoreCheckingImageMappings { get; set; }
        public virtual ICollection<StoreImageMappingDAO> StoreImageMappings { get; set; }
    }
}
