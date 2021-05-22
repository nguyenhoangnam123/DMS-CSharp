using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ProductTypeHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(ProductType);

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
                List<ProductType> ProductTypes = JsonConvert.DeserializeObject<List<ProductType>>(json);
                await UOW.ProductTypeRepository.BulkMerge(ProductTypes);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ProductTypeHandler));
            }
        }
    }
}
