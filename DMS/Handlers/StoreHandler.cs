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
    public class StoreHandler : Handler
    {
        private string UsedKey => $"{Name}.Used";
        public override string Name => nameof(Store);

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
                List<Store> Store = JsonConvert.DeserializeObject<List<Store>>(json);
                List<long> Ids = Store.Select(a => a.Id).ToList();
                await UOW.StoreRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(StoreHandler));
            }
        }
    }
}
