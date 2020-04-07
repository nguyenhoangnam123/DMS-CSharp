using DMS.Entities;
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
        private IUOW UOW;
        public ProvinceHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            List<Province> Provinces = JsonConvert.DeserializeObject<List<Province>>(json);
            UOW.ProvinceRepository.BulkMerge(Provinces);
            return true;
        }
    }
}
