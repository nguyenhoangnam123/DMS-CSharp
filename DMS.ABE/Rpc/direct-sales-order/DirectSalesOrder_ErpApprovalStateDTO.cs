using DMS.ABE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.ABE.Entities;

namespace DMS.ABE.Rpc.direct_sales_order
{
    public class DirectSalesOrder_ErpApprovalStateDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public DirectSalesOrder_ErpApprovalStateDTO() {}
        public DirectSalesOrder_ErpApprovalStateDTO(ErpApprovalState ErpApprovalState)
        {
            
            this.Id = ErpApprovalState.Id;
            
            this.Code = ErpApprovalState.Code;
            
            this.Name = ErpApprovalState.Name;
            
            this.Errors = ErpApprovalState.Errors;
        }
    }

    public class DirectSalesOrder_ErpApprovalStateFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ErpApprovalStateOrder OrderBy { get; set; }
    }
}