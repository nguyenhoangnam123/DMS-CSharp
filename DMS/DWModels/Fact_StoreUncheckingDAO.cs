using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Fact_StoreUncheckingDAO
    {
        public long StoreUnchecking { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime Date { get; set; }
    }
}
