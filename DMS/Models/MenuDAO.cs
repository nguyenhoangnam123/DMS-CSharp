using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class MenuDAO
    {
        public MenuDAO()
        {
            Fields = new HashSet<FieldDAO>();
            Pages = new HashSet<PageDAO>();
            Permissions = new HashSet<PermissionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<FieldDAO> Fields { get; set; }
        public virtual ICollection<PageDAO> Pages { get; set; }
        public virtual ICollection<PermissionDAO> Permissions { get; set; }
    }
}
