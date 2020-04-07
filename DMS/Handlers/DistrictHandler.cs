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
        public DistrictHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            List<District> Districts = JsonConvert.DeserializeObject<List<District>>(json);
            UOW.DistrictRepository.BulkMerge(Districts);
            return true;
        }
    }
}
