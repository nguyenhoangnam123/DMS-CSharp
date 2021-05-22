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
    public class StoreGroupingHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(StoreGrouping);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {

            if (routingKey == UsedKey)
                await Used(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            try
            {
                List<StoreGrouping> StoreGrouping = JsonConvert.DeserializeObject<List<StoreGrouping>>(json);
                List<long> Ids = StoreGrouping.Select(a => a.Id).ToList();
                await UOW.StoreGroupingRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(StoreGroupingHandler));
            }
        }
    }
}
