using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiCriteriaGeneralDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        public List<long> EmployeeIds { get; set; }


        public Mobile_KpiCriteriaGeneralDTO() {}
        public Mobile_KpiCriteriaGeneralDTO(KpiCriteriaGeneral KpiCriteriaGeneral)
        {
            
            this.Id = KpiCriteriaGeneral.Id;
            
            this.Code = KpiCriteriaGeneral.Code;
            
            this.Name = KpiCriteriaGeneral.Name;
            
            this.Errors = KpiCriteriaGeneral.Errors;
        }
    }

    public class Mobile_KpiCriteriaGeneralFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public KpiCriteriaGeneralOrder OrderBy { get; set; }
    }
}