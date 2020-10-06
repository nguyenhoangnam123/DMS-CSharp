using Common;
using DMS.Entities;
using DMS.Models;
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
        public MobileSync_WorkflowStateDTO(WorkflowStateDAO WorkflowStateDAO)
        {
            this.Id = WorkflowStateDAO.Id;
            this.Code = WorkflowStateDAO.Code;
            this.Name = WorkflowStateDAO.Name;
        }
    }
}
