using Common;
using System.Collections.Generic;

namespace DMS.Rpc.workflow_definition
{
    public class WorkflowDefinitionRoute : Root
    {
        public const string Master = Module + "/workflow/workflow-definition-master";
        public const string Detail = Module + "/workflow/workflow-definition-detail";
        private const string Default = Rpc + Module + "/workflow-definition";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Export = Default + "/export";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListWorkflowType = Default + "/filter-list-workflow-type";
        public const string FilterListWorkflowDirection = Default + "/filter-list-workflow-direction";
        public const string FilterListWorkflowStep = Default + "/filter-list-workflow-step";
        public const string FilterListWorkflowParameter = Default + "/filter-list-workflow-parameter";
        public const string FilterListRole = Default + "/filter-list-role";
        public const string FilterListStatus = Default + "/filter-list-status";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListWorkflowType = Default + "/single-list-workflow-type";
        public const string SingleListWorkflowDirection = Default + "/single-list-workflow-direction";
        public const string SingleListWorkflowStep = Default + "/single-list-workflow-step";
        public const string SingleListWorkflowParameter = Default + "/single-list-workflow-parameter";
        public const string SingleListRole = Default + "/single-list-role";
        public const string SingleListStatus = Default + "/single-list-status";

        public const string CountDirection = Default + "/count-direction";
        public const string ListDirection = Default + "/list-direction";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };
        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListWorkflowType, FilterListWorkflowDirection, FilterListWorkflowStep, FilterListWorkflowParameter, FilterListRole, FilterListStatus } },
            { "Thêm", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListWorkflowType, FilterListWorkflowDirection, FilterListWorkflowStep, FilterListWorkflowParameter, FilterListRole, FilterListStatus,
                Detail, Create,
                SingleListAppUser, SingleListWorkflowType, SingleListWorkflowDirection, SingleListWorkflowStep, SingleListWorkflowParameter, SingleListRole, SingleListStatus } },
            { "Sửa", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListWorkflowType, FilterListWorkflowDirection, FilterListWorkflowStep, FilterListWorkflowParameter, FilterListRole, FilterListStatus,
                Detail, Update,
                SingleListAppUser, SingleListWorkflowType, SingleListWorkflowDirection, SingleListWorkflowStep, SingleListWorkflowParameter, SingleListRole, SingleListStatus } },
            { "Xoá", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListWorkflowType, FilterListWorkflowDirection, FilterListWorkflowStep, FilterListWorkflowParameter, FilterListRole, FilterListStatus,
                Detail, Delete,
                SingleListAppUser, SingleListWorkflowType, SingleListWorkflowDirection, SingleListWorkflowStep, SingleListWorkflowParameter, SingleListRole, SingleListStatus } },
        };
    }
}
