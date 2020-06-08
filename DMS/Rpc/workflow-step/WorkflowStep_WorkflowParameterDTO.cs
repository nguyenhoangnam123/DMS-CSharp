using Common;
using DMS.Entities;

namespace DMS.Rpc.workflow_step
{
    public class WorkflowStep_WorkflowParameterDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public WorkflowStep_WorkflowParameterDTO() { }
        public WorkflowStep_WorkflowParameterDTO(WorkflowParameter WorkflowParameter)
        {
            this.Id = WorkflowParameter.Id;
            this.Code = WorkflowParameter.Code;
            this.Name = WorkflowParameter.Name;
        }
    }
}
