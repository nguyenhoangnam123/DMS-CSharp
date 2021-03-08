using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_statistic_store_scouting
{
    public class ReportStatisticStoreScoutingRoute : Root
    {
        public const string Parent = Module + "/store-report";
        public const string Master = Module + "/store-report/statistic-store-scouting-report-master";

        private const string Default = Rpc + Module + "/statistic-store-scouting-report";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Total = Default + "/total";
        public const string Export = Default + "/export";

        public const string FilterListProvince = Default + "/filter-list-province";
        public const string FilterListDistrict = Default + "/filter-list-district";
        public const string FilterListWard = Default + "/filter-list-ward";

        public static Dictionary<string, long> Filters = new Dictionary<string, long>
        {
            { nameof(ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO.OrganizationId), FieldTypeEnum.ID.Id },
            { nameof(CurrentContext.UserId), FieldTypeEnum.ID.Id },
        };

        public static Dictionary<string, IEnumerable<string>> Action = new Dictionary<string, IEnumerable<string>>
        {
            { "Tìm kiếm", new List<string> {
                Parent,
                Master, Count, List, Total, Export,
                FilterListProvince, FilterListDistrict, FilterListWard  } },

        };
    }
}
