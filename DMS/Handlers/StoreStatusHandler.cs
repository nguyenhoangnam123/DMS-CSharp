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
    public class StoreStatusHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "StoreStatus";

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
            List<EventMessage<StoreStatus>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<StoreStatus>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<StoreStatus>> StoreStatusEventMessages = await ListEventMessage<StoreStatus>(context, SyncKey, RowIds);
            List<StoreStatus> StoreStatuss = StoreStatusEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<StoreStatusDAO> StoreStatusDAOs = StoreStatuss.Select(x => new StoreStatusDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(StoreStatusDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(StoreStatusHandler));
            }
        }
    }
}
