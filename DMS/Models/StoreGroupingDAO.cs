using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreGroupingDAO
    {
        public StoreGroupingDAO()
        {
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
