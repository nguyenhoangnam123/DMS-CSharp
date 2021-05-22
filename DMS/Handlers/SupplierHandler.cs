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
    public class SupplierHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Supplier);

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
                List<Supplier> Suppliers = JsonConvert.DeserializeObject<List<Supplier>>(json);
                await UOW.SupplierRepository.BulkMerge(Suppliers);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(SupplierHandler));
            }
        }
    }
}
