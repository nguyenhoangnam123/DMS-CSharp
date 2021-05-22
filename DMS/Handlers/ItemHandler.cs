using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Handlers
{
    public class ItemHandler : Handler
    {
        private string UsedKey => Name + ".Used";
        public override string Name => nameof(Item);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == UsedKey)
                await Used(UOW, content);
        }

        private async Task Used(IUOW UOW, string json)
        {
            //List<EventMessage<Item>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<Item>>>(json);
            //List<long> ItemIds = EventMessageReviced.Select(em => em.Content.Id).ToList();
            //await context.Item.Where(a => ItemIds.Contains(a.Id)).UpdateFromQueryAsync(a => new ItemDAO { Used = true });
            //List<long> ProductIds = await context.Item.Where(i => ItemIds.Contains(i.Id)).Select(i => i.ProductId).Distinct().ToListAsync();
            //await context.Product.Where(a => ProductIds.Contains(a.Id)).UpdateFromQueryAsync(a => new ProductDAO { Used = true });
            try
            {
                List<Item> Items = JsonConvert.DeserializeObject<List<Item>>(json);

                List<long> ItemIds = Items.Select(a => a.Id).ToList();
                List<long> ProductIds = Items.Where(i => ItemIds.Contains(i.Id))
                    .Select(i => i.ProductId)
                    .Distinct().ToList();

                await UOW.ItemRepository.Used(ItemIds);
                await UOW.ProductRepository.Used(ProductIds);
            }
            catch (Exception ex)
            {
                SystemLog(ex, nameof(ItemHandler));
            }
        }
    }
}
