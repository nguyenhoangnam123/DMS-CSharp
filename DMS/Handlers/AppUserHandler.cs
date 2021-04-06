using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
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
        private string UpdateGPSKey => Name + ".UpdateGPS";
        public override string Name => "AppUser";

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
            if (routingKey == UpdateGPSKey)
                await UpdateGPS(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<AppUser>> AppUserEventMessages = JsonConvert.DeserializeObject<List<EventMessage<AppUser>>>(json);

            List<AppUser> AppUsers = AppUserEventMessages.Select(x => x.Content).ToList();
            IUOW UOW = new UOW(context);
            try
            {
                List<long> Ids = AppUsers.Select(x => x.Id).ToList();
                List<AppUser> oldAppUsers = await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Selects = AppUserSelect.Id | AppUserSelect.GPSUpdatedAt,
                    Skip = 0,
                    Take = int.MaxValue,
                    Id = new IdFilter { In = Ids }
                });
                foreach(AppUser AppUser in AppUsers)
                {
                    AppUser old = oldAppUsers.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                    if (old == null)
                        AppUser.GPSUpdatedAt = DateTime.Now;
                    else
                        AppUser.GPSUpdatedAt = old.GPSUpdatedAt;
                }
                await UOW.AppUserRepository.BulkMerge(AppUsers);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(AppUserHandler));
            }
        }

        private async Task UpdateGPS(DataContext context, string json)
        {
            EventMessage<AppUser> AppUserEventMessage = JsonConvert.DeserializeObject<EventMessage<AppUser>>(json);

            AppUser AppUser = AppUserEventMessage.Content;
            IUOW UOW = new UOW(context);
            try
            {
                await UOW.AppUserRepository.SimpleUpdate(AppUser);
                //await DataContext.AppUser.Where(x => x.Id == AppUser.Id).UpdateFromQueryAsync(x => new AppUserDAO
                //{
                //    Latitude = AppUser.Latitude,
                //    Longitude = AppUser.Longitude,
                //    GPSUpdatedAt = StaticParams.DateTimeNow,
                //});
                //List<long> Ids = AppUsers.Select(x => x.Id).ToList();
                //List<AppUser> oldAppUsers = await UOW.AppUserRepository.List(new AppUserFilter
                //{
                //    Selects = AppUserSelect.Id | AppUserSelect.GPSUpdatedAt,
                //    Skip = 0,
                //    Take = int.MaxValue,
                //    Id = new IdFilter { In = Ids }
                //});
                //foreach (AppUser AppUser in AppUsers)
                //{
                //    AppUser old = oldAppUsers.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                //    if (old == null)
                //        AppUser.GPSUpdatedAt = DateTime.Now;
                //    else
                //        AppUser.GPSUpdatedAt = old.GPSUpdatedAt;
                //}
                //await UOW.AppUserRepository.BulkMerge(AppUsers);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(AppUserHandler));
            }
        }
    }
}
