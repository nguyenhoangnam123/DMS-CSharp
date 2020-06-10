using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class BrandDAO
    {
        public BrandDAO()
        {
            Products = new HashSet<ProductDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
    }
}
