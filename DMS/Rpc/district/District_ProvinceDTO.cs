using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.district
{
    public class District_ProvinceDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public long StatusId { get; set; }
        

        public District_ProvinceDTO() {}
        public District_ProvinceDTO(Province Province)
        {
            
            this.Id = Province.Id;
            
            this.Name = Province.Name;
            
            this.Priority = Province.Priority;
            
            this.StatusId = Province.StatusId;
            
        }
    }

    public class District_ProvinceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public ProvinceOrder OrderBy { get; set; }
    }
}