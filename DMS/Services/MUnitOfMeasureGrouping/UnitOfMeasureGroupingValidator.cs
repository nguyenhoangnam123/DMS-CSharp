using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MUnitOfMeasureGrouping
{
    public interface IUnitOfMeasureGroupingValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping);
        Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
        Task<bool> Import(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings);
    }

    public class UnitOfMeasureGroupingValidator : IUnitOfMeasureGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            NameEmpty,
            NameOverLength,
            StatusNotExisted,
            UnitOfMeasureEmpty,
            UnitOfMeasureNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UnitOfMeasureGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasureGrouping.Id },
                Selects = UnitOfMeasureGroupingSelect.Id
            };

            int count = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
            if (count == 0)
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateCode(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (string.IsNullOrWhiteSpace(UnitOfMeasureGrouping.Code))
            {
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Code), ErrorCode.CodeEmpty);
                return false;
            }
            else
            {
                var Code = UnitOfMeasureGrouping.Code;
                if (UnitOfMeasureGrouping.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(UnitOfMeasureGrouping.Code))
                {
                    UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Code), ErrorCode.CodeHasSpecialCharacter);
                    return false;
                }

                UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = UnitOfMeasureGrouping.Id },
                    Code = new StringFilter { Equal = UnitOfMeasureGrouping.Code },
                    Selects = UnitOfMeasureGroupingSelect.Code
                };

                int count = await UOW.UnitOfMeasureGroupingRepository.Count(UnitOfMeasureGroupingFilter);
                if (count != 0)
                    UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Code), ErrorCode.CodeExisted);
            }
            return UnitOfMeasureGrouping.IsValidated;
        }
        public async Task<bool> ValidateName(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (string.IsNullOrWhiteSpace(UnitOfMeasureGrouping.Name))
            {
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Name), ErrorCode.NameEmpty);
            }
            if (UnitOfMeasureGrouping.Name.Length > 255)
            {
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Name), ErrorCode.NameOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateStatus(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (StatusEnum.ACTIVE.Id != UnitOfMeasureGrouping.StatusId && StatusEnum.INACTIVE.Id != UnitOfMeasureGrouping.StatusId)
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.Status), ErrorCode.StatusNotExisted);
            return true;
        }
        private async Task<bool> ValidateUnitOfMeasureId(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (UnitOfMeasureGrouping.UnitOfMeasureId == 0)
            {
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.UnitOfMeasureId), ErrorCode.UnitOfMeasureEmpty);
            }
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasureGrouping.UnitOfMeasureId },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                Selects = UnitOfMeasureSelect.Id
            };

            int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
            if (count == 0)
                UnitOfMeasureGrouping.AddError(nameof(UnitOfMeasureGroupingValidator), nameof(UnitOfMeasureGrouping.UnitOfMeasureId), ErrorCode.UnitOfMeasureNotExisted);
            return count != 0;
        }

        public async Task<bool> Create(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            await ValidateCode(UnitOfMeasureGrouping);
            await ValidateName(UnitOfMeasureGrouping);
            await ValidateUnitOfMeasureId(UnitOfMeasureGrouping);
            await ValidateStatus(UnitOfMeasureGrouping);
            return UnitOfMeasureGrouping.IsValidated;
        }

        public async Task<bool> Update(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (await ValidateId(UnitOfMeasureGrouping))
            {
                await ValidateCode(UnitOfMeasureGrouping);
                await ValidateName(UnitOfMeasureGrouping);
                await ValidateUnitOfMeasureId(UnitOfMeasureGrouping);
                await ValidateStatus(UnitOfMeasureGrouping);
            }
            return UnitOfMeasureGrouping.IsValidated;
        }

        public async Task<bool> Delete(UnitOfMeasureGrouping UnitOfMeasureGrouping)
        {
            if (await ValidateId(UnitOfMeasureGrouping))
            {
            }
            return UnitOfMeasureGrouping.IsValidated;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            return true;
        }

        public async Task<bool> Import(List<UnitOfMeasureGrouping> UnitOfMeasureGroupings)
        {
            return true;
        }
    }
}
