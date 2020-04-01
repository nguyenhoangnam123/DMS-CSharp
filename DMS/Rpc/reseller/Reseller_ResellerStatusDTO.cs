using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.reseller
{
    public class Reseller_ResellerStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Reseller_ResellerStatusDTO() {}
        public Reseller_ResellerStatusDTO(ResellerStatus ResellerStatus)
        {
            
            this.Id = ResellerStatus.Id;
            
            this.Code = ResellerStatus.Code;
            
            this.Name = ResellerStatus.Name;
            
            this.Errors = ResellerStatus.Errors;
        }
    }

    public class Reseller_ResellerStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ResellerStatusOrder OrderBy { get; set; }
    }
}