using DMS.ABE.Common;
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
    public class ShowingItemHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(ShowingItemHandler);

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
            List<EventMessage<ShowingItem>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<ShowingItem>>>(json);
            List<long> ShowingItemIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
            await context.ShowingItem.Where(a => ShowingItemIds.Contains(a.Id)).UpdateFromQueryAsync(x => new ShowingItemDAO { Used = true });
        }
    }
}
