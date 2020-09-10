using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.indirect_sales_order
{
    public class IndirectSalesOrder_RoleDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public IndirectSalesOrder_RoleDTO() { }
        public IndirectSalesOrder_RoleDTO(Role Role)
        {
            this.Id = Role.Id;
            this.Code = Role.Code;
            this.Name = Role.Name;
        }
    }
}
