using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreUncheckingDAO
    {
        public long Id { get; set; }
        public long AppUserId { get; set; }
        public long StoreId { get; set; }
        public DateTime Date { get; set; }
    }
}
