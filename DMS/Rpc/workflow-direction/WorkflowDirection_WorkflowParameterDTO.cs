using Common;
using DMS.Entities;

namespace DMS.Rpc.workflow_direction
{
    public class WorkflowDirection_WorkflowParameterDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long WorkflowParameterTypeId { get; set; }

        public WorkflowDirection_WorkflowParameterDTO() { }
        public WorkflowDirection_WorkflowParameterDTO(WorkflowParameter WorkflowParameter)
        {
            this.Id = WorkflowParameter.Id;
            this.Code = WorkflowParameter.Code;
            this.Name = WorkflowParameter.Name;
            this.WorkflowParameterTypeId = WorkflowParameter.WorkflowParameterTypeId;
        }
    }

    public class WorkflowDirection_WorkflowParameterFilterDTO : DataDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
    }
}
