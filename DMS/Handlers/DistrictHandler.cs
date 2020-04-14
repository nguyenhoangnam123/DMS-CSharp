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
    public class DistrictHandler
    {
        private DataContext context;
        private const string SyncKey = "District.Sync";
        public DistrictHandler(DataContext context)
        {
            this.context = context;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<District> Districts = JsonConvert.DeserializeObject<List<District>>(json);
                    context.District.BulkMerge(Districts);
                    break;
            }
            return true;
        }
    }
}
