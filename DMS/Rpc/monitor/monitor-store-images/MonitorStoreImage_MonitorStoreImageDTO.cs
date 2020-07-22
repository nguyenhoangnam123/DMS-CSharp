using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_images
{
    public class MonitorStoreImage_MonitorStoreImageDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<MonitorStoreImage_SaleEmployeeDTO> SaleEmployees { get; set; }
    }

    public class MonitorStoreImage_SaleEmployeeDTO : DataDTO
    {
        public long STT { get; set; }
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public List<MonitorStoreImage_StoreCheckingDTO> StoreCheckings { get; set; }
    }

    public class MonitorStoreImage_StoreCheckingDTO : DataDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public long StoreId { get; set; }
        public string StoreName { get; set; }
        public long ImageCounter { get; set; }
        public long SaleEmployeeId { get; set; }
    }

    public class MonitorStoreImage_MonitorStoreImageFilterDTO : FilterDTO 
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter CheckIn { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter HasImage { get; set; }
        public IdFilter HasOrder { get; set; }
        public List<MonitorStoreImage_MonitorStoreImageFilterDTO> OrFilters { get; set; }
    }
}
