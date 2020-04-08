using DMS.Entities;
using DMS.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class OrganizationHandler
    {
        private IUOW UOW;
        private const string SyncKey = "Organization.Sync";
        public OrganizationHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Organization> Organizations = JsonConvert.DeserializeObject<List<Organization>>(json);
                    UOW.OrganizationRepository.BulkMerge(Organizations);
                    break;
            }
            return true;
        }
    }
}
