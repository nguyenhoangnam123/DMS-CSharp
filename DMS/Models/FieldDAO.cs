using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class FieldDAO
    {
        public FieldDAO()
        {
            PermissionFieldMappings = new HashSet<PermissionFieldMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long MenuId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual ICollection<PermissionFieldMappingDAO> PermissionFieldMappings { get; set; }
    }
}
