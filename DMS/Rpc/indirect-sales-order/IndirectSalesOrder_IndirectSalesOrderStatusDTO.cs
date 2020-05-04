using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_IndirectSalesOrderStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public IndirectSalesOrder_IndirectSalesOrderStatusDTO() {}
        public IndirectSalesOrder_IndirectSalesOrderStatusDTO(IndirectSalesOrderStatus IndirectSalesOrderStatus)
        {
            
            this.Id = IndirectSalesOrderStatus.Id;
            
            this.Code = IndirectSalesOrderStatus.Code;
            
            this.Name = IndirectSalesOrderStatus.Name;
            
            this.Errors = IndirectSalesOrderStatus.Errors;
        }
    }

    public class IndirectSalesOrder_IndirectSalesOrderStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IndirectSalesOrderStatusOrder OrderBy { get; set; }
    }
}