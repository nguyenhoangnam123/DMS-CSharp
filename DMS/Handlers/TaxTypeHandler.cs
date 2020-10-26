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
    public class TaxTypeHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(TaxType);

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
            List<EventMessage<TaxType>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<TaxType>>>(json);
            List<long> TaxTypeIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
            await context.TaxType.Where(a => TaxTypeIds.Contains(a.Id)).UpdateFromQueryAsync(a => new TaxTypeDAO { Used = true });
        }
    }
}
