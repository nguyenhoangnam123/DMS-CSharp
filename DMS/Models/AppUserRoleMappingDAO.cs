﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class AppUserRoleMappingDAO
    {
        public long AppUserId { get; set; }
        public long RoleId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual RoleDAO Role { get; set; }
    }
}
