using Common;
using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class PositionHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "Position.Sync";
        public PositionHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<Position>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Position>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(Position) },
                        Selects = EventMessageSelect.ALL,
                    };
                    List<EventMessage<Position>> PositionEventMessages = await UOW.EventMessageRepository.List<Position>(EventMessageFilter);

                    List<Position> Positions = new List<Position>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<Position> EventMessage = PositionEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Positions.Add(EventMessage.Content);
                    }
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
                    break;
            }
            return true;
        }
    }
}
