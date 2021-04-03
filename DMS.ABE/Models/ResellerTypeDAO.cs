using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class ResellerTypeDAO
    {
        public ResellerTypeDAO()
        {
            Resellers = new HashSet<ResellerDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<ResellerDAO> Resellers { get; set; }
    }
}
