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
                        Selects = EventMessageSelect.ALL,
                    };
                    List<EventMessage<Organization>> OrganizationEventMessages = await UOW.EventMessageRepository.List<Organization>(EventMessageFilter);

                    List<Organization> Organizations = new List<Organization>();
                    foreach (var RowId in RowIds)
                    {
                        EventMessage<Organization> EventMessage = OrganizationEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                        if (EventMessage != null)
                            Organizations.Add(EventMessage.Content);
                    }
                    List<OrganizationDAO> OrganizationDAOs = Organizations.Select(o => new OrganizationDAO
                    {
                        Address = o.Address,
                        Code = o.Address,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        DeletedAt = o.DeletedAt,
                        Email = o.Email,
                        Id = o.Id,
                        Level = o.Level,
                        Name = o.Name,
                        ParentId = o.ParentId,
                        Path = o.Path,
                        Phone = o.Phone,
                        RowId = o.RowId,
                        StatusId = o.StatusId,
                    }).ToList();
                    await context.Organization.BulkMergeAsync(OrganizationDAOs);
                    break;
            }
            return true;
        }
    }
}
