﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_albums
{
    public class MonitorStoreAlbum_MonitorStoreAlbumDTO : DataDTO
    {
        public long Id { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        public DateTime? CheckInAt { get; set; }
        public DateTime? CheckOutAt { get; set; }
        public MonitorStoreAlbum_AppUserDTO SaleEmployee { get; set; }
        public MonitorStoreAlbum_StoreDTO Store { get; set; }
        public List<MonitorStoreAlbum_StoreCheckingImageMappingDTO> StoreCheckingImageMappings { get; set; }
    }

    public class MonitorStoreAlbum_MonitorStoreAlbumFilterDTO : FilterDTO
    {
        public IdFilter AlbumId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter CheckIn { get; set; }
        public IdFilter StoreId { get; set; }
    }
}
