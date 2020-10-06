using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_WorkflowStepDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long RoleId { get; set; }
        public MobileSync_RoleDTO Role { get; set; }
        public MobileSync_WorkflowStepDTO() { }
        public MobileSync_WorkflowStepDTO(WorkflowStepDAO WorkflowStepDAO)
        {
            this.Id = WorkflowStepDAO.Id;
            this.Code = WorkflowStepDAO.Code;
            this.Name = WorkflowStepDAO.Name;
            this.RoleId = WorkflowStepDAO.RoleId;
            this.Role = WorkflowStepDAO.Role == null ? null : new MobileSync_RoleDTO(WorkflowStepDAO.Role);
        }
    }
}
