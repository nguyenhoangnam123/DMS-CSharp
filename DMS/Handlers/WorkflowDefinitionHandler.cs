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
    public class WorkflowDefinitionHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(WorkflowDefinition);

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
            List<EventMessage<WorkflowDefinition>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<WorkflowDefinition>>>(json);
            List<long> WorkflowDefinitionIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
            await context.WorkflowDefinition.Where(a => WorkflowDefinitionIds.Contains(a.Id)).UpdateFromQueryAsync(a => new WorkflowDefinitionDAO { Used = true });
        }
    }
}
