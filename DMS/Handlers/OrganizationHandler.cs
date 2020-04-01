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
        public OrganizationHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            List<Organization> Organizations = JsonConvert.DeserializeObject<List<Organization>>(json);

            return true;
        }
    }
}
