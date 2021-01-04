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
    public class CategoryHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync" ;
        private string UsedKey => $"DMS.{Name}.Used";
        public override string Name => nameof(Category);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
            channel.QueueBind(queue, exchange, $"DMS.{Name}.*", null);
        }
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<Category>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Category>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<Category>> CategoryEventMessages = await ListEventMessage<Category>(context, SyncKey, RowIds);

            List<Category> Categorys = new List<Category>();
            foreach (var RowId in RowIds)
            {
                EventMessage<Category> EventMessage = CategoryEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    Categorys.Add(EventMessage.Content);
            }
            try
            {
                List<CategoryDAO> CategoryDAOs = Categorys.Select(x => new CategoryDAO
                {
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Id = x.Id,
                    Name = x.Name,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                    ImageId = x.ImageId,
                    Level = x.Level,
                    ParentId = x.ParentId,
                    Path = x.Path,
                }).ToList();
                await context.BulkMergeAsync(CategoryDAOs);
            }
            catch(Exception ex)
            {
                Log(ex, nameof(CategoryHandler));
            }
        }
    }
}
