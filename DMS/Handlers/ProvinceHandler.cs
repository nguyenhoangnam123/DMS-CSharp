using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ProvinceHandler
    {
        private DataContext context;
        private const string SyncKey = "Province.Sync";
        public ProvinceHandler(DataContext context)
        {
            this.context = context;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Province> Provinces = JsonConvert.DeserializeObject<List<Province>>(json);
                    context.Province.BulkMerge(Provinces);
                    break;
            }
            return true;
        }
    }
}
