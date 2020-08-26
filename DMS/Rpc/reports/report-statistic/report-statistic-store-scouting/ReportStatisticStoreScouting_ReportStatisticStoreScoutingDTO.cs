using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_statistic.report_statistic_store_scouting
{
    public class ReportStatisticStoreScouting_ReportStatisticStoreScoutingDTO : DataDTO
    {
        public long STT { get; set; }
        public string OfficalName { get; set; }
        internal long? ProvinceId {get;set;}
        internal long? DistrictId {get;set;}
        internal long? WardId {get;set;}
        public long StoreScoutingCounter => StoreScoutingIds.Count();
        public List<long> StoreScoutingIds { get; set; }
        public long StoreOpennedCounter => StoreOpennedIds.Count();
        internal List<long> StoreOpennedIds { get; set; }
        public long StoreScoutingUnOpen => StoreScoutingCounter - StoreOpennedCounter;
        public long StoreCounter => StoreIds.Count();
        internal List<long> StoreIds { get; set; }
        public decimal? StoreCoutingOpennedRate => StoreScoutingCounter == 0 ? null : (decimal?)((StoreOpennedCounter / StoreScoutingCounter)*100);
        public string eStoreCoutingOpennedRate => StoreCoutingOpennedRate.HasValue == false ? "" : $"{Math.Round(StoreCoutingOpennedRate.Value, 0)}%";
        public decimal? StoreCoutingRate => (StoreScoutingUnOpen + StoreCounter) == 0 ? null : (decimal?)((StoreScoutingUnOpen / (StoreScoutingUnOpen + StoreCounter))*100);
        public string eStoreCoutingRate => StoreCoutingRate.HasValue == false ? "" : $"{Math.Round(StoreCoutingRate.Value, 0)}%";
        public decimal? StoreRate => StoreCoutingRate.HasValue == false ? null : (decimal?)(100 - StoreCoutingRate.Value);
        public string eStoreRate => StoreRate.HasValue == false ? "" : $"{Math.Round(StoreRate.Value, 0)}%";
    }

    public class ReportStatisticStoreScouting_ReportStatisticStoreScoutingFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public DateFilter Date { get; set; }
        internal bool HasValue => (ProvinceId != null && ProvinceId.HasValue) ||
            (DistrictId != null && DistrictId.HasValue) ||
            (WardId != null && WardId.HasValue);
    }
}
