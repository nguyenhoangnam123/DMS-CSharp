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
    public class ProvinceHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "Province.Sync";
        public ProvinceHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<Province>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Province>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(Province) },
                    };
                    List<EventMessage<Province>> ProvinceEventMessages = await UOW.EventMessageRepository.List<Province>(EventMessageFilter);

                    List<Province> Provinces = new List<Province>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<Province> EventMessage = ProvinceEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Provinces.Add(EventMessage.Content);
                    }
                    context.BulkMerge<ProvinceDAO>(Provinces);
                    break;
            }
            return true;
        }
    }
}
