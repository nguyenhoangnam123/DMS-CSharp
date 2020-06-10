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
    public class ProductHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(Product);

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
            List<EventMessage<Product>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Product>>>(json);
            List<long> ProductIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
            await context.Product.Where(a => ProductIds.Contains(a.Id)).UpdateFromQueryAsync(a => new ProductDAO { Used = true });
        }
    }
}
