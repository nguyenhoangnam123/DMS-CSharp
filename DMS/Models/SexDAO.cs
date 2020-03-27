using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class SexDAO
    {
        public SexDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
    }
}
