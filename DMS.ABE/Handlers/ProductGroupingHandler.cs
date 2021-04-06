﻿using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using DMS.ABE.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DMS.ABE.Enums;

namespace DMS.ABE.Handlers
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
                }
                await context.BulkMergeAsync(ProductGroupingDAOs);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ProductGroupingHandler));
            }

        }
    }
}