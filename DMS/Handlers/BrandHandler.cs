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
    public class BrandHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Brand);

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
            try
            {
                List<EventMessage<Brand>> BrandEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Brand>>>(json);
                List<Brand> Brands = BrandEventMessages.Select(x => x.Content).ToList();
                IUOW UOW = new UOW(context);
                await UOW.BrandRepository.BulkMerge(Brands);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(BrandHandler));
            }
        }
    }
}
