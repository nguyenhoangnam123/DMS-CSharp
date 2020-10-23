using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DirectSalesOrder_RoleDTO() { }
        public DirectSalesOrder_RoleDTO(Role Role)
        {
            this.Id = Role.Id;
            this.Code = Role.Code;
            this.Name = Role.Name;
        }
    }
}
