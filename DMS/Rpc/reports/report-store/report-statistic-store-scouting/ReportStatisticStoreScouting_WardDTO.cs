using Common;
using DMS.Entities;

namespace DMS.Rpc.reports.report_store.report_statistic_store_scouting
{
    public class ReportStatisticStoreScouting_WardDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public long? Priority { get; set; }

        public long DistrictId { get; set; }

        public long StatusId { get; set; }


        public ReportStatisticStoreScouting_WardDTO() { }
        public ReportStatisticStoreScouting_WardDTO(Ward Ward)
        {

            this.Id = Ward.Id;

            this.Name = Ward.Name;

            this.Priority = Ward.Priority;

            this.DistrictId = Ward.DistrictId;

            this.StatusId = Ward.StatusId;

        }
    }

    public class ReportStatisticStoreScouting_WardFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public LongFilter Priority { get; set; }

        public IdFilter DistrictId { get; set; }

        public IdFilter StatusId { get; set; }

        public WardOrder OrderBy { get; set; }
    }
}