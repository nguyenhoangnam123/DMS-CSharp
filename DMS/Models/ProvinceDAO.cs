using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ProvinceDAO
    {
        public ProvinceDAO()
        {
            Districts = new HashSet<DistrictDAO>();
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<DistrictDAO> Districts { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
