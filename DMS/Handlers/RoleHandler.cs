using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MRole;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class RoleHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(Role);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<Role>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Role>>>(json);
            await SaveEventMessage(context, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Role>> RoleEventMessages = await ListEventMessage<Role>(context, RowIds);

            List<Role> Roles = new List<Role>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Role> EventMessage = RoleEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Roles.Add(EventMessage.Content);
            }
            try
            {
                List<AppUserRoleMappingDAO> AppUserRoleMappingDAOs = new List<AppUserRoleMappingDAO>();
                foreach (Role Role in Roles)
                {
                    RoleDAO RoleDAO = await context.Role.Where(r => r.Code == Role.Code).FirstOrDefaultAsync();
                    if (RoleDAO != null)
                    {
                        AppUserRoleMappingDAOs.AddRange(Role.AppUserRoleMappings.Select(ar => new AppUserRoleMappingDAO
                        {
                            RoleId = RoleDAO.Id,
                            AppUserId = ar.AppUserId,
                        }).ToList());
                    }
                }

                await context.BulkMergeAsync(AppUserRoleMappingDAOs);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
