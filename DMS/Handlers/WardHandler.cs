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
    public class WardHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "District.Sync";
        public WardHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<Ward>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Ward>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(Ward) },
                    };
                    List<EventMessage<Ward>> WardEventMessages = await UOW.EventMessageRepository.List<Ward>(EventMessageFilter);

                    List<Ward> Wards = new List<Ward>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<Ward> EventMessage = WardEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Wards.Add(EventMessage.Content);
                    }
                    context.Ward.BulkMerge(Wards);
                    break;
            }
            return true;
        }
    }
}
