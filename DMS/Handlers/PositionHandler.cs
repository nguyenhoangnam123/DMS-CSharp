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
    public class PositionHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(Position);

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
            List<EventMessage<Position>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Position>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReviced);

            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Position>> PositionEventMessages = await ListEventMessage<Position>(context, SyncKey, RowIds);

            List<Position> Positions = new List<Position>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Position> EventMessage = PositionEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Positions.Add(EventMessage.Content);
            }
            try
            {
                List<PositionDAO> PositionDAOs = Positions.Select(x => new PositionDAO
                {
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    Name = x.Name,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                }).ToList();
                await context.BulkMergeAsync(PositionDAOs);
            }
            catch(Exception ex)
            {
                Log(ex, nameof(PositionHandler));
            }
        }
    }
}
