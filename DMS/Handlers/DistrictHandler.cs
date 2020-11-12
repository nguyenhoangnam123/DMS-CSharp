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
            List<EventMessage<District>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<District>>>(json);
            await SaveEventMessage(context, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<District>> DistrictEventMessages = await ListEventMessage<District>(context, RowIds);
            List<District> Districts = new List<District>();
            foreach (var RowId in RowIds)
            {
                EventMessage<District> EventMessage = DistrictEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Districts.Add(EventMessage.Content);
            }
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
    }
}
