using Common;
using DMS.Entities;

namespace DMS.Rpc.reports.report_statistic.report_statistic_store_scouting
{
    public class ReportStatisticStoreScouting_ProvinceDTO : DataDTO
    {

        public long Id { get; set; }

        public string Name { get; set; }

        public long? Priority { get; set; }

        public long StatusId { get; set; }


        public ReportStatisticStoreScouting_ProvinceDTO() { }
        public ReportStatisticStoreScouting_ProvinceDTO(Province Province)
        {

            this.Id = Province.Id;

            this.Name = Province.Name;

            this.Priority = Province.Priority;

            this.StatusId = Province.StatusId;

        }
    }

    public class ReportStatisticStoreScouting_ProvinceFilterDTO : FilterDTO
    {

        public IdFilter Id { get; set; }

        public StringFilter Name { get; set; }

        public LongFilter Priority { get; set; }

        public IdFilter StatusId { get; set; }

        public ProvinceOrder OrderBy { get; set; }
    }
}