using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MWorkflowStep
{
    public interface IWorkflowStepValidator : IServiceScoped
    {
        Task<bool> Create(WorkflowStep WorkflowStep);
        Task<bool> Update(WorkflowStep WorkflowStep);
        Task<bool> Delete(WorkflowStep WorkflowStep);
        Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps);
        Task<bool> Import(List<WorkflowStep> WorkflowSteps);
    }

    public class WorkflowStepValidator : IWorkflowStepValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WorkflowStepValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(WorkflowStep WorkflowStep)
        {
            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = WorkflowStep.Id },
                Selects = WorkflowStepSelect.Id
            };

            int count = await UOW.WorkflowStepRepository.Count(WorkflowStepFilter);
            if (count == 0)
                WorkflowStep.AddError(nameof(WorkflowStepValidator), nameof(WorkflowStep.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(WorkflowStep WorkflowStep)
        {
            return WorkflowStep.IsValidated;
        }

        public async Task<bool> Update(WorkflowStep WorkflowStep)
        {
            if (await ValidateId(WorkflowStep))
            {
            }
            return WorkflowStep.IsValidated;
        }

        public async Task<bool> Delete(WorkflowStep WorkflowStep)
        {
            if (await ValidateId(WorkflowStep))
            {
            }
            return WorkflowStep.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<WorkflowStep> WorkflowSteps)
        {
            return true;
        }
        
        public async Task<bool> Import(List<WorkflowStep> WorkflowSteps)
        {
            return true;
        }
    }
}
