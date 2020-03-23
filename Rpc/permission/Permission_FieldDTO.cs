using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.permission
{
    public class Permission_FieldDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public long MenuId { get; set; }
        
        public bool IsDeleted { get; set; }
        

        public Permission_FieldDTO() {}
        public Permission_FieldDTO(Field Field)
        {
            
            this.Id = Field.Id;
            
            this.Name = Field.Name;
            
            this.Type = Field.Type;
            
            this.MenuId = Field.MenuId;
            
            this.IsDeleted = Field.IsDeleted;
            
        }
    }

    public class Permission_FieldFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Type { get; set; }
        
        public IdFilter MenuId { get; set; }
        
        public FieldOrder OrderBy { get; set; }
    }
}