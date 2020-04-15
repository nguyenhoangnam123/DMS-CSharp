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
    public class OrganizationHandler
    {
        private DataContext context;
        private IUOW UOW;
        private const string SyncKey = "Organization.Sync";
        public OrganizationHandler(DataContext context, IUOW UOW)
        {
            this.context = context;
            this.UOW = UOW;
        }
        public async Task<bool> Handle(string routingKey, string json)
        {
            switch (routingKey)
            {
                case SyncKey:
                    List<EventMessage<Organization>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Organization>>>(json);
                    await UOW.EventMessageRepository.BulkMerge(EventMessageReviced);
                    List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();

                    EventMessageFilter EventMessageFilter = new EventMessageFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        RowId = new GuidFilter { In = RowIds },
                        EntityName = new StringFilter { Equal = nameof(Organization) },
                    };
                    List<EventMessage<Organization>> OrganizationEventMessages = await UOW.EventMessageRepository.List<Organization>(EventMessageFilter);

                    List<Organization> Organizations = new List<Organization>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<Organization> EventMessage = OrganizationEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Organizations.Add(EventMessage.Content);
                    }
                    context.Organization.BulkMerge(Organizations);
                    break;
            }
            return true;
        }
    }
}
