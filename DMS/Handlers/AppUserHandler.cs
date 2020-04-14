using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MAppUser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class AppUserHandler
    {
        private DataContext context;
        private const string SyncKey = "AppUser.Sync";
        public AppUserHandler(DataContext context)
        {
            this.context = context;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<AppUser> appUsers = JsonConvert.DeserializeObject<List<AppUser>>(json);
                    context.AppUser.BulkMerge(appUsers);
                    break;
            }
            return true;
        }
    }
}
