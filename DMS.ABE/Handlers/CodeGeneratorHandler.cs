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
            List<EventMessage<CodeGeneratorRule>> CodeGeneratorRuleEventMessages = JsonConvert.DeserializeObject<List<EventMessage<CodeGeneratorRule>>>(json);

            List<CodeGeneratorRule> CodeGeneratorRules = CodeGeneratorRuleEventMessages.Select(x => x.Content).ToList();
           
            try
            {
                List<long> Ids = CodeGeneratorRules.Select(x => x.Id).ToList();

                List<CodeGeneratorRuleDAO> CodeGeneratorRuleDAOs = await context.CodeGeneratorRule.Where(x => Ids.Contains(x.Id)).ToListAsync();
                List<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMappingDAOs = new List<CodeGeneratorRuleEntityComponentMappingDAO>();

                foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
                {
                    CodeGeneratorRuleDAO CodeGeneratorRuleDAO = CodeGeneratorRuleDAOs.Where(x => x.Id == CodeGeneratorRule.Id).FirstOrDefault();
                    if (CodeGeneratorRuleDAO == null)
                    {
                        CodeGeneratorRuleDAO = new CodeGeneratorRuleDAO();
                        CodeGeneratorRuleDAOs.Add(CodeGeneratorRuleDAO);
                    }
                    CodeGeneratorRuleDAO.Id = CodeGeneratorRule.Id;
                    CodeGeneratorRuleDAO.EntityTypeId = CodeGeneratorRule.EntityTypeId;
                    CodeGeneratorRuleDAO.StatusId = CodeGeneratorRule.StatusId;
                    CodeGeneratorRuleDAO.CreatedAt = CodeGeneratorRule.CreatedAt;
                    CodeGeneratorRuleDAO.UpdatedAt = CodeGeneratorRule.UpdatedAt;
                    CodeGeneratorRuleDAO.DeletedAt = CodeGeneratorRule.DeletedAt;
                    CodeGeneratorRuleDAO.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
                    CodeGeneratorRuleDAO.RowId = CodeGeneratorRule.RowId;
                    CodeGeneratorRuleDAO.Used = CodeGeneratorRule.Used;

                    foreach (CodeGeneratorRuleEntityComponentMapping CodeGeneratorRuleEntityComponentMapping in CodeGeneratorRule.CodeGeneratorRuleEntityComponentMappings)
                    {
                        CodeGeneratorRuleEntityComponentMappingDAO CodeGeneratorRuleEntityComponentMappingDAO = new CodeGeneratorRuleEntityComponentMappingDAO
                        {
                            CodeGeneratorRuleId = CodeGeneratorRule.Id,
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
                SystemLog(ex, nameof(CodeGeneratorRuleHandler));
            }
        }
    }
}
