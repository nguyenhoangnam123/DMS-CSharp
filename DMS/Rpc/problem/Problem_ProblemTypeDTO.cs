using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.problem
{
    public class Problem_ProblemTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Problem_ProblemTypeDTO() {}
        public Problem_ProblemTypeDTO(ProblemType ProblemType)
        {
            
            this.Id = ProblemType.Id;
            
            this.Code = ProblemType.Code;
            
            this.Name = ProblemType.Name;
            
            this.Errors = ProblemType.Errors;
        }
    }

    public class Problem_ProblemTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ProblemTypeOrder OrderBy { get; set; }
    }
}