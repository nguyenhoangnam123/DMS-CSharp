using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.showing_warehouse
{
    public class ShowingWarehouse_ProvinceDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        

        public ShowingWarehouse_ProvinceDTO() {}
        public ShowingWarehouse_ProvinceDTO(Province Province)
        {
            
            this.Id = Province.Id;
            
            this.Code = Province.Code;
            
            this.Name = Province.Name;
            
            this.Priority = Province.Priority;
            
            this.StatusId = Province.StatusId;
            
            this.RowId = Province.RowId;
            
            this.Errors = Province.Errors;
        }
    }

    public class ShowingWarehouse_ProvinceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public ProvinceOrder OrderBy { get; set; }
    }
}