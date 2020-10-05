using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_WorkflowStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public MobileSync_WorkflowStateDTO() { }
        public MobileSync_WorkflowStateDTO(WorkflowState WorkflowState)
        {
            this.Id = WorkflowState.Id;
            this.Code = WorkflowState.Code;
            this.Name = WorkflowState.Name;
        }
    }
}
