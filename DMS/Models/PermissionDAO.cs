﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PermissionDAO
    {
        public PermissionDAO()
        {
            PermissionActionMappings = new HashSet<PermissionActionMappingDAO>();
            PermissionFields = new HashSet<PermissionFieldDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public long StatusId { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual RoleDAO Role { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<PermissionActionMappingDAO> PermissionActionMappings { get; set; }
        public virtual ICollection<PermissionFieldDAO> PermissionFields { get; set; }
    }
}
