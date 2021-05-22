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
    public class StoreUserHandler : Handler
    {
        private string UsedKey => "DMS." + Name + ".Used";
        public override string Name =>nameof(StoreUser);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"DMS.{Name}.*", null);
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
                List<StoreUser> StoreUser = JsonConvert.DeserializeObject<List<StoreUser>>(json);
                List<long> Ids = StoreUser.Select(a => a.Id).ToList();
                await UOW.StoreUserRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(StoreUserHandler));
            }
        }
    }
}
