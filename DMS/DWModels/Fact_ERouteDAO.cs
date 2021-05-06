using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_ERouteDAO
    {
        public long ERouteContentDayId { get; set; }
        public long ERouteContentId { get; set; }
        public long ERouteId { get; set; }
        public long StoreId { get; set; }
        public long OrderDay { get; set; }
        public bool Planned { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SaleEmployeeId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RealStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long ERouteTypeId { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public long CreatorId { get; set; }
    }
}
