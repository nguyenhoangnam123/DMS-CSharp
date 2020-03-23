using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.permission
{
    public class Permission_PermissionFieldMappingDTO : DataDTO
    {
        public long PermissionId { get; set; }
        public long FieldId { get; set; }
        public string Value { get; set; }
        public Permission_FieldDTO Field { get; set; }   
        
        public Permission_PermissionFieldMappingDTO() {}
        public Permission_PermissionFieldMappingDTO(PermissionFieldMapping PermissionFieldMapping)
        {
            this.PermissionId = PermissionFieldMapping.PermissionId;
            this.FieldId = PermissionFieldMapping.FieldId;
            this.Value = PermissionFieldMapping.Value;
            this.Field = PermissionFieldMapping.Field == null ? null : new Permission_FieldDTO(PermissionFieldMapping.Field);
        }
    }

    public class Permission_PermissionFieldMappingFilterDTO : FilterDTO
    {
        
        public IdFilter PermissionId { get; set; }
        
        public IdFilter FieldId { get; set; }
        
        public StringFilter Value { get; set; }
        
        public PermissionFieldMappingOrder OrderBy { get; set; }
    }
}