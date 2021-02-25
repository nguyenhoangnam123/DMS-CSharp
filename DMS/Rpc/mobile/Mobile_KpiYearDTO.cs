using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiYearDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Mobile_KpiYearDTO() {}
        public Mobile_KpiYearDTO(KpiYear KpiYear)
        {
            
            this.Id = KpiYear.Id;
            
            this.Code = KpiYear.Code;
            
            this.Name = KpiYear.Name;
            
            this.Errors = KpiYear.Errors;
        }
    }

    public class Mobile_KpiYearFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public KpiYearOrder OrderBy { get; set; }
    }
}