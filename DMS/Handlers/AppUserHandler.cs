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
        private IUOW UOW;
        private const string SyncKey = "AppUser.Sync";
        public AppUserHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<AppUser>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(AppUser) },
                    };
                    List<EventMessage<AppUser>> AppUserEventMessages = await UOW.EventMessageRepository.List<AppUser>(EventMessageFilter);

                    List<AppUser> AppUsers = new List<AppUser>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<AppUser> EventMessage = AppUserEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            AppUsers.Add(EventMessage.Content);
                    }
                    context.BulkMerge<AppUserDAO>(AppUsers);
                    break;
            }
            return true;
        }
    }
}
