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
            List<OrganizationDAO> OrganizationDAOs = await context.Organization.ToListAsync();
            var AppUsers = Organizations.Where(x => x.AppUsers != null).SelectMany(x => x.AppUsers).ToList();
            var AppUserIds = AppUsers.Select(x => x.Id).ToList();
            var AppUserDAOs = await context.AppUser.Where(x => AppUserIds.Contains(x.Id)).ToListAsync();

            try
            {
                foreach (var Organization in Organizations)
                {
                    OrganizationDAO OrganizationDAO = OrganizationDAOs.Where(x => x.Id == Organization.Id).FirstOrDefault();
                    if(OrganizationDAO == null)
                    {
                        OrganizationDAO = new OrganizationDAO
                        {
                            Id = Organization.Id,
                            Code = Organization.Code,
                            Name = Organization.Name,
                            Address = Organization.Address,
                            CreatedAt = Organization.CreatedAt,
                            UpdatedAt = Organization.UpdatedAt,
                            DeletedAt = Organization.DeletedAt,
                            Email = Organization.Email,
                            Level = Organization.Level,
                            ParentId = Organization.ParentId,
                            Path = Organization.Path,
                            Phone = Organization.Phone,
                            RowId = Organization.RowId,
                            StatusId = Organization.StatusId,
                            IsDisplay = true
                        };
                        OrganizationDAOs.Add(OrganizationDAO);
                    }
                    else
                    {
                        OrganizationDAO.Id = Organization.Id;
                        OrganizationDAO.Code = Organization.Code;
                        OrganizationDAO.Name = Organization.Name;
                        OrganizationDAO.Address = Organization.Address;
                        OrganizationDAO.CreatedAt = Organization.CreatedAt;
                        OrganizationDAO.UpdatedAt = Organization.UpdatedAt;
                        OrganizationDAO.DeletedAt = Organization.DeletedAt;
                        OrganizationDAO.Email = Organization.Email;
                        OrganizationDAO.Level = Organization.Level;
                        OrganizationDAO.ParentId = Organization.ParentId;
                        OrganizationDAO.Path = Organization.Path;
                        OrganizationDAO.Phone = Organization.Phone;
                        OrganizationDAO.RowId = Organization.RowId;
                        OrganizationDAO.StatusId = Organization.StatusId;
                    }
                }
                await context.Organization.BulkMergeAsync(OrganizationDAOs);

                foreach (var AppUserDAO in AppUserDAOs)
                {
                    AppUserDAO.OrganizationId = AppUsers.Where(x => x.Id == AppUserDAO.Id).Select(x => x.OrganizationId).FirstOrDefault();
                }
                await context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                SystemLog(ex, nameof(OrganizationHandler));
            }
        }
    }
}
