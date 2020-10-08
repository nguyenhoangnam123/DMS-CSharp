using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_MobileSyncDTO
    {
        public MobileSync_ChangeDTO Changes { get; set; }

        public DateTime Timestamp { get; set; }
    }

    public class MobileSync_ChangeDTO
    {
        public MobileSync_BannerSyncDTO Banner { get; set; }
        public MobileSync_IndirectSalesOrderSyncDTO IndirectSalesOrder { get; set; }
        public MobileSync_ProblemSyncDTO Problem { get; set; }
        public MobileSync_ProductSyncDTO Product { get; set; }
        public MobileSync_ItemSyncDTO Item { get; set; }
        public MobileSync_StoreScoutingSyncDTO StoreScouting { get; set; }
        public MobileSync_StoreSyncDTO Store { get; set; }
        public MobileSync_SurveySyncDTO Survey { get; set; }
    }
}
