﻿using DMS.Common;
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
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<Brand> Brands = JsonConvert.DeserializeObject<List<Brand>>(json);
                await UOW.BrandRepository.BulkMerge(Brands);
            }
            catch (Exception e)
            {
                Log(e, nameof(BrandHandler));
            }
        }
    }
}
