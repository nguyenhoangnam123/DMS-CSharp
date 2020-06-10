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
    public class ItemHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(Item);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(context, content);
        }

        private async Task Used(DataContext context, string json)
        {
            List<EventMessage<Item>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Item>>>(json);
            List<long> ItemIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
            await context.Item.Where(a => ItemIds.Contains(a.Id)).UpdateFromQueryAsync(a => new ItemDAO { Used = true });
        }
    }
}
