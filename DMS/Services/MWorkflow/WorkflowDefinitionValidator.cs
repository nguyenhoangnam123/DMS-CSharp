using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            WorkflowDefinitionInUsed,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            WorkflowTypeNotExisted,
            WorkflowTypeEmpty,
            StatusNotExisted
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

        private async Task<bool> ValidateCode(WorkflowDefinition WorkflowDefinition)
        {
            if (string.IsNullOrWhiteSpace(WorkflowDefinition.Code))
            {
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = WorkflowDefinition.Code;
                if (WorkflowDefinition.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(WorkflowDefinition.Code))
                {
                    WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                WorkflowDefinitionFilter WorkflowDefinitionFilter = new WorkflowDefinitionFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = WorkflowDefinition.Id },
                    Code = new StringFilter { Equal = WorkflowDefinition.Code },
                    Selects = WorkflowDefinitionSelect.Code
                };

                int count = await UOW.WorkflowDefinitionRepository.Count(WorkflowDefinitionFilter);
                if (count != 0)
                    WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Code), ErrorCode.CodeExisted);
            }

            return WorkflowDefinition.IsValidated;
        }

        private async Task<bool> ValidateName(WorkflowDefinition WorkflowDefinition)
        {
            if (string.IsNullOrWhiteSpace(WorkflowDefinition.Name))
            {
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Name), ErrorCode.NameEmpty);
            }
            else if (WorkflowDefinition.Name.Length > 255)
            {
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Name), ErrorCode.NameOverLength);
            }
            return WorkflowDefinition.IsValidated;
        }

        private async Task<bool> ValidateWorkflowType(WorkflowDefinition WorkflowDefinition)
        {
            if (WorkflowDefinition.WorkflowTypeId == 0)
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.WorkflowType), ErrorCode.WorkflowTypeEmpty);
            else if (WorkflowDefinition.WorkflowTypeId != WorkflowTypeEnum.ORDER.Id && WorkflowDefinition.WorkflowTypeId != WorkflowTypeEnum.PRODUCT.Id &&
                WorkflowDefinition.WorkflowTypeId != WorkflowTypeEnum.ROUTE.Id && WorkflowDefinition.WorkflowTypeId != WorkflowTypeEnum.STORE.Id)
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.WorkflowType), ErrorCode.WorkflowTypeNotExisted);
            return WorkflowDefinition.IsValidated;
        }

        private async Task<bool> ValidateStatusId(WorkflowDefinition WorkflowDefinition)
        {
            if (StatusEnum.ACTIVE.Id != WorkflowDefinition.StatusId && StatusEnum.INACTIVE.Id != WorkflowDefinition.StatusId)
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Status), ErrorCode.StatusNotExisted);
            return WorkflowDefinition.IsValidated;
        }

        private async Task<bool> WorkflowDefinitionInUsed(WorkflowDefinition WorkflowDefinition)
        {
            RequestWorkflowDefinitionMappingFilter RequestWorkflowDefinitionMappingFilter = new RequestWorkflowDefinitionMappingFilter
            {
                Skip = 0,
                Take = 1,
                WorkflowDefinitionId = new IdFilter { Equal = WorkflowDefinition.Id }
            };

            var count = await UOW.RequestWorkflowDefinitionMappingRepository.Count(RequestWorkflowDefinitionMappingFilter);
            if (count != 0)
                WorkflowDefinition.AddError(nameof(WorkflowDefinitionValidator), nameof(WorkflowDefinition.Id), ErrorCode.WorkflowDefinitionInUsed);
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> Create(WorkflowDefinition WorkflowDefinition)
        {
            await ValidateCode(WorkflowDefinition);
            await ValidateName(WorkflowDefinition);
            await ValidateWorkflowType(WorkflowDefinition);
            await ValidateStatusId(WorkflowDefinition);
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> Update(WorkflowDefinition WorkflowDefinition)
        {
            if (await ValidateId(WorkflowDefinition))
            {
                await ValidateCode(WorkflowDefinition);
                await ValidateName(WorkflowDefinition);
                await ValidateWorkflowType(WorkflowDefinition);
                await ValidateStatusId(WorkflowDefinition);
            }
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> Delete(WorkflowDefinition WorkflowDefinition)
        {
            if (await ValidateId(WorkflowDefinition))
            {
                await WorkflowDefinitionInUsed(WorkflowDefinition);
            }
            return WorkflowDefinition.IsValidated;
        }

        public async Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions)
        {
            foreach (WorkflowDefinition WorkflowDefinition in WorkflowDefinitions)
            {
                await Delete(WorkflowDefinition);
            }
            return WorkflowDefinitions.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<WorkflowDefinition> WorkflowDefinitions)
        {
            return true;
        }
    }
}
