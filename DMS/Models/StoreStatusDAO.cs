using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class StoreStatusDAO
    {
        public StoreStatusDAO()
        {
            StoreHistoryPreviousStoreStatuses = new HashSet<StoreHistoryDAO>();
            StoreHistoryStoreStatuses = new HashSet<StoreHistoryDAO>();
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StoreHistoryDAO> StoreHistoryPreviousStoreStatuses { get; set; }
        public virtual ICollection<StoreHistoryDAO> StoreHistoryStoreStatuses { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
