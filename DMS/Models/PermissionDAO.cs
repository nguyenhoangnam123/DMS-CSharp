using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PermissionDAO
    {
        public PermissionDAO()
        {
            PermissionFieldMappings = new HashSet<PermissionFieldMappingDAO>();
            PermissionPageMappings = new HashSet<PermissionPageMappingDAO>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public long MenuId { get; set; }
        public long StatusId { get; set; }

        public virtual MenuDAO Menu { get; set; }
        public virtual RoleDAO Role { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<PermissionFieldMappingDAO> PermissionFieldMappings { get; set; }
        public virtual ICollection<PermissionPageMappingDAO> PermissionPageMappings { get; set; }
    }
}
