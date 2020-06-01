using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.workflow_direction
{
    public class WorkflowDirectionRoute : Root
    {
        public const string Master = Module + "/workflow-direction/workflow-direction-master";
        public const string Detail = Module + "/workflow-direction/workflow-direction-detail";
        private const string Default = Rpc + Module + "/workflow-direction";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Export = Default + "/export";

        public const string FilterListWorkflowStep = Default + "/filter-list-workflow-step";
        public const string FilterListWorkflowDefinition = Default + "/filter-list-workflow-definition";

        public const string SingleListWorkflowStep = Default + "/single-list-workflow-step";
        public const string SingleListWorkflowDefinition = Default + "/single-list-workflow-definition";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(WorkflowDirectionFilter.Id), FieldType.ID },
            { nameof(WorkflowDirectionFilter.WorkflowDefinitionId), FieldType.ID },
            { nameof(WorkflowDirectionFilter.FromStepId), FieldType.ID },
            { nameof(WorkflowDirectionFilter.ToStepId), FieldType.ID },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get,
                FilterListWorkflowStep, SingleListWorkflowDefinition, } },
            { "Thêm", new List<string> {
                Master, Count, List, Get,
                FilterListWorkflowStep, SingleListWorkflowDefinition,
                Detail, Create,
                SingleListWorkflowStep, SingleListWorkflowDefinition,  } },
            { "Sửa", new List<string> {
                Master, Count, List, Get,
                FilterListWorkflowStep, SingleListWorkflowDefinition,
                Detail, Update,
                SingleListWorkflowStep, SingleListWorkflowDefinition,  } },
            { "Xoá", new List<string> {
                Master, Count, List, Get,
                FilterListWorkflowStep, SingleListWorkflowDefinition,
                Detail, Delete,
                SingleListWorkflowStep, SingleListWorkflowDefinition,  } },
        };
    }
}
