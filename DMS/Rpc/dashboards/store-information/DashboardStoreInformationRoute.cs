using DMS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.store_information
{
    [DisplayName("Dashboard thông tin điểm bán")]
    public class DashboardStoreInformationRoute : Root
    {
        public const string Parent = Module + "/dashboards";
        public const string Master = Module + "/dashboards/store-information";
        private const string Default = Rpc + Module + "/dashboards/store-information";

        public const string StoreCounter = Default + "/store-counter";
        public const string BrandStatistic = Default + "/brand-statistics";
        public const string BrandUnStatistic = Default + "/brand-un-statistics";
        public const string StoreCoverage = Default + "/store-coverage";
        public const string ProductGroupingStatistic = Default + "/product-grouping-statistics";
        public const string TopBrand = Default + "/top-brand";

        public const string ExportBrandStatistic = Default + "/export-brand-statistics";
        public const string ExportBrandUnStatistic = Default + "/export-brand-un-statistics";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListBrand = Default + "/filter-list-brand";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListProvince = Default + "/filter-list-province";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(DashboardStoreInformation_BrandStatisticsFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Hiển thị", new List<string> {
                Parent,
                Master,
                StoreCounter, BrandStatistic, BrandUnStatistic, StoreCoverage, ProductGroupingStatistic, TopBrand,
                FilterListOrganization, FilterListBrand, FilterListDistrict, FilterListProvince
            } },

            { "Xuất Excel", new List<string> {
                Parent,
                Master,
                StoreCounter, BrandStatistic, BrandUnStatistic, StoreCoverage, ProductGroupingStatistic, TopBrand,
                FilterListOrganization, FilterListBrand, FilterListDistrict, FilterListProvince,
                ExportBrandStatistic, ExportBrandUnStatistic
            } },
        };
    }
}
