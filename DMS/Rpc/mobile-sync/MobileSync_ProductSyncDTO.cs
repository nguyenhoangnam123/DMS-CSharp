using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_ProductSyncDTO
    {
        public List<MobileSync_ProductDTO> Created { get; set; }
        public List<MobileSync_ProductDTO> Updated { get; set; }
        public List<MobileSync_ProductDTO> Deleted { get; set; }
    }
}
