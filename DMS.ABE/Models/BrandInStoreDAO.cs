using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class BrandInStoreDAO
    {
        public BrandInStoreDAO()
        {
            BrandInStoreProductGroupingMappings = new HashSet<BrandInStoreProductGroupingMappingDAO>();
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public long BrandId { get; set; }
        public long Top { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual BrandDAO Brand { get; set; }
        public virtual AppUserDAO Creator { get; set; }
        public virtual StoreDAO Store { get; set; }
        public virtual ICollection<BrandInStoreProductGroupingMappingDAO> BrandInStoreProductGroupingMappings { get; set; }
    }
}
