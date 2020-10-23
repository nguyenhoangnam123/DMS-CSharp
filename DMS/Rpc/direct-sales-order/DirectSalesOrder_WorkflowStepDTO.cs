using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_WorkflowStepDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public IndirectSalesOrder_RoleDTO Role { get; set; }
        public DirectSalesOrder_WorkflowStepDTO() { }
        public DirectSalesOrder_WorkflowStepDTO(WorkflowStep WorkflowStep)
        {
            this.Id = WorkflowStep.Id;
            this.Code = WorkflowStep.Code;
            this.Name = WorkflowStep.Name;
            this.RoleId = WorkflowStep.RoleId;
            this.Role = WorkflowStep.Role == null ? null : new IndirectSalesOrder_RoleDTO(WorkflowStep.Role);
        }
    }
}
