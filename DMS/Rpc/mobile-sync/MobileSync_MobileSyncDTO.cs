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
        public List<MobileSync_BannerSyncDTO> Banner { get; set; }
        public List<MobileSync_IndirectSalesOrderSyncDTO> IndirectSalesOrder { get; set; }
        public List<MobileSync_ProblemSyncDTO> Problem { get; set; }
        public List<MobileSync_ProductSyncDTO> Product { get; set; }
        public List<MobileSync_StoreScoutingSyncDTO> StoreScouting { get; set; }
        public List<MobileSync_StoreSyncDTO> Store { get; set; }
        public List<MobileSync_SurveySyncDTO> Survey { get; set; }
    }
}
