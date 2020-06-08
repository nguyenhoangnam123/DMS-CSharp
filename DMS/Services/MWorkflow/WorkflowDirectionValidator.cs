using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MWorkflow
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
            FromStepNotExisted,
            FromStepEmpty,
            ToStepNotExisted,
            ToStepEmpty,
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
            if (WorkflowDirection.FromStepId == 0)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.FromStep), ErrorCode.FromStepEmpty);
            else
            {
                WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = WorkflowDirection.FromStepId },
                    WorkflowDefinitionId = new IdFilter { Equal = WorkflowDirection.WorkflowDefinitionId },
                    Selects = WorkflowStepSelect.Id
                };
                int count = await UOW.WorkflowStepRepository.Count(WorkflowStepFilter);
                if (count == 0)
                    WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.FromStep), ErrorCode.FromStepNotExisted);
            }
            if (WorkflowDirection.ToStepId == 0)
                WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.ToStep), ErrorCode.ToStepEmpty);
            else
            {
                WorkflowStepFilter WorkflowStepFilter = new WorkflowStepFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = WorkflowDirection.ToStepId },
                    WorkflowDefinitionId = new IdFilter { Equal = WorkflowDirection.WorkflowDefinitionId },
                    Selects = WorkflowStepSelect.Id
                };
                int count = await UOW.WorkflowStepRepository.Count(WorkflowStepFilter);
                if (count == 0)
                    WorkflowDirection.AddError(nameof(WorkflowDirectionValidator), nameof(WorkflowDirection.ToStep), ErrorCode.ToStepNotExisted);
            }
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

        public async Task<bool> Create(WorkflowDirection WorkflowDirection)
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
