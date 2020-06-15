using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class KpiYearDAO
    {
        public KpiYearDAO()
        {
            KpiGenerals = new HashSet<KpiGeneralDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KpiGeneralDAO> KpiGenerals { get; set; }
    }
}
