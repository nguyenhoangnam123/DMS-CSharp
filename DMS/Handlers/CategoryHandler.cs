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
    public class CategoryHandler : Handler
    {
        private string SyncKey => Name + ".Sync";

        public override string Name => nameof(Category);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
            {
                await Sync(UOW, content);
            }
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<Category> Categorys = JsonConvert.DeserializeObject<List<Category>>(json);
                await UOW.CategoryRepository.BulkMerge(Categorys);
            }
            catch (Exception e)
            {
                SystemLog(e, nameof(CategoryHandler));
            }
        }
    }
}
