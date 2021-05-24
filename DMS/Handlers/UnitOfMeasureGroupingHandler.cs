using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DMS.Enums;
using DMS.Repositories;

namespace DMS.Handlers
{
    public class UnitOfMeasureGroupingHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(UnitOfMeasureGrouping);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW , string json)
        {
            try
            {
                List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = JsonConvert.DeserializeObject<List<UnitOfMeasureGrouping>>(json);
                await UOW.UnitOfMeasureGroupingRepository.BulkMerge(UnitOfMeasureGroupings);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(UnitOfMeasureGroupingHandler));
            }

        }
    }
}
