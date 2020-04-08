using DMS.Entities;
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
        private IUOW UOW;
        private const string SyncKey = "District.Sync";
        public DistrictHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<District> Districts = JsonConvert.DeserializeObject<List<District>>(json);
                    UOW.DistrictRepository.BulkMerge(Districts);
                    break;
            }
            return true;
        }
    }
}
