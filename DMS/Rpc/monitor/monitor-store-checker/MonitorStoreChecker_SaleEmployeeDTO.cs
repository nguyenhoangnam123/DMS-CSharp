using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public List<MonitorStoreChecker_StoreCheckingDTO> StoreCheckings { get; set; }
    }
}
