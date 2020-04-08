using Common;
using DMS.Entities;
using DMS.Enums;
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
        private IUOW UOW;
        private const string SyncKey = "AppUser.Sync";
        public AppUserHandler(IUOW UOW)
        {
            this.UOW = UOW;
        }
        public bool Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<AppUser> appUsers = JsonConvert.DeserializeObject<List<AppUser>>(json);
                    UOW.AppUserRepository.BulkMerge(appUsers);
                    break;
            }
            return true;
        }
    }
}
