using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ImageDAO
    {
        public ImageDAO()
        {
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
            StoreImageMappings = new HashSet<StoreImageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
        public virtual ICollection<StoreImageMappingDAO> StoreImageMappings { get; set; }
    }
}
