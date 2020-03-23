using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.unit_of_measure_grouping
{
    public class UnitOfMeasureGrouping_UnitOfMeasureDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public long StatusId { get; set; }
        

        public UnitOfMeasureGrouping_UnitOfMeasureDTO() {}
        public UnitOfMeasureGrouping_UnitOfMeasureDTO(UnitOfMeasure UnitOfMeasure)
        {
            
            this.Id = UnitOfMeasure.Id;
            
            this.Code = UnitOfMeasure.Code;
            
            this.Name = UnitOfMeasure.Name;
            
            this.Description = UnitOfMeasure.Description;
            
            this.StatusId = UnitOfMeasure.StatusId;
            
        }
    }

    public class UnitOfMeasureGrouping_UnitOfMeasureFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Description { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public UnitOfMeasureOrder OrderBy { get; set; }
    }
}