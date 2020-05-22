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
            WorkflowDefinitionInUsed,
            FromStepIdNotExisted,
            ToStepIdNotExisted,
            SubjectMailForCreatorOverLength,
            SubjectMailForNextStepOverLength
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

        private async Task<bool> CanDelete(WorkflowDirection WorkflowDirection)
        {
            RequestWorkflowDefinitionMappingFilter RequestWorkflowDefinitionMappingFilter = new RequestWorkflowDefinitionMappingFilter
            {
                Skip = 0,
                Take = 1,
                WorkflowDefinitionId = new IdFilter { Equal = WorkflowDirection.WorkflowDefinitionId }
            };

            var count = await UOW.RequestWorkflowDefinitionMappingRepository.Count(RequestWorkflowDefinitionMappingFilter);
            if (count != 0)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.Id), ErrorCode.WorkflowDefinitionInUsed);
            return WorkflowDirection.IsValidated;
        }

        private async Task<bool> ValidateStep(WorkflowDirection WorkflowDirection)
        {
            WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { In = new List<long> { WorkflowDirection.FromStepId, WorkflowDirection.ToStepId } },
                Selects = WorkflowStepSelect.Id
            };

            List<long> WorkflowStepIds = (await UOW.WorkflowStepRepository.List(WorkflowStepFilter)).Select(x => x.Id).ToList();
            if(!WorkflowStepIds.Contains(WorkflowDirection.FromStepId))
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.FromStep), ErrorCode.FromStepIdNotExisted);
            if (!WorkflowStepIds.Contains(WorkflowDirection.ToStepId))
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.ToStep), ErrorCode.ToStepIdNotExisted);
            return WorkflowDirection.IsValidated;
        }

        private async Task<bool> ValidateSubjectMail(WorkflowDirection WorkflowDirection)
        {
            if (!string.IsNullOrWhiteSpace(WorkflowDirection.SubjectMailForCreator) && WorkflowDirection.SubjectMailForCreator.Length > 255)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.SubjectMailForCreator), ErrorCode.SubjectMailForCreatorOverLength);
            if (!string.IsNullOrWhiteSpace(WorkflowDirection.SubjectMailForNextStep) && WorkflowDirection.SubjectMailForNextStep.Length > 255)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.SubjectMailForNextStep), ErrorCode.SubjectMailForNextStepOverLength);
            return WorkflowDirection.IsValidated;
        }

        public async Task<bool>Create(WorkflowDirection WorkflowDirection)
        {
            await ValidateStep(WorkflowDirection);
            await ValidateSubjectMail(WorkflowDirection);
            return WorkflowDirection.IsValidated;
        }

        public async Task<bool> Update(WorkflowDirection WorkflowDirection)
        {
            if (await ValidateId(WorkflowDirection))
            {
                await ValidateStep(WorkflowDirection);
                await ValidateSubjectMail(WorkflowDirection);
            }
            return WorkflowDirection.IsValidated;
        }

        public async Task<bool> Delete(WorkflowDirection WorkflowDirection)
        {
            if (await ValidateId(WorkflowDirection))
            {
                await CanDelete(WorkflowDirection);
            }
            return WorkflowDirection.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<WorkflowDirection> WorkflowDirections)
        {
            foreach (WorkflowDirection WorkflowDirection in WorkflowDirections)
            {
                await Delete(WorkflowDirection);
            }
            return WorkflowDirections.All(st => st.IsValidated);
        }
        
        public async Task<bool> Import(List<WorkflowDirection> WorkflowDirections)
        {
            return true;
        }
    }
}
