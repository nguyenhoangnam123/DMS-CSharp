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
using DMS.Repositories;

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
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }
        public async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<Nation> Nations = JsonConvert.DeserializeObject<List<Nation>>(json);
                await UOW.NationRepository.BulkMerge(Nations);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(NationHandler));
            }
        }
    }
}
