using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store
{
    public class Store_StoreTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long StatusId { get; set; }
        

        public Store_StoreTypeDTO() {}
        public Store_StoreTypeDTO(StoreType StoreType)
        {
            
            this.Id = StoreType.Id;
            
            this.Code = StoreType.Code;
            
            this.Name = StoreType.Name;
            
            this.StatusId = StoreType.StatusId;
            
        }
    }

    public class Store_StoreTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public StoreTypeOrder OrderBy { get; set; }
    }
}