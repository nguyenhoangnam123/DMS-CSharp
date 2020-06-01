using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificKpiDAO
    {
        public ItemSpecificKpiDAO()
        {
            ItemSpecificKpiContents = new HashSet<ItemSpecificKpiContentDAO>();
            ItemSpecificKpiTotalItemSpecificCriteriaMappings = new HashSet<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long EmployeeId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual AppUserDAO Creator { get; set; }
        public virtual AppUserDAO Employee { get; set; }
        public virtual KpiPeriodDAO KpiPeriod { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ItemSpecificKpiContentDAO> ItemSpecificKpiContents { get; set; }
        public virtual ICollection<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }
    }
}
