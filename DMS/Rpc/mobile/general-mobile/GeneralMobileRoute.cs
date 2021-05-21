using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.mobile.general_mobile
{
    public class GeneralMobileRoute : Root
    {
        public const string Master = Module + "/general-mobile/store-checking-master";
        public const string Detail = Module + "/general-mobile/store-checking-detail";
        private const string Default = Rpc + Module + "/general-mobile";
        public const string CountStoreChecking = Default + "/count-store-checking";
        public const string ListStoreChecking = Default + "/list-store-checking";
        public const string GetStoreChecking = Default + "/get-store-checking";
        public const string UpdateStoreChecking = Default + "/update-store-checking";
        public const string UpdateStoreCheckingImage = Default + "/update-store-checking-image";
        public const string CheckIn = Default + "/check-in";
        public const string CheckOut = Default + "/check-out";
        public const string GetConfiguration = Default + "/get-configuration";

        public const string CountCompletedIndirectSalesOrder = Default + "/count-completed-indirect-sales-order";
        public const string ListCompletedIndirectSalesOrder = Default + "/list-completed-indirect-sales-order";
        public const string CountNewIndirectSalesOrder = Default + "/count-new-indirect-sales-order";
        public const string ListNewIndirectSalesOrder = Default + "/list-new-indirect-sales-order";

        // mobile route custom
        public const string CountIndirectSalesOrder = Default + "/count-indirect-sales-order";
        public const string ListIndirectSalesOrder = Default + "/list-indirect-sales-order";

        public const string CountDirectSalesOrder = Default + "/count-direct-sales-order";
        public const string ListDirectSalesOrder = Default + "/list-direct-sales-order";

        public const string GetIndirectSalesOrder = Default + "/get-indirect-sales-order";
        public const string CreateIndirectSalesOrder = Default + "/create-indirect-sales-order";
        public const string UpdateIndirectSalesOrder = Default + "/update-indirect-sales-order";
        public const string SendIndirectSalesOrder = Default + "/send-indirect-sales-order";
        public const string PreviewIndirectOrder = Default + "/preview-indirect-order";

        public const string CountCompletedDirectSalesOrder = Default + "/count-completed-direct-sales-order";
        public const string ListCompletedDirectSalesOrder = Default + "/list-completed-direct-sales-order";
        public const string CountNewDirectSalesOrder = Default + "/count-new-direct-sales-order";
        public const string ListNewDirectSalesOrder = Default + "/list-new-direct-sales-order";
        public const string GetDirectSalesOrder = Default + "/get-direct-sales-order";
        public const string CreateDirectSalesOrder = Default + "/create-direct-sales-order";
        public const string UpdateDirectSalesOrder = Default + "/update-direct-sales-order";
        public const string SendDirectSalesOrder = Default + "/send-direct-sales-order";

        public const string CreateProblem = Default + "/create-problem";
        public const string SaveImage = Default + "/save-image";
        public const string SaveImage64 = Default + "/save-image-64";
        public const string UpdateAlbum = Default + "/update-album";
        public const string GetNotification = Default + "/get-notification";
        public const string UpdateGPS = Default + "/update-gps";
        public const string PrintIndirectOrder = Default + "/print-indirect-order";
        public const string PrintDirectOrder = Default + "/print-direct-order";

        public const string StoreReport = Default + "/store-report";
        public const string StoreStatistic = Default + "/store-statistic";

        public const string SingleListAlbum = Default + "/single-list-album";
        public const string SingleListAppUser = Default + "/single-list-app-user";
        public const string SingleListEroute = Default + "/single-list-e-route";
        public const string SingleListStore = Default + "/single-list-store";
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListStoreStatus = Default + "/single-list-store-status";
        public const string SingleListStoreType = Default + "/single-list-store-type";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListProblemType = Default + "/single-list-problem-type";
        public const string SingleListStoreScoutingType = Default + "/single-list-store-scouting-type";
        public const string SingleListProvince = Default + "/single-list-province";
        public const string SingleListDistrict = Default + "/single-list-district";
        public const string SingleListWard = Default + "/single-list-ward";
        public const string SingleListTime = Default + "/single-list-time";
        public const string SingleListSalesOrderType = Default + "/single-list-sales-order-type";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListColor = Default + "/single-list-color";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListStoreCheckingStatus = Default + "/single-list-store-checking-status";
        public const string SingleListStoreDraftType = Default + "/single-list-store-draft-type";
        public const string SingleListCategory = Default + "/single-list-category";

        public const string CountBanner = Default + "/count-banner";
        public const string ListBanner = Default + "/list-banner";
        public const string GetBanner = Default + "/get-banner";
        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string ListItemDirectOrder = Default + "/list-item-direct-order";
        public const string GetItem = Default + "/get-item";
        public const string CountStore = Default + "/count-store";
        public const string ListStore = Default + "/list-store";
        public const string CountBuyerStore = Default + "/count-buyer-store";
        public const string ListBuyerStore = Default + "/list-buyer-store";
        public const string GetStore = Default + "/get-store";
        public const string CreateStore = Default + "/create-store";
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

        public const string ListRewardHistory = Default + "/list-reward";
        public const string CountRewardHistory = Default + "/count-reward";
        public const string GetRewardHistory = Default + "/get-reward";
        public const string CreateRewardHistory = Default + "/create-reward";
        public const string LuckyDraw = Default + "/lucky-draw";

        public const string CountProductGrouping = Default + "/count-product-grouping";
        public const string ListProductGrouping = Default + "/list-product-grouping";
        public const string CountBrand = Default + "/count-brand";
        public const string ListBrand = Default + "/list-brand";

        // appUsers for chatting
        public const string ListAppUser = Default + "/list-app-user";
        public const string CountAppUser = Default + "/count-app-user";

        // resize Image
        public const string GetCroppedImage = Default + "/get/{FileName}";
        public const string CroppedImage = Default + "/crop/{FileName}";
        public const string ResizeImage = Default + "/resize/{FileName}";

        // list path
        public const string ListPath = Default + "/list-path";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking, ListPath
                } },
            { "Checkin", new List<string> {
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking, CountProductGrouping, ListProductGrouping, CountBrand, ListBrand,
                Detail, CheckIn,  UpdateStoreChecking, UpdateStoreCheckingImage, CheckOut, PrintIndirectOrder, PreviewIndirectOrder, StoreReport, StoreStatistic, PrintDirectOrder,
                CreateIndirectSalesOrder, CreateProblem, SaveImage, GetSurveyForm, SaveSurveyForm, CreateDirectSalesOrder,
                CountItem, ListItem, ListItemDirectOrder, CountStorePlanned, ListStorePlanned, CountStoreUnPlanned, ListStoreUnPlanned, CountStoreInScope, ListStoreInScope, CountProblem, ListProblem, CountSurvey, ListSurvey, CountStoreScouting, ListStoreScouting,
                SingleListAlbum, SingleListAppUser, SingleListStore, SingleListStoreStatus, SingleListTaxType, SingleListUnitOfMeasure, SingleListProblemType, SingleListStoreScoutingType, SingleListProvince, SingleListDistrict, SingleListWard, SingleListStoreDraftType,
                SingleListSalesOrderType, SingleListCategory,
                ListPath
            } },
            { "Quay thưởng", new List<string>{
                Master, CountStoreChecking, ListStoreChecking, GetStoreChecking,
                Detail, ListRewardHistory,  CountRewardHistory, GetRewardHistory, LuckyDraw, CreateRewardHistory,
                CountItem, ListItem, CountStorePlanned, ListStorePlanned, CountStoreUnPlanned, ListStoreUnPlanned, CountStoreInScope, ListStoreInScope, CountProblem, ListProblem, CountSurvey, ListSurvey, CountStoreScouting, ListStoreScouting,
                SingleListAlbum, SingleListAppUser, SingleListStore, SingleListStoreStatus, SingleListTaxType, SingleListUnitOfMeasure, SingleListProblemType, SingleListStoreScoutingType, SingleListProvince, SingleListDistrict, SingleListWard, SingleListStoreDraftType,
                SingleListSalesOrderType, SingleListCategory,
                ListPath
            } },
            {"Trò chuyện", new List<string>{
                ListPath,
                ListAppUser, CountAppUser
            } }
        };
    }
}
