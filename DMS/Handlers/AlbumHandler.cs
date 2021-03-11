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
    public class AlbumHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(Album);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(context, content);
        }

        private async Task Used(DataContext context, string json)
        {
            List<EventMessage<Album>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Album>>>(json);
            List<Album> Albums = EventMessageReviced.Select(em => em.Content).ToList();
            IUOW UOW = new UOW(context);
            await UOW.AlbumRepository.BulkUsed(Albums);
        }
    }
}
