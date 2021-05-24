using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class UsedVariationHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "UsedVariation";

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<UsedVariation> UsedVariations = JsonConvert.DeserializeObject<List<UsedVariation>>(json);
                await UOW.UsedVariationRepository.BulkMerge(UsedVariations);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(UsedVariationHandler));
            }
        }
    }
}
