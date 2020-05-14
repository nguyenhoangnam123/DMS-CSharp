using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AlbumDAO
    {
        public AlbumDAO()
        {
            ImageStoreCheckingMappings = new HashSet<ImageStoreCheckingMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ImageStoreCheckingMappingDAO> ImageStoreCheckingMappings { get; set; }
    }
}
