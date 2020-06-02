using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_MonitorStoreCheckerDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<MonitorStoreChecker_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class MonitorStoreChecker_MonitorStoreCheckerFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter CheckIn { get; set; }
        public IdFilter Checking { get; set; }
        public IdFilter Image { get; set; }
        public IdFilter SalesOrder { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MonitorStoreCheckerOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
