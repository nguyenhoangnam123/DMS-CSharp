using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class UserStatusDAO
    {
        public UserStatusDAO()
        {
            AppUsers = new HashSet<AppUserDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AppUserDAO> AppUsers { get; set; }
    }
}
