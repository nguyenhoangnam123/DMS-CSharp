using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PermissionOperatorDAO
    {
        public PermissionOperatorDAO()
        {
            PermissionFields = new HashSet<PermissionFieldDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long FieldTypeId { get; set; }

        public virtual FieldTypeDAO FieldType { get; set; }
        public virtual ICollection<PermissionFieldDAO> PermissionFields { get; set; }
    }
}
