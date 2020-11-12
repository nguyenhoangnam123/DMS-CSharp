﻿using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_AlbumDTO : DataDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public MonitorStoreChecker_AlbumDTO() { }
        public MonitorStoreChecker_AlbumDTO(Album Album)
        {
            this.Id = Album.Id;
            this.Name = Album.Name;
            this.StatusId = Album.StatusId;
        }
    }

    public class MonitorStoreChecker_AlbumFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public AlbumOrder OrderBy { get; set; }
    }
}
