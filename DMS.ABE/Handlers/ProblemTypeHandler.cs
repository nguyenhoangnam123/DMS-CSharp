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
    public class ProblemTypeHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(ProblemType);

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
            try
            {
                List<EventMessage<ProblemType>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<ProblemType>>>(json);
                List<long> ProblemTypeIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
                await context.ProblemType.Where(a => ProblemTypeIds.Contains(a.Id)).UpdateFromQueryAsync(a => new ProblemTypeDAO { Used = true });
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ProblemTypeHandler));
            }
        }
    }
}
