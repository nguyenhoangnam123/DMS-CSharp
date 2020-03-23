using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.store_grouping
{
    public class StoreGrouping_StatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public StoreGrouping_StatusDTO() {}
        public StoreGrouping_StatusDTO(Status Status)
        {
            
            this.Id = Status.Id;
            
            this.Code = Status.Code;
            
            this.Name = Status.Name;
            
        }
    }

    public class StoreGrouping_StatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StatusOrder OrderBy { get; set; }
    }
}