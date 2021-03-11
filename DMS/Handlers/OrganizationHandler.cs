using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class OrganizationHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(Organization);

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
            List<EventMessage<Organization>> OrganizationEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Organization>>>(json);

            List<Organization> Organizations = OrganizationEventMessages.Select(x => x.Content).ToList();

            var AppUsers = Organizations.Where(x => x.AppUsers != null).SelectMany(x => x.AppUsers).ToList();
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();
            var AppUserDAOs = await context.AppUser.Where(x => AppUserIds.Contains(x.Id)).ToListAsync();

            try
            {
                List<OrganizationDAO> OrganizationDAOs = Organizations.Select(o => new OrganizationDAO
                {
                    Id = o.Id,
                    Code = o.Code,
                    Name = o.Name,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    DeletedAt = o.DeletedAt,
                    Email = o.Email,
                    Level = o.Level,
                    ParentId = o.ParentId,
                    Path = o.Path,
                    Phone = o.Phone,
                    RowId = o.RowId,
                    StatusId = o.StatusId,
                }).ToList();
                await context.Organization.BulkMergeAsync(OrganizationDAOs);

                foreach (var AppUserDAO in AppUserDAOs)
                {
                    AppUserDAO.OrganizationId = AppUsers.Where(x => x.Id == AppUserDAO.Id).Select(x => x.OrganizationId).FirstOrDefault();
                }
                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Log(ex, nameof(OrganizationHandler));
            }
        }
    }
}
