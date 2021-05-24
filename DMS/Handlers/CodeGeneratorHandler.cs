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
    public class CodeGeneratorRuleHandler : Handler
    {
        private string SyncKey => Name + ".Sync";
        public override string Name => nameof(CodeGeneratorRule);

        public override void QueueBind(IModel channel, string queue, string exchange)
        {
            channel.QueueBind(queue, exchange, $"{Name}.*", null);
        }
        public override async Task Handle(IUOW UOW, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(UOW, content);
        }

        private async Task Sync(IUOW UOW, string json)
        {
            try
            {
                List<CodeGeneratorRule> CodeGeneratorRules = JsonConvert.DeserializeObject<List<CodeGeneratorRule>>(json);
                await UOW.CodeGeneratorRuleRepository.BulkMerge(CodeGeneratorRules);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(CodeGeneratorRuleHandler));
            }
        }
    }
}
