using DMS.Common;
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
        public MobileSync_WorkflowStepDTO(WorkflowStep WorkflowStep)
        {
            this.Id = WorkflowStep.Id;
            this.Code = WorkflowStep.Code;
            this.Name = WorkflowStep.Name;
            this.RoleId = WorkflowStep.RoleId;
            this.Role = WorkflowStep.Role == null ? null : new MobileSync_RoleDTO(WorkflowStep.Role);
        }
    }
}
