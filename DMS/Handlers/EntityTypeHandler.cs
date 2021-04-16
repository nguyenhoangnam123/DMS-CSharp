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
    public class EntityTypeHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(EntityType);

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
            List<EventMessage<EntityType>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<EntityType>>>(json);
            List<EntityType> EntityTypes = EventMessageReceived.Select(x => x.Content).ToList();
            try
            {
                List<EntityTypeDAO> EntityTypeDAOs = EntityTypes.Select(x => new EntityTypeDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(EntityTypeDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(EntityTypeHandler));
            }
        }
    }
}
