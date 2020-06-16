using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_StoreCheckingImageMappingDTO : DataDTO
    {
        public long Count => StoreCheckingImageMappingHours.Sum(x => x.Counter);
        public List<Dashboard_StoreCheckingImageMappingHourDTO> StoreCheckingImageMappingHours { get; set; }
    }

    public class Dashboard_StoreCheckingImageMappingHourDTO : DataDTO
    {
        public long Hour { get; set; }
        public long Counter { get; set; }
    }
}
