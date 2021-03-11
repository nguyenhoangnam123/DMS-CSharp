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
    public class ProvinceHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(Province);

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
            List<EventMessage<Province>> ProvinceEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Province>>>(json);
            List<Province> Provinces = ProvinceEventMessages.Select(x => x.Content).ToList();

            List<ProvinceDAO> ProvinceDAOs = Provinces.Select(x => new ProvinceDAO
            {
                Code = x.Code,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                Name = x.Name,
                Priority = x.Priority,
                RowId = x.RowId,
                StatusId = x.StatusId,
            }).ToList();
            await context.BulkMergeAsync(ProvinceDAOs);
        }
    }
}
