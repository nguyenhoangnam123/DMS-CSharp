using Common;
using System.Collections.Generic;

namespace DMS.Rpc.mobile
{
    public class MobileRoute : Root
    {
        public const string Master = Module + "/mobile/store-checking-master";
        public const string Detail = Module + "/mobile/store-checking-detail";
        private const string Default = Rpc + Module + "/mobile";
        public const string CountStoreChecking = Default + "/count-store-checking";
        public const string ListStoreChecking = Default + "/list-store-checking";
        public const string GetStoreChecking = Default + "/get-store-checking";
        public const string UpdateStoreChecking = Default + "/update-store-checking";
        public const string CheckIn = Default + "/check-in";
        public const string CheckOut = Default + "/check-out";
        public const string CreateIndirectSalesOrder = Default + "/create-indirect-sales-order";
        public const string CreateProblem = Default + "/create-problem";
        public const string SaveImage = Default + "/save-image";

        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";

        public const string SingleListAlbum = Default + "/single-list-album";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListEroute = Default + "/single-list-e-route";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListProblemType = Default + "/single-list-problem-type";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListWard = Default + "/single-list-ward";

        public const string CountBanner = Default + "/count-banner";
        public const string ListBanner = Default + "/list-banner";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountStorePlanned = Default + "/count-store-planned";
        public const string ListStorePlanned = Default + "/list-store-planned";
        public const string CountStoreUnPlanned = Default + "/count-store-unplanned";
        public const string ListStoreUnPlanned = Default + "/list-store-unplanned";
        public const string CountProblem = Default + "/count-problem";
        public const string ListProblem = Default + "/list-problem";
        public const string CountSurvey = Default + "/count-survey";
        public const string ListSurvey = Default + "/list-survey";
        public const string CountStoreScouting = Default + "/count-store-scouting";
        public const string ListStoreScouting = Default + "/list-store-scouting";
        public const string GetSurveyForm = Default + "/get-survey-form";
        public const string SaveSurveyForm = Default + "/save-survey-form";
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking,
                FilterListAppUser, FilterListStore, } },
            { "Checkin", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking,
                FilterListAppUser, FilterListStore,
                Detail, CheckIn,  UpdateStoreChecking, CheckOut,
                CreateIndirectSalesOrder, CreateProblem, SaveImage, GetSurveyForm, SaveSurveyForm,
                CountItem, ListItem, CountStorePlanned, ListStorePlanned, CountStoreUnPlanned, ListStoreUnPlanned, CountProblem, ListProblem, CountSurvey, ListSurvey, CountStoreScouting, ListStoreScouting,
                SingleListAlbum, SingleListAppUser, SingleListStore, SingleListTaxType, SingleListUnitOfMeasure, SingleListProblemType, SingleListProvince, SingleListDistrict, SingleListWard, } },
        };
    }
}