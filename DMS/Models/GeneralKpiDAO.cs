using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class GeneralKpiDAO
    {
        public GeneralKpiDAO()
        {
            GeneralKpiCriteriaMappings = new HashSet<GeneralKpiCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long EmployeeId { get; set; }
        public long Year { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<GeneralKpiCriteriaMappingDAO> GeneralKpiCriteriaMappings { get; set; }
    }
}
