using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MWorkflow
{
    public interface IWorkflowDefinitionValidator : IServiceScoped
    {
        Task<bool> Create(WorkflowDefinition WorkflowDefinition);
        Task<bool> Update(WorkflowDefinition WorkflowDefinition);
        Task<bool> Delete(WorkflowDefinition WorkflowDefinition);
        Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions);
        Task<bool> Import(List<WorkflowDefinition> WorkflowDefinitions);
    }

    public class WorkflowDefinitionValidator : IWorkflowDefinitionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WorkflowDefinitionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(WorkflowDefinition WorkflowDefinition)
        {
            WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = WorkflowDefinition.Id },
                Selects = WorkflowDefinitionSelect.Id
            };

            int count = await UOW.WorkflowDefinitionRepository.Count(WorkflowDefinitionFilter);
            if (count == 0)
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(WorkflowDefinition WorkflowDefinition)
        {
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> Update(WorkflowDefinition WorkflowDefinition)
        {
            if (await ValidateId(WorkflowDefinition))
            {
            }
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> Delete(WorkflowDefinition WorkflowDefinition)
        {
            if (await ValidateId(WorkflowDefinition))
            {
            }
            return WorkflowDefinition.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions)
        {
            return true;
        }
        
        public async Task<bool> Import(List<WorkflowDefinition> WorkflowDefinitions)
        {
            return true;
        }
    }
}
