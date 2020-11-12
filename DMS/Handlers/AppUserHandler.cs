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
    public class AppUserHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => "AppUser";

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
            List<EventMessage<AppUser>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);
            await SaveEventMessage(context, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<AppUser>> AppUserEventMessages = await ListEventMessage<AppUser>(context, RowIds);

            List<AppUser> AppUsers = new List<AppUser>();
            foreach (var RowId in RowIds)
            {
                EventMessage<AppUser> EventMessage = AppUserEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    AppUsers.Add(EventMessage.Content);
            }
            try
            {
                List<long> Ids = AppUsers.Select(x => x.Id).ToList();
                List<AppUserDAO> AppUserDAOs = await context.AppUser.Where(x => Ids.Contains(x.Id)).ToListAsync();
                foreach(AppUser AppUser in AppUsers)
                {
                    AppUserDAO AppUserDAO = AppUserDAOs.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                    if(AppUserDAO == null)
                    {
                        AppUserDAO = new AppUserDAO
                        {
                            GPSUpdatedAt = DateTime.Now,
                        };
                        AppUserDAOs.Add(AppUserDAO);
                    }
                    AppUserDAO.Address = AppUser.Address;
                    AppUserDAO.Avatar = AppUser.Avatar;
                    AppUserDAO.CreatedAt = AppUser.CreatedAt;
                    AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                    AppUserDAO.DeletedAt = AppUser.DeletedAt;
                    AppUserDAO.Department = AppUser.Department;
                    AppUserDAO.DisplayName = AppUser.DisplayName;
                    AppUserDAO.Email = AppUser.Email;
                    AppUserDAO.Id = AppUser.Id;
                    AppUserDAO.OrganizationId = AppUser.OrganizationId;
                    AppUserDAO.Phone = AppUser.Phone;
                    AppUserDAO.PositionId = AppUser.PositionId;
                    AppUserDAO.ProvinceId = AppUser.ProvinceId;
                    AppUserDAO.RowId = AppUser.RowId;
                    AppUserDAO.StatusId = AppUser.StatusId;
                    AppUserDAO.Username = AppUser.Username;
                    AppUserDAO.SexId = AppUser.SexId;
                    AppUserDAO.Birthday = AppUser.Birthday;
                    AppUserDAO.Longitude = AppUser.Longitude;
                    AppUserDAO.Latitude = AppUser.Latitude;
                }    
                await context.BulkMergeAsync(AppUserDAOs);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
