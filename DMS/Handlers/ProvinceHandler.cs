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
        private const string SyncKey = "Province.Sync";
        public ProvinceHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Province> Provinces = JsonConvert.DeserializeObject<List<Province>>(json);
                    UOW.ProvinceRepository.BulkMerge(Provinces);
                    break;
            }
            return true;
        }
    }
}
