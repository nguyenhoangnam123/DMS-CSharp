﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PageDAO
    {
        public PageDAO()
        {
            ActionPageMappings = new HashSet<ActionPageMappingDAO>();
            PermissionPageMappings = new HashSet<PermissionPageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual ICollection<ActionPageMappingDAO> ActionPageMappings { get; set; }
        public virtual ICollection<PermissionPageMappingDAO> PermissionPageMappings { get; set; }
    }
}
