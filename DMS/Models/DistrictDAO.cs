using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DistrictDAO
    {
        public DistrictDAO()
        {
            Stores = new HashSet<StoreDAO>();
            Wards = new HashSet<WardDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long ProvinceId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ProvinceDAO Province { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
        public virtual ICollection<WardDAO> Wards { get; set; }
    }
}
