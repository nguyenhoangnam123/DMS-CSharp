using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.permission
{
    public class Permission_RoleDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long StatusId { get; set; }
        

        public Permission_RoleDTO() {}
        public Permission_RoleDTO(Role Role)
        {
            
            this.Id = Role.Id;
            
            this.Code = Role.Code;
            
            this.Name = Role.Name;
            
            this.StatusId = Role.StatusId;
            
        }
    }

    public class Permission_RoleFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public RoleOrder OrderBy { get; set; }
    }
}