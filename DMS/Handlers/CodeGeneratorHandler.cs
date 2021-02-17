using DMS.Common;
using DMS.Entities;
using DMS.Models;
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
        public override async Task Handle(DataContext context, string routingKey, string content)
        {
            if (routingKey == SyncKey)
                await Sync(context, content);
        }

        private async Task Sync(DataContext context, string json)
        {
            List<EventMessage<CodeGeneratorRule>> EventMessageReviced = JsonConvert.DeserializeObject<List<EventMessage<CodeGeneratorRule>>>(json);
            await SaveEventMessage(context, SyncKey, EventMessageReviced);
            List<Guid> RowIds = EventMessageReviced.Select(a => a.RowId).Distinct().ToList();
            List<EventMessage<CodeGeneratorRule>> CodeGeneratorRuleEventMessages = await ListEventMessage<CodeGeneratorRule>(context, SyncKey, RowIds);

            List<CodeGeneratorRule> CodeGeneratorRules = new List<CodeGeneratorRule>();
            foreach (var RowId in RowIds)
            {
                EventMessage<CodeGeneratorRule> EventMessage = CodeGeneratorRuleEventMessages.Where(e => e.RowId == RowId).OrderByDescending(e => e.Time).FirstOrDefault();
                if (EventMessage != null)
                    CodeGeneratorRules.Add(EventMessage.Content);
            }
            try
            {
                List<long> Ids = CodeGeneratorRules.Select(x => x.Id).ToList();

                List<CodeGeneratorRuleDAO> CodeGeneratorRuleDAOs = await context.CodeGeneratorRule.Where(x => Ids.Contains(x.Id)).ToListAsync();
                List<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMappingDAOs = new List<CodeGeneratorRuleEntityComponentMappingDAO>();

                foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
                {
                    CodeGeneratorRuleDAO CodeGeneratorRuleDAO = CodeGeneratorRuleDAOs.Where(x => x.Id == CodeGeneratorRule.Id).FirstOrDefault();
                    CodeGeneratorRuleDAO.Id = CodeGeneratorRule.Id;
                    CodeGeneratorRuleDAO.EntityTypeId = CodeGeneratorRule.EntityTypeId;
                    CodeGeneratorRuleDAO.StatusId = CodeGeneratorRule.StatusId;
                    CodeGeneratorRuleDAO.CreatedAt = CodeGeneratorRule.CreatedAt;
                    CodeGeneratorRuleDAO.UpdatedAt = CodeGeneratorRule.UpdatedAt;
                    CodeGeneratorRuleDAO.DeletedAt = CodeGeneratorRule.DeletedAt;
                    CodeGeneratorRuleDAO.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
                    CodeGeneratorRuleDAO.RowId = CodeGeneratorRule.RowId;
                    CodeGeneratorRuleDAO.Used = CodeGeneratorRule.Used;
                    CodeGeneratorRuleDAOs.Add(CodeGeneratorRuleDAO);

                    foreach (CodeGeneratorRuleEntityComponentMapping CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
                    {
                        CodeGeneratorRuleEntityComponentMappingDAO CodeGeneratorRuleEntityComponentMappingDAO = new CodeGeneratorRuleEntityComponentMappingDAO
                        {
                            CodeGeneratorRuleId = CodeGeneratorRuleEntityComponentMapping.CodeGeneratorRuleId,
                            EntityComponentId = CodeGeneratorRuleEntityComponentMapping.EntityComponentId,
                        };
                        CodeGeneratorRuleEntityComponentMappingDAOs.Add(CodeGeneratorRuleEntityComponentMappingDAO);
                    }
                }    
                await context.BulkMergeAsync(CodeGeneratorRuleDAOs);
                await context.BulkMergeAsync(CodeGeneratorRuleEntityComponentMappingDAOs);
            }
            catch (Exception ex)
            {
                Log(ex, nameof(CodeGeneratorRuleHandler));
            }
        }
    }
}
