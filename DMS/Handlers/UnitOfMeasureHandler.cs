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
    public class UnitOfMeasureHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(UnitOfMeasure);

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
                List<UnitOfMeasure> UnitOfMeasures = JsonConvert.DeserializeObject<List<UnitOfMeasure>>(json);
                await UOW.UnitOfMeasureRepository.BulkMerge(UnitOfMeasures);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(UnitOfMeasureHandler));
            }
        }
    }
}
