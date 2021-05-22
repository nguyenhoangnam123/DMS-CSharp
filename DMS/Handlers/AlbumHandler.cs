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
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            try
            {
                List<Album> Album = JsonConvert.DeserializeObject<List<Album>>(json);
                List<long> Ids = Album.Select(a => a.Id).ToList();
                await UOW.AlbumRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(AlbumHandler));
            }
        }
    }
}
