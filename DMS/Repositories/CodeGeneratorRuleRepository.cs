using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Enums;

namespace DMS.Repositories
{
    public interface ICodeGeneratorRuleRepository
    {
        Task<bool> BulkMerge(List<CodeGeneratorRule> CodeGeneratorRules);
    }
    public class CodeGeneratorRuleRepository : ICodeGeneratorRuleRepository
    {
        private DataContext DataContext;
        public CodeGeneratorRuleRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<bool> BulkMerge(List<CodeGeneratorRule> CodeGeneratorRules)
        {
            List<CodeGeneratorRuleDAO> CodeGeneratorRuleDAOs = new List<CodeGeneratorRuleDAO>();
            List<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMappingDAOs = new List<CodeGeneratorRuleEntityComponentMappingDAO>();

            foreach (CodeGeneratorRule CodeGeneratorRule in CodeGeneratorRules)
            {
                CodeGeneratorRuleDAO CodeGeneratorRuleDAO = new CodeGeneratorRuleDAO();
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
                        Sequence = CodeGeneratorRuleEntityComponentMapping.Sequence,
                        Value = CodeGeneratorRuleEntityComponentMapping.Value,
                        EntityComponentId = CodeGeneratorRuleEntityComponentMapping.EntityComponentId,
                    };
                    CodeGeneratorRuleEntityComponentMappingDAOs.Add(CodeGeneratorRuleEntityComponentMappingDAO);
                }
            }
            await DataContext.BulkMergeAsync(CodeGeneratorRuleDAOs);
            await DataContext.BulkMergeAsync(CodeGeneratorRuleEntityComponentMappingDAOs);
            return true;
        }
    }
}
