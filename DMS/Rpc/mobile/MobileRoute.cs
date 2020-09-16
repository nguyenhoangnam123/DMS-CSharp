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
        public const string CountIndirectSalesOrder = Default + "/count-indirect-sales-order";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";
        public const string GetIndirectSalesOrder = Default + "/get-indirect-sales-order";
        public const string CreateIndirectSalesOrder = Default + "/create-indirect-sales-order";
        public const string UpdateIndirectSalesOrder = Default + "/update-indirect-sales-order";
        public const string CreateProblem = Default + "/create-problem";
        public const string SaveImage = Default + "/save-image";
        public const string SaveImage64 = Default + "/save-image-64";
        public const string UpdateAlbum = Default + "/update-album";
        public const string GetNotification = Default + "/get-notification";
        public const string UpdateGPS = Default + "/update-gps";

        public const string SingleListAlbum = Default + "/single-list-album";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListEroute = Default + "/single-list-e-route";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListProblemType = Default + "/single-list-problem-type";
        public const string SingleListStoreScoutingType = Default + "/single-list-store-scouting-type";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListWard = Default + "/single-list-ward";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListColor = Default + "/single-list-color";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListStoreCheckingStatus = Default + "/single-list-store-checking-status";

        public const string CountBanner = Default + "/count-banner";
        public const string ListBanner = Default + "/list-banner";
        public const string GetBanner = Default + "/get-banner";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string GetItem = Default + "/get-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountBuyerStore = Default + "/count-buyer-store";
        public const string ListBuyerStore = Default + "/list-buyer-store";
        public const string GetStore = Default + "/get-store";
        public const string UpdateStore = Default + "/update-store";
        public const string CountStorePlanned = Default + "/count-store-planned";
        public const string ListStorePlanned = Default + "/list-store-planned";
        public const string CountStoreUnPlanned = Default + "/count-store-unplanned";
        public const string ListStoreUnPlanned = Default + "/list-store-unplanned";
        public const string CountStoreInScope = Default + "/count-store-in-scope";
        public const string ListStoreInScope = Default + "/list-store-in-scope";
        public const string CountProblem = Default + "/count-problem";
        public const string ListProblem = Default + "/list-problem";
        public const string GetProblem = Default + "/get-problem";
        public const string CountSurvey = Default + "/count-survey";
        public const string ListSurvey = Default + "/list-survey";
        public const string GetSurveyForm = Default + "/get-survey-form";
        public const string SaveSurveyForm = Default + "/save-survey-form";

        public const string CountStoreScouting = Default + "/count-store-scouting";
        public const string ListStoreScouting = Default + "/list-store-scouting";
        public const string GetStoreScouting = Default + "/get-store-scouting";
        public const string CreateStoreScouting = Default + "/create-store-scouting";
        public const string UpdateStoreScouting = Default + "/update-store-scouting";
        public const string DeleteStoreScouting = Default + "/delete-store-scouting";

      
        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking,
                } },
            { "Checkin", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking,
                Detail, CheckIn,  UpdateStoreChecking, CheckOut,
                CreateIndirectSalesOrder, CreateProblem, SaveImage, GetSurveyForm, SaveSurveyForm,
                CountItem, ListItem, CountStorePlanned, ListStorePlanned, CountStoreUnPlanned, ListStoreUnPlanned, CountStoreInScope, ListStoreInScope, CountProblem, ListProblem, CountSurvey, ListSurvey, CountStoreScouting, ListStoreScouting,
                SingleListAlbum, SingleListAppUser, SingleListStore, SingleListTaxType, SingleListUnitOfMeasure, SingleListProblemType, SingleListStoreScoutingType, SingleListProvince, SingleListDistrict, SingleListWard, } },
        };
    }
}
