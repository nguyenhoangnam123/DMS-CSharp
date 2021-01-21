using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class NationHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(Nation);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }
        public async Task Sync(DataContext context, string json)
        {
            List<EventMessage<Nation>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<Nation>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Nation>> NationEventMessages = await ListEventMessage<Nation>(context, SyncKey, RowIds);
            List<Nation> Nations = NationEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<NationDAO> NationDAOs = Nations.Select(x => new NationDAO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Used = x.Used,
                    Priority = x.Priority,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                }).ToList();
                await context.BulkMergeAsync(NationDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(NationHandler));
            }
        }
    }
}
