using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class WardDAO
    {
        public WardDAO()
        {
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long DistrictId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual DistrictDAO District { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
