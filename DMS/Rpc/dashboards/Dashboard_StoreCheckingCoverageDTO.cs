using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_StoreCheckingCoverageDTO : DataDTO
    {
        public long Id { get; set; }
        
        public DateTime CheckOutAt { get; set; }
        public long StoreId { get; set; }
    }
}
