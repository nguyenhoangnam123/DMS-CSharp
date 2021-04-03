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
    public class SexHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "Sex";

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
            List<EventMessage<Sex>> SexEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Sex>>>(json);
            List<Sex> Sexs = SexEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<SexDAO> SexDAOs = Sexs.Select(x => new SexDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(SexDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(SexHandler));
            }
        }
    }
}
