using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.e_route_change_request_content
{
    public class ERouteChangeRequestContent_ERouteChangeRequestDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long ERouteId { get; set; }
        
        public long CreatorId { get; set; }
        
        public long RequestStateId { get; set; }
        

        public ERouteChangeRequestContent_ERouteChangeRequestDTO() {}
        public ERouteChangeRequestContent_ERouteChangeRequestDTO(ERouteChangeRequest ERouteChangeRequest)
        {
            
            this.Id = ERouteChangeRequest.Id;
            
            this.ERouteId = ERouteChangeRequest.ERouteId;
            
            this.CreatorId = ERouteChangeRequest.CreatorId;
            
            this.RequestStateId = ERouteChangeRequest.RequestStateId;
            
            this.Errors = ERouteChangeRequest.Errors;
        }
    }

    public class ERouteChangeRequestContent_ERouteChangeRequestFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ERouteId { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public IdFilter RequestStateId { get; set; }
        
        public ERouteChangeRequestOrder OrderBy { get; set; }
    }
}