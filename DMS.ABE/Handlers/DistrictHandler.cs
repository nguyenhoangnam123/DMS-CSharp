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
    public class DistrictHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(District);

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
            List<EventMessage<District>> DistrictEventMessages = JsonConvert.DeserializeObject<List<EventMessage<District>>>(json);
            List<District> Districts = DistrictEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<DistrictDAO> DistrictDAOs = Districts.Select(x => new DistrictDAO
                {
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    ProvinceId = x.ProvinceId,
                    Id = x.Id,
                    Name = x.Name,
                    Priority = x.Priority,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                }).ToList();
                await context.BulkMergeAsync(DistrictDAOs);
            }
            catch(Exception ex)
            {
                SystemLog(ex, nameof(DistrictHandler));
            }
        }
    }
}
