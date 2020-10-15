using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.e_route
{
    public class ERoute_WorkflowStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ERoute_WorkflowStateDTO() { }
        public ERoute_WorkflowStateDTO(WorkflowState WorkflowState)
        {
            this.Id = WorkflowState.Id;
            this.Code = WorkflowState.Code;
            this.Name = WorkflowState.Name;
        }
    }
}
