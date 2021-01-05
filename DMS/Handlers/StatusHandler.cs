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
    public class StatusHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "Status";

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
            List<EventMessage<Status>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<Status>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Status>> StatusEventMessages = await ListEventMessage<Status>(context, SyncKey, RowIds);
            List<Status> Statuss = StatusEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<StatusDAO> StatusDAOs = Statuss.Select(x => new StatusDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(StatusDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(StatusHandler));
            }
        }
    }
}
