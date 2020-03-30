using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MUnitOfMeasure
{
    public interface IUnitOfMeasureValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasure UnitOfMeasure);
        Task<bool> Update(UnitOfMeasure UnitOfMeasure);
        Task<bool> Delete(UnitOfMeasure UnitOfMeasure);
        Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
        Task<bool> Import(List<UnitOfMeasure> UnitOfMeasures);
    }

    public class UnitOfMeasureValidator : IUnitOfMeasureValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            NameEmpty,
            NameOverLength,
            StatusNotExisted
        }


        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public UnitOfMeasureValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(UnitOfMeasure UnitOfMeasure)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = UnitOfMeasure.Id },
                Selects = UnitOfMeasureSelect.Id
            };

            int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
            if (count == 0)
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        public async Task<bool> ValidateCode(UnitOfMeasure UnitOfMeasure)
        {
            if (string.IsNullOrEmpty(UnitOfMeasure.Code))
            {
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Code), ErrorCode.CodeEmpty);
                return false;
            }
            var Code = UnitOfMeasure.Code;
            if (UnitOfMeasure.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(UnitOfMeasure.Code))
            {
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Code), ErrorCode.CodeHasSpecialCharacter);
                return false;
            }
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = UnitOfMeasure.Id },
                Code = new StringFilter { Equal = UnitOfMeasure.Code },
                Selects = UnitOfMeasureSelect.Code
            };

            int count = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
            if (count != 0)
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Code), ErrorCode.CodeExisted);
            return count == 0;
        }
        public async Task<bool> ValidateName(UnitOfMeasure UnitOfMeasure)
        {
            if (string.IsNullOrEmpty(UnitOfMeasure.Name))
            {
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Name), ErrorCode.NameEmpty);
            }
            if (UnitOfMeasure.Name.Length > 255)
            {
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Name), ErrorCode.NameOverLength);
            }
            return true;
        }

        public async Task<bool> ValidateStatus(UnitOfMeasure UnitOfMeasure)
        {
            if (StatusEnum.ACTIVE.Id != UnitOfMeasure.StatusId && StatusEnum.INACTIVE.Id != UnitOfMeasure.StatusId)
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Status), ErrorCode.StatusNotExisted);
            return true;
        }

        public async Task<bool> Create(UnitOfMeasure UnitOfMeasure)
        {
            await ValidateCode(UnitOfMeasure);
            await ValidateName(UnitOfMeasure);
            await ValidateStatus(UnitOfMeasure);
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> Update(UnitOfMeasure UnitOfMeasure)
        {
            if (await ValidateId(UnitOfMeasure))
            {
                await ValidateCode(UnitOfMeasure);
                await ValidateName(UnitOfMeasure);
                await ValidateStatus(UnitOfMeasure);
            }
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> Delete(UnitOfMeasure UnitOfMeasure)
        {
            if (await ValidateId(UnitOfMeasure))
            {
            }
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            return true;
        }

        public async Task<bool> Import(List<UnitOfMeasure> UnitOfMeasures)
        {
            return true;
        }
    }
}
