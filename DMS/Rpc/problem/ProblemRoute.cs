using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.problem
{
    public class ProblemRoute : Root
    {
        public const string Master = Module + "/problem/problem-master";
        public const string Detail = Module + "/problem/problem-detail";
        private const string Default = Rpc + Module + "/problem";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/export-tempate";
        public const string BulkDelete = Default + "/bulk-delete";


        public const string FilterListAppUser = Default + "/filter-list-app-user";

        public const string FilterListProblemStatus = Default + "/filter-list-problem-status";

        public const string FilterListProblemType = Default + "/filter-list-problem-type";

        public const string FilterListStore = Default + "/filter-list-store";

        public const string FilterListStoreChecking = Default + "/filter-list-store-checking";

        public const string FilterListImage = Default + "/filter-list-image";



        public const string SingleListAppUser = Default + "/single-list-app-user";

        public const string SingleListProblemStatus = Default + "/single-list-problem-status";

        public const string SingleListProblemType = Default + "/single-list-problem-type";

        public const string SingleListStore = Default + "/single-list-store";

        public const string SingleListStoreChecking = Default + "/single-list-store-checking";

        public const string SingleListImage = Default + "/single-list-image";

        public const string CountImage = Default + "/count-image";
        public const string ListImage = Default + "/list-image";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ProblemFilter.Id), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.StoreCheckingId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.StoreId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.CreatorId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.ProblemTypeId), FieldTypeEnum.ID.Id },
            { nameof(ProblemFilter.ProblemStatusId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage, } },

            { "Thêm", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                Detail, Create,
                 SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, SingleListImage, } },

            { "Sửa", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                Detail, Update,
                 SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, SingleListImage, } },

            { "Xoá", new List<string> {
                Master, Count, List, Get,  FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                Detail, Delete,
                 SingleListAppUser, SingleListProblemStatus, SingleListProblemType, SingleListStore, SingleListStoreChecking, SingleListImage, } },

            { "Xoá nhiều", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                BulkDelete } },

            { "Xuất excel", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                Export } },

            { "Nhập excel", new List<string> {
                Master, Count, List, Get, FilterListAppUser, FilterListProblemStatus, FilterListProblemType, FilterListStore, FilterListStoreChecking, FilterListImage,
                ExportTemplate, Import } },
        };
    }
}
