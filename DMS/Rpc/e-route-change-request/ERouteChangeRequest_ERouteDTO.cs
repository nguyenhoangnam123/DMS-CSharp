using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.e_route_change_request
{
    public class ERouteChangeRequest_ERouteDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long SaleEmployeeId { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public long? ERouteTypeId { get; set; }
        
        public long RequestStateId { get; set; }
        
        public long StatusId { get; set; }
        
        public long CreatorId { get; set; }
        

        public ERouteChangeRequest_ERouteDTO() {}
        public ERouteChangeRequest_ERouteDTO(ERoute ERoute)
        {
            
            this.Id = ERoute.Id;
            
            this.Code = ERoute.Code;
            
            this.Name = ERoute.Name;
            
            this.SaleEmployeeId = ERoute.SaleEmployeeId;
            
            this.StartDate = ERoute.StartDate;
            
            this.EndDate = ERoute.EndDate;
            
            this.ERouteTypeId = ERoute.ERouteTypeId;
            
            this.RequestStateId = ERoute.RequestStateId;
            
            this.StatusId = ERoute.StatusId;
            
            this.CreatorId = ERoute.CreatorId;
            
            this.Errors = ERoute.Errors;
        }
    }

    public class ERouteChangeRequest_ERouteFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter SaleEmployeeId { get; set; }
        
        public DateFilter StartDate { get; set; }
        
        public DateFilter EndDate { get; set; }
        
        public IdFilter ERouteTypeId { get; set; }
        
        public IdFilter RequestStateId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public ERouteOrder OrderBy { get; set; }
    }
}