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
    public class BrandHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Brand);

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
            List<EventMessage<Brand>> BrandEventMessages = JsonConvert.DeserializeObject<List<EventMessage<Brand>>>(json);
            List<Brand> Brands = BrandEventMessages.Select(x => x.Content).ToList();
            try
            {
                List<BrandDAO> BrandDAOs = Brands.Select(x => new BrandDAO
                {
                    Code = x.Code,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    DeletedAt = x.DeletedAt,
                    Used = x.Used,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    RowId = x.RowId,
                    StatusId = x.StatusId,
                }).ToList();
                await context.BulkMergeAsync(BrandDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(BrandHandler));
            }
        }
    }
}
