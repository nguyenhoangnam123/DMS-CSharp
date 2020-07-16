using Common;
using DMS.Entities;
using System.Collections.Generic;

namespace DMS.Rpc.survey
{
    public class SurveyRoute : Root
    {
        public const string Master = Module + "/knowledge/survey/survey-master";
        public const string Detail = Module + "/knowledge/survey/survey-detail/*";
        public const string Mobile = Module + ".survey.*";
        private const string Default = Rpc + Module + "/survey";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Export = Default + "/export";
        public const string GetSurveyForm = Default + "/get-survey-form";
        public const string SaveSurveyForm = Default + "/save-survey-form";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStatus = Default + "/filter-list-status";
        public const string FilterListSurveyOptionType = Default + "/filter-list-survey-option-type";
        public const string FilterListSurveyQuestionType = Default + "/filter-list-survey-question-type";

        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSurveyOptionType = Default + "/single-list-survey-option-type";
        public const string SingleListSurveyQuestionType = Default + "/single-list-survey-question-type";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(SurveyFilter.Title), FieldTypeEnum.STRING.Id },
            { nameof(SurveyFilter.Description), FieldTypeEnum.STRING.Id },
            { nameof(SurveyFilter.StartAt), FieldTypeEnum.DATE.Id },
            { nameof(SurveyFilter.EndAt), FieldTypeEnum.DATE.Id },
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType, } },
            { "Thêm", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType,
                Detail, Create,
                SingleListAppUser, SingleListStatus, SingleListSurveyQuestionType, SingleListSurveyOptionType,  } },
            { "Sửa", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType,
                Detail, Update,
                SingleListAppUser, SingleListStatus, SingleListSurveyQuestionType, SingleListSurveyOptionType, } },
            { "Xoá", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType,
                Delete, } },
            { "Trả lời", new List<string> {
                Master, Count, List, Get,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType,
                GetSurveyForm, SaveSurveyForm } },
            { "Xuất Excel", new List<string> {
                Master, Count, List, Get, Export,
                FilterListAppUser, FilterListStatus, FilterListSurveyQuestionType,
                Delete, } },
        };
    }
}
