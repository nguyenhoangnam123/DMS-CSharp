using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_albums
{
    public class MonitorStoreAlbum_MonitorStoreAlbumDTO : DataDTO
    {
        public List<MonitorStoreAlbum_StoreCheckingImageMappingDTO> StoreCheckingImageMappings { get; set; }
    }

    public class MonitorStoreAlbum_MonitorStoreAlbumFilterDTO : FilterDTO
    {
        public IdFilter AlbumId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter CheckIn { get; set; }
        public IdFilter StoreId { get; set; }
        public List<MonitorStoreAlbum_MonitorStoreAlbumFilterDTO> OrFilters { get; set; }
    }
}
