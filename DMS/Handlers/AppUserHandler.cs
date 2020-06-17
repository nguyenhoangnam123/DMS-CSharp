using Common;
using DMS.Entities;
using DMS.Models;
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
                List<AppUserDAO> AppUserDAOs = AppUsers.Select(au => new AppUserDAO
                {
                    Address = au.Address,
                    Avatar = au.Avatar,
                    CreatedAt = au.CreatedAt,
                    UpdatedAt = au.UpdatedAt,
                    DeletedAt = au.DeletedAt,
                    Department = au.Department,
                    DisplayName = au.DisplayName,
                    Email = au.Email,
                    Id = au.Id,
                    OrganizationId = au.OrganizationId,
                    Phone = au.Phone,
                    PositionId = au.PositionId,
                    ProvinceId = au.ProvinceId,
                    RowId = au.RowId,
                    StatusId = au.StatusId,
                    Username = au.Username,
                    SexId = au.SexId,
                    Birthday = au.Birthday,
                    Longitude = au.Longitude,
                    Latitude = au.Latitude,
                }).ToList();
                await context.BulkMergeAsync(AppUserDAOs);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
