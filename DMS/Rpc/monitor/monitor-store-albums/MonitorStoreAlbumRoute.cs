﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_albums
{
    public class MonitorStoreAlbumRoute : Root
    {
        public const string Master = Module + "/monitor-store-album/monitor-store-album-master";

        private const string Default = Rpc + Module + "/monitor-store-album";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Export = Default + "/export";

        public const string FilterListOrganization = Default + "/filter-list-organization";
        public const string FilterListAlbum = Default + "/filter-list-album";
        public const string FilterListAppUser = Default + "/filter-list-app-user";
        public const string FilterListStore = Default + "/filter-list-store";

        public static Dictionary<string, List<string>> Action = new Dictionary<string, List<string>>
        {
            { "Tìm kiếm", new List<string> {
                Master, List, Get, Export, FilterListOrganization, FilterListAppUser, FilterListStore } },
        };
    }
}