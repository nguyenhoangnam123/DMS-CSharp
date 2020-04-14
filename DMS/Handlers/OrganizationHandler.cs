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
    public class OrganizationHandler
    {
        private DataContext context;
        private const string SyncKey = "Organization.Sync";
        public OrganizationHandler(DataContext context)
        {
            this.context = context;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<Organization> Organizations = JsonConvert.DeserializeObject<List<Organization>>(json);
                    context.Organization.BulkMerge(Organizations);
                    break;
            }
            return true;
        }
    }
}
