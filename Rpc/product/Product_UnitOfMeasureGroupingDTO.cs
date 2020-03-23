using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.product
{
    public class Product_UnitOfMeasureGroupingDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long UnitOfMeasureId { get; set; }
        
        public long StatusId { get; set; }
        

        public Product_UnitOfMeasureGroupingDTO() {}
        public Product_UnitOfMeasureGroupingDTO(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            
            this.Id = UnitOfMeasureGrouping.Id;
            
            this.Name = UnitOfMeasureGrouping.Name;
            
            this.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
            
            this.StatusId = UnitOfMeasureGrouping.StatusId;
            
        }
    }

    public class Product_UnitOfMeasureGroupingFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter UnitOfMeasureId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public UnitOfMeasureGroupingOrder OrderBy { get; set; }
    }
}