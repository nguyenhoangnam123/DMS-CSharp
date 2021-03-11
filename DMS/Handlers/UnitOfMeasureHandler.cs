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
    public class UnitOfMeasureHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(UnitOfMeasure);

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
            List<EventMessage<UnitOfMeasure>> UnitOfMeasureEventMessages = JsonConvert.DeserializeObject<List<EventMessage<UnitOfMeasure>>>(json);
            List<UnitOfMeasure> UnitOfMeasures = UnitOfMeasureEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<UnitOfMeasureDAO> UnitOfMeasureDAOs = UnitOfMeasures
                    .Select(x => new UnitOfMeasureDAO
                    {
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        DeletedAt = x.DeletedAt,
                        Id = x.Id,
                        Code = x.Code,
                        Name = x.Name,
                        Description = x.Description,
                        Used = x.Used,
                        RowId = x.RowId,
                        StatusId = x.StatusId
                    }).ToList();
                await context.BulkMergeAsync(UnitOfMeasureDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(UnitOfMeasureHandler));
            }
        }
    }
}
