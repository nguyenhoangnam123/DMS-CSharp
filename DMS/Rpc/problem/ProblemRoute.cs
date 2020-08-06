using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.problem
{
    public class ProblemRoute : Root
    {
        public const string Parent = Module + "/problem";
        public const string Master = Module + "/problem/problem-master";
        public const string Detail = Module + "/problem/problem-detail/*";
        public const string Mobile = Module + ".problem.*";
        private const string Default = Rpc + Module + "/problem";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListProblemStatus = Default + "/filter-list-problem-status";
        public const string FilterListProblemType = Default + "/filter-list-problem-type";
        public const string FilterListStore = Default + "/filter-list-store";
        public const string FilterListStoreChecking = Default + "/filter-list-store-checking";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListProblemStatus = Default + "/single-list-problem-status";
        public const string SingleListProblemType = Default + "/single-list-problem-type";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreChecking = Default + "/single-list-store-checking";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProblemFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.StoreCheckingId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.StoreId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.AppUserId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.ProblemTypeId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.ProblemStatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, } },

            { "Thêm", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking,
                Detail, Create, SaveImage,
                SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, } },

            { "Sửa", new List<string> {
                Parent,
                Master, Count, List, Get, 
                FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking,
                Detail, Update, SaveImage,
                SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, } },

            { "Xoá", new List<string> {
                Parent,
                Master, Count, List, Get,  
                FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking,
                Detail, Delete,
                SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, } },

        };
    }
}
