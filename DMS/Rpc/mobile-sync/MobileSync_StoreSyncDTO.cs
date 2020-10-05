using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_StoreSyncDTO
    {
        public List<MobileSync_StoreDTO> Created { get; set; }
        public List<MobileSync_StoreDTO> Updated { get; set; }
        public List<MobileSync_StoreDTO> Deleted { get; set; }
    }

    
}
