using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.general_criteria
{
    public class GeneralCriteria_StatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public GeneralCriteria_StatusDTO() {}
        public GeneralCriteria_StatusDTO(Status Status)
        {
            
            this.Id = Status.Id;
            
            this.Code = Status.Code;
            
            this.Name = Status.Name;
            
            this.Errors = Status.Errors;
        }
    }

    public class GeneralCriteria_StatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StatusOrder OrderBy { get; set; }
    }
}