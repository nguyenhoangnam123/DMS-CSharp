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
    public class RoleHandler : Handler
    {
        private string UsedKey => $"DMS.{Name}.Used";
        public override string Name => nameof(Role);

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
                List<Role> Role = JsonConvert.DeserializeObject<List<Role>>(json);
                List<long> Ids = Role.Select(a => a.Id).ToList();
                await UOW.RoleRepository.Used(Ids);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(RoleHandler));
            }
        }
    }
}
