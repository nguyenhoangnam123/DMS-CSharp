using DMS.Entities;
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
        private IUOW UOW;
        private const string SyncKey = "District.Sync";
        public WardHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Ward> Wards = JsonConvert.DeserializeObject<List<Ward>>(json);
                    UOW.WardRepository.BulkMerge(Wards);
                    break;
            }
            return true;
        }
    }
}
