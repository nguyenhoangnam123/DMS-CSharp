using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.monitor_store_problems
{
    public class MonitorStoreProblemRoute : Root
    {
        public const string Master = Module + "/monitor-store-problem/monitor-store-problem-master";
        public const string Detail = Module + "/monitor-store-problem/monitor-store-problem-detail";
        private const string Default = Rpc + Module + "/monitor-store-problem";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListProblemStatus = Default + "/filter-list-problem-status";
        public const string FilterListProblemType = Default + "/filter-list-problem-type";
        public const string FilterListStore = Default + "/filter-list-store";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListOrganization = Default + "/single-list-organization";
        public const string SingleListProblemStatus = Default + "/single-list-problem-status";
        public const string SingleListProblemType = Default + "/single-list-problem-type";
        public const string SingleListStore = Default + "/single-list-store";

        public const string CountProblemHistory = Default + "/count-problem-history";
        public const string ListProblemHistory = Default + "/list-problem-history";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(MonitorStoreProblem_ProblemFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(MonitorStoreProblem_ProblemFilterDTO.AppUserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListOrganization, FilterListProblemStatus, FilterListProblemType, FilterListStore,
                CountProblemHistory, ListProblemHistory } },

            { "Sửa", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListOrganization, FilterListProblemStatus, FilterListProblemType, FilterListStore,
                Detail, Update,
                SingleListAppUser, SingleListOrganization, SingleListProblemStatus, SingleListProblemType, SingleListStore, } },

            { "Xoá", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListOrganization, FilterListProblemStatus, FilterListProblemType, FilterListStore,
                Detail, Delete,
                SingleListAppUser, SingleListOrganization, SingleListProblemStatus, SingleListProblemType, SingleListStore, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListOrganization, FilterListProblemStatus, FilterListProblemType, FilterListStore,
                CountProblemHistory, ListProblemHistory,
                BulkDelete } },

        };
    }
}
