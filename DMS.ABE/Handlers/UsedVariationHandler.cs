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
    public class UsedVariationHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "UsedVariation";

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
            List<EventMessage<UsedVariation>> UsedVariationEventMessages = JsonConvert.DeserializeObject<List<EventMessage<UsedVariation>>>(json);
            List<UsedVariation> UsedVariations = UsedVariationEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<UsedVariationDAO> UsedVariationDAOs = UsedVariations.Select(x => new UsedVariationDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(UsedVariationDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(UsedVariationHandler));
            }
        }
    }
}
