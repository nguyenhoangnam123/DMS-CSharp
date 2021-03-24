using DMS.Common;
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
    public class ShowingCategoryHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(ShowingCategoryHandler);

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
            List<EventMessage<ShowingCategory>> EventMessageRecieved = JsonConvert.DeserializeObject<List<EventMessage<ShowingCategory>>>(json);
            List<long> ShowingCategoryIds = EventMessageRecieved.Select(x => x.Content.Id).ToList();
            await context.ShowingCategory.Where(a => ShowingCategoryIds.Contains(a.Id)).UpdateFromQueryAsync(x => new ShowingCategoryDAO { Used = true });
        }
    }
}