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
            List<EventMessage<Position>> PositionEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Position>>>(json);

            List<Position> Positions = PositionEventMessages.Select(x => x.Content).ToList();
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
            catch (Exception ex)
            {
                SystemLog(ex, nameof(PositionHandler));
            }
        }
    }
}
