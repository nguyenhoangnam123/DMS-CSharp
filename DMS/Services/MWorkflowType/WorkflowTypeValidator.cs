using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MWorkflowType
{
    public interface IWorkflowTypeValidator : IServiceScoped
    {
        Task<bool> Create(WorkflowType WorkflowType);
        Task<bool> Update(WorkflowType WorkflowType);
        Task<bool> Delete(WorkflowType WorkflowType);
        Task<bool> BulkDelete(List<WorkflowType> WorkflowTypes);
        Task<bool> Import(List<WorkflowType> WorkflowTypes);
    }

    public class WorkflowTypeValidator : IWorkflowTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WorkflowTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(WorkflowType WorkflowType)
        {
            WorkflowTypeFilter WorkflowTypeFilter = new WorkflowTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = WorkflowType.Id },
                Selects = WorkflowTypeSelect.Id
            };

            int count = await UOW.WorkflowTypeRepository.Count(WorkflowTypeFilter);
            if (count == 0)
                WorkflowType.AddError(nameof(WorkflowTypeValidator), nameof(WorkflowType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(WorkflowType WorkflowType)
        {
            return WorkflowType.IsValidated;
        }

        public async Task<bool> Update(WorkflowType WorkflowType)
        {
            if (await ValidateId(WorkflowType))
            {
            }
            return WorkflowType.IsValidated;
        }

        public async Task<bool> Delete(WorkflowType WorkflowType)
        {
            if (await ValidateId(WorkflowType))
            {
            }
            return WorkflowType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<WorkflowType> WorkflowTypes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<WorkflowType> WorkflowTypes)
        {
            return true;
        }
    }
}
