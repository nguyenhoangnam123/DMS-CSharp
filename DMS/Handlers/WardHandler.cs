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
    public class WardHandler
    {
        private DataContext context;
        private const string SyncKey = "District.Sync";
        public WardHandler(DataContext context)
        {
            this.context = context;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Ward> Wards = JsonConvert.DeserializeObject<List<Ward>>(json);
                    context.Ward.BulkMerge(Wards);
                    break;
            }
            return true;
        }
    }
}
