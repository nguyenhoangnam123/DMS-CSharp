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
    public class NationTypeHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "NationType";

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
            List<EventMessage<NationType>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<NationType>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<NationType>> NationTypeEventMessages = await ListEventMessage<NationType>(context, SyncKey, RowIds);
            List<NationType> NationTypes = NationTypeEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<NationTypeDAO> NationTypeDAOs = NationTypes.Select(x => new NationTypeDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(NationTypeDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(NationTypeHandler));
            }
        }
    }
}
