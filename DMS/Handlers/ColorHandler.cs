using DMS.Common;
using DMS.Entities;
using DMS.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ColorHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => "Color";

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
            List<EventMessage<Color>> ColorEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Color>>>(json);
            List<Color> Colors = ColorEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<ColorDAO> ColorDAOs = Colors.Select(x => new ColorDAO
                {
                    Code = x.Code,
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                await context.BulkMergeAsync(ColorDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ColorHandler));
            }
        }
    }
}
