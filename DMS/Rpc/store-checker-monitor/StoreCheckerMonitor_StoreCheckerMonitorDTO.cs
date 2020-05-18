using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checker_monitor
{
    public class StoreCheckerMonitor_StoreCheckerMonitorDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public List<StoreCheckerMonitor_StoreCheckingDTO> StoreCheckings { get;set;}
    }

    public class StoreCheckerMonitor_StoreCheckerMonitorFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter CheckIn { get; set; }
        public IdFilter Checking { get; set; }
        public IdFilter Image { get; set; }
        public IdFilter SalesOrder { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreCheckerMonitorOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
