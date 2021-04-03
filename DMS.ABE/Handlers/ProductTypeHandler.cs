using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Handlers
{
    public class ProductTypeHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(ProductType);

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
            List<EventMessage<ProductType>> ProductTypeEventMessages = JsonConvert.DeserializeObject<List<EventMessage<ProductType>>>(json);
            List<ProductType> ProductTypes = ProductTypeEventMessages.Select(x => x.Content).ToList();

            List<ProductTypeDAO> ProductTypeInDB = await context.ProductType.ToListAsync();
            try
            {
                List<ProductTypeDAO> ProductTypeDAOs = new List<ProductTypeDAO>();
                foreach (var ProductType in ProductTypes)
                {
                    ProductTypeDAO ProductTypeDAO = ProductTypeInDB.Where(x => x.Id == ProductType.Id).FirstOrDefault();
                    if (ProductTypeDAO == null)
                    {
                        ProductTypeDAO = new ProductTypeDAO();
                    }
                    ProductTypeDAO.Id = ProductType.Id;
                    ProductTypeDAO.CreatedAt = ProductType.CreatedAt;
                    ProductTypeDAO.UpdatedAt = ProductType.UpdatedAt;
                    ProductTypeDAO.DeletedAt = ProductType.DeletedAt;
                    ProductTypeDAO.Id = ProductType.Id;
                    ProductTypeDAO.Code = ProductType.Code;
                    ProductTypeDAO.Name = ProductType.Name;
                    ProductTypeDAO.StatusId = ProductType.StatusId;
                    ProductTypeDAO.Description = ProductType.Description;
                    ProductTypeDAO.Used = ProductType.Used;
                    ProductTypeDAO.RowId = ProductType.RowId;
                    ProductTypeDAOs.Add(ProductTypeDAO);
                }
                await context.BulkMergeAsync(ProductTypeDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ProductTypeHandler));
            }
        }
    }
}
