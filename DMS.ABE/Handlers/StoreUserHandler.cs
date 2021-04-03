using DMS.ABE.Entities;
using DMS.ABE.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Handlers
{
    public class StoreUserHandler : Handler
    {
        private string UsedKey => "DMS.ABE." + Name + ".Used";
        public override string Name =>nameof(StoreUser);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"DMS.ABE.{Name}.*", null);
        }

        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(context, content);
        }

        private async Task Used(DataContext context, string json)
        {
            List<EventMessage<StoreUser>> StoreUserEventMessages = JsonConvert.DeserializeObject<List<EventMessage<StoreUser>>>(json);
            List<long> StoreUserIds = StoreUserEventMessages.Select(x => x.Content.Id).ToList();
            await context.StoreUser.Where(x => StoreUserIds.Contains(x.Id)).UpdateFromQueryAsync(u => new StoreUserDAO { Used = true });
        }
    }
}
