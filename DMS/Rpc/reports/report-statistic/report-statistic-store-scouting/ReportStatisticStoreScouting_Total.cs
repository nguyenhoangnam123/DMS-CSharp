using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_statistic.report_statistic_store_scouting
{
    public class ReportStatisticStoreScouting_TotalDTO : DataDTO
    {
        public string OfficalName { get; set; }
        public long StoreScoutingCounter { get; set; }
        public long StoreOpennedCounter { get; set; }
        public long StoreScoutingUnOpen => StoreScoutingCounter - StoreOpennedCounter;
        public long StoreCounter { get; set; }
        public decimal? StoreCoutingOpennedRate => StoreScoutingCounter == 0 ? null : (decimal?)((StoreOpennedCounter / StoreScoutingCounter)*100);
        public string eStoreCoutingOpennedRate => StoreCoutingOpennedRate.HasValue == false ? "" : $"{Math.Round(StoreCoutingOpennedRate.Value, 0)}%";
        public decimal? StoreCoutingRate => (StoreScoutingUnOpen + StoreCounter) == 0 ? null : (decimal?)((StoreScoutingUnOpen / (StoreScoutingUnOpen + StoreCounter))*100);
        public string eStoreCoutingRate => StoreCoutingRate.HasValue == false ? "" : $"{Math.Round(StoreCoutingRate.Value, 0)}%";
        public decimal? StoreRate => StoreCoutingRate.HasValue == false ? null : (decimal?)(100 - StoreCoutingRate.Value);
        public string eStoreRate => StoreRate.HasValue == false ? "" : $"{Math.Round(StoreRate.Value, 0)}%";
    }

}
