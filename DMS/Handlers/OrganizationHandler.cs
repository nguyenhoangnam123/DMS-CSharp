﻿using DMS.Common;
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
    public class OrganizationHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(Organization);

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
            List<EventMessage<Organization>> EventMessageReceived = JsonConvert.DeserializeObject<List<EventMessage<Organization>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReceived);
            List<Guid> RowIds = EventMessageReceived.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Organization>> OrganizationEventMessages = await ListEventMessage<Organization>(context, SyncKey, RowIds);

            List<Organization> Organizations = new List<Organization>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Organization> EventMessage = OrganizationEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Organizations.Add(EventMessage.Content);
            }
            try
            {
                List<OrganizationDAO> OrganizationDAOs = Organizations.Select(o => new OrganizationDAO
                {
                    Id = o.Id,
                    Code = o.Code,
                    Name = o.Name,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    DeletedAt = o.DeletedAt,
                    Email = o.Email,
                    Level = o.Level,
                    ParentId = o.ParentId,
                    Path = o.Path,
                    Phone = o.Phone,
                    RowId = o.RowId,
                    StatusId = o.StatusId,
                }).ToList();
                await context.Organization.BulkMergeAsync(OrganizationDAOs);
            }
            catch(Exception ex)
            {
                Log(ex, nameof(OrganizationHandler));
            }
        }
    }
}
