using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.showing_warehouse
{
    public class ShowingWarehouse_DistrictDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public long ProvinceId { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        

        public ShowingWarehouse_DistrictDTO() {}
        public ShowingWarehouse_DistrictDTO(District District)
        {
            
            this.Id = District.Id;
            
            this.Code = District.Code;
            
            this.Name = District.Name;
            
            this.Priority = District.Priority;
            
            this.ProvinceId = District.ProvinceId;
            
            this.StatusId = District.StatusId;
            
            this.RowId = District.RowId;
            
            this.Errors = District.Errors;
        }
    }

    public class ShowingWarehouse_DistrictFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public IdFilter ProvinceId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public DistrictOrder OrderBy { get; set; }
    }
}