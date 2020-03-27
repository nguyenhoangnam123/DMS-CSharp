using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.role
{
    public class Role_PermissionFieldMappingDTO : DataDTO
    {
        public long PermissionId { get; set; }
        public long FieldId { get; set; }
        public string Value { get; set; }
        public Role_FieldDTO Field { get; set; }

        public Role_PermissionFieldMappingDTO() { }
        public Role_PermissionFieldMappingDTO(PermissionFieldMapping PermissionFieldMapping)
        {
            this.PermissionId = PermissionFieldMapping.PermissionId;
            this.FieldId = PermissionFieldMapping.FieldId;
            this.Value = PermissionFieldMapping.Value;
            this.Field = PermissionFieldMapping.Field == null ? null : new Role_FieldDTO(PermissionFieldMapping.Field);
        }
    }

    public class Role_PermissionFieldMappingFilterDTO : FilterDTO
    {

        public IdFilter PermissionId { get; set; }

        public IdFilter FieldId { get; set; }

        public StringFilter Value { get; set; }

        public PermissionFieldMappingOrder OrderBy { get; set; }
    }
}
