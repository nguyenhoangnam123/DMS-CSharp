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
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<UnitOfMeasureGrouping>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<UnitOfMeasureGrouping>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<UnitOfMeasureGrouping>> UnitOfMeasureGroupingEventMessages = await ListEventMessage<UnitOfMeasureGrouping>(context, SyncKey, RowIds);

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = new List<UnitOfMeasureGrouping>();
            foreach (var RowId in RowIds)
            {
                EventMessage<UnitOfMeasureGrouping> EventMessage = UnitOfMeasureGroupingEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    UnitOfMeasureGroupings.Add(EventMessage.Content);
            }

            try
            {
                List<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupingDAOs = UnitOfMeasureGroupings.Select(x => new UnitOfMeasureGroupingDAO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Used = x.Used,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                    Description = x.Description,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                }).ToList();

                var Ids = UnitOfMeasureGroupings.Select(x => x.Id).ToList();
                await context.UnitOfMeasureGroupingContent.Where(x => Ids.Contains(x.UnitOfMeasureGroupingId)).DeleteFromQueryAsync();
                List<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentDAOs = UnitOfMeasureGroupings
                    .SelectMany(x => x.UnitOfMeasureGroupingContents.Select(y => new UnitOfMeasureGroupingContentDAO
                    {
                        Id = y.Id,
                        Factor = y.Factor,
                        RowId = y.RowId,
                        UnitOfMeasureId = y.UnitOfMeasureId,
                        UnitOfMeasureGroupingId = y.UnitOfMeasureGroupingId,
                    })).ToList();
                await context.BulkMergeAsync(UnitOfMeasureGroupingDAOs);
                await context.BulkMergeAsync(UnitOfMeasureGroupingContentDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(UnitOfMeasureGroupingHandler));
            }

        }
    }
}
