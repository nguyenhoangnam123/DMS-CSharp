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
    public class DistrictHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "District.Sync";
        public DistrictHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<District>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<District>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(District) },
                    };
                    List<EventMessage<District>> DistrictEventMessages = await UOW.EventMessageRepository.List<District>(EventMessageFilter);

                    List<District> Districts = new List<District>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<District> EventMessage = DistrictEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Districts.Add(EventMessage.Content);
                    }
                    context.District.BulkMerge(Districts);
                    break;
            }
            return true;
        }
    }
}
