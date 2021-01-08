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
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(TaxType);

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
            List<EventMessage<TaxType>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<TaxType>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<TaxType>> TaxTypeEventMessages = await ListEventMessage<TaxType>(context, SyncKey, RowIds);
            List<TaxType> TaxTypes = TaxTypeEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<TaxTypeDAO> TaxTypeDAOs = TaxTypes
                    .Select(x => new TaxTypeDAO
                    {
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        DeletedAt = x.DeletedAt,
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        StatusId = x.StatusId,
                        Percentage = x.Percentage,
                        Used = x.Used,
                        RowId = x.RowId,

                    }).ToList();
                await context.BulkMergeAsync(TaxTypeDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(TaxTypeHandler));
            }
        }
    }
}
