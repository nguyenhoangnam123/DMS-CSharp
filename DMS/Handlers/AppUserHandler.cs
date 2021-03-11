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
            List<EventMessage<AppUser>> AppUserEventMessages = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);

            List<AppUser> AppUsers = AppUserEventMessages.Select(x => x.Content).ToList();
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
                SystemLog(ex, nameof(AppUserHandler));
            }
        }
    }
}
