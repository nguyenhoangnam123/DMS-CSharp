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
    public class WardHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(Ward);

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
            List<EventMessage<Ward>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Ward>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Ward>> WardEventMessages = await ListEventMessage<Ward>(context, SyncKey, RowIds);

            List<Ward> Wards = new List<Ward>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Ward> EventMessage = WardEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Wards.Add(EventMessage.Content);
            }
            List<WardDAO> WardDAOs = Wards.Select(x => new WardDAO
            {
                Code = x.Code,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                DistrictId = x.DistrictId,
                Id = x.Id,
                Name = x.Name,
                Priority = x.Priority,
                RowId = x.RowId,
                StatusId = x.StatusId,
            }).ToList();
            await context.BulkMergeAsync(WardDAOs);
        }
    }
}
