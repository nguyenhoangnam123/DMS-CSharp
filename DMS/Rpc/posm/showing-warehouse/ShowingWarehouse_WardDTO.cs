using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.showing_warehouse
{
    public class ShowingWarehouse_WardDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public long DistrictId { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        

        public ShowingWarehouse_WardDTO() {}
        public ShowingWarehouse_WardDTO(Ward Ward)
        {
            
            this.Id = Ward.Id;
            
            this.Code = Ward.Code;
            
            this.Name = Ward.Name;
            
            this.Priority = Ward.Priority;
            
            this.DistrictId = Ward.DistrictId;
            
            this.StatusId = Ward.StatusId;
            
            this.RowId = Ward.RowId;
            
            this.Errors = Ward.Errors;
        }
    }

    public class ShowingWarehouse_WardFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public IdFilter DistrictId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public WardOrder OrderBy { get; set; }
    }
}