using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MWorkflowDirection
{
    public interface IWorkflowDirectionValidator : IServiceScoped
    {
        Task<bool> Create(WorkflowDirection WorkflowDirection);
        Task<bool> Update(WorkflowDirection WorkflowDirection);
        Task<bool> Delete(WorkflowDirection WorkflowDirection);
        Task<bool> BulkDelete(List<WorkflowDirection> WorkflowDirections);
        Task<bool> Import(List<WorkflowDirection> WorkflowDirections);
    }

    public class WorkflowDirectionValidator : IWorkflowDirectionValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WorkflowDirectionValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(WorkflowDirection WorkflowDirection)
        {
            WorkflowDirectionFilter WorkflowDirectionFilter = new WorkflowDirectionFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = WorkflowDirection.Id },
                Selects = WorkflowDirectionSelect.Id
            };

            int count = await UOW.WorkflowDirectionRepository.Count(WorkflowDirectionFilter);
            if (count == 0)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(WorkflowDirection WorkflowDirection)
        {
            return WorkflowDirection.IsValidated;
        }

        public async Task<bool> Update(WorkflowDirection WorkflowDirection)
        {
            if (await ValidateId(WorkflowDirection))
            {
            }
            return WorkflowDirection.IsValidated;
        }

        public async Task<bool> Delete(WorkflowDirection WorkflowDirection)
        {
            if (await ValidateId(WorkflowDirection))
            {
            }
            return WorkflowDirection.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<WorkflowDirection> WorkflowDirections)
        {
            return true;
        }
        
        public async Task<bool> Import(List<WorkflowDirection> WorkflowDirections)
        {
            return true;
        }
    }
}
