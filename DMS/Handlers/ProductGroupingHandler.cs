﻿using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DMS.Enums;

namespace DMS.Handlers
{
    public class ProductGroupingHandler : Handler
    {
        private string SyncKey => $"{Name}.Sync";
        public override string Name => nameof(ProductGrouping);

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
            List<EventMessage<ProductGrouping>> ProductGroupingEventMessages = JsonConvert.DeserializeObject<List<EventMessage<ProductGrouping>>>(json);
            List<ProductGrouping> ProductGroupings = ProductGroupingEventMessages.Select(x => x.Content).ToList();
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            List<ProductGroupingDAO> ProductGroupingInDB = await context.ProductGrouping.ToListAsync();
            try
            {
                List<ProductGroupingDAO> ProductGroupingDAOs = new List<ProductGroupingDAO>();
                foreach (ProductGrouping ProductGrouping in ProductGroupings)
                {
                    ProductGroupingDAO ProductGroupingDAO = ProductGroupingInDB.Where(x => x.Id == ProductGrouping.Id).FirstOrDefault();
                    if (ProductGroupingDAO == null)
                    {
                        ProductGroupingDAO = new ProductGroupingDAO();
                    }
                    ProductGroupingDAO.Id = ProductGrouping.Id;
                    ProductGroupingDAO.Code = ProductGrouping.Code;
                    ProductGroupingDAO.CreatedAt = ProductGrouping.CreatedAt;
                    ProductGroupingDAO.UpdatedAt = ProductGrouping.UpdatedAt;
                    ProductGroupingDAO.DeletedAt = ProductGrouping.DeletedAt;
                    ProductGroupingDAO.Id = ProductGrouping.Id;
                    ProductGroupingDAO.Name = ProductGrouping.Name;
                    ProductGroupingDAO.RowId = ProductGrouping.RowId;
                    ProductGroupingDAO.Description = ProductGrouping.Description;
                    ProductGroupingDAO.Level = ProductGrouping.Level;
                    ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
                    ProductGroupingDAO.Path = ProductGrouping.Path;
                    ProductGroupingDAOs.Add(ProductGroupingDAO);

                    // add product productgrouping mapping 
                    foreach (var ProductProductGroupingMapping in ProductGrouping.ProductProductGroupingMappings)
                    {
                        ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO
                        {
                            ProductId = ProductProductGroupingMapping.ProductId,
                            ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId,
                        };
                        ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                    }
                }
                List<long> Ids = ProductGroupings.Select(x => x.Id).ToList();

                await context.ProductProductGroupingMapping
                  .Where(x => Ids.Contains(x.ProductGroupingId))
                  .DeleteFromQueryAsync();

                await context.BulkMergeAsync(ProductGroupingDAOs);
                await context.BulkMergeAsync(ProductProductGroupingMappingDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ProductGroupingHandler));
            }

        }
    }
}
