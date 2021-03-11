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
    public class StoreUserHandler : Handler
    {
        private string UsedKey => "DMS." + Name + ".Used";
        public override string Name =>nameof(StoreUser);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"DMS.{Name}.*", null);
        }

        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(context, content);
        }

        private async Task Used(DataContext context, string json)
        {
            List<StoreUser> StoreUsers = await GetDataFromMessage(context, json);
            List<long> StoreUserIds = StoreUsers.Select(x => x.Id).ToList();
            await context.StoreUser.Where(x => StoreUserIds.Contains(x.Id)).UpdateFromQueryAsync(u => new StoreUserDAO { Used = true });
        }

        private async Task<List<StoreUser>> GetDataFromMessage(DataContext context, string json)
        {
            List<StoreUser> StoreUsers = new List<StoreUser>();
            List<EventMessage<StoreUser>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<StoreUser>>>(json);
            await SaveEventMessage(context, UsedKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<StoreUser>> StoreUserEventMessages = await ListEventMessage<StoreUser>(context, UsedKey, RowIds);

            foreach (var RowId in RowIds)
            {
                EventMessage<StoreUser> EventMessage = StoreUserEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    StoreUsers.Add(EventMessage.Content);
            } // loc message theo rowId

            return StoreUsers;
        }
    }
}
