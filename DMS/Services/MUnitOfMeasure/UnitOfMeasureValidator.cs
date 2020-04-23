using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MUnitOfMeasure
{
    public interface IUnitOfMeasureValidator : IServiceScoped
    {
        Task<bool> Create(UnitOfMeasure UnitOfMeasure);
        Task<bool> Update(UnitOfMeasure UnitOfMeasure);
        Task<bool> Delete(UnitOfMeasure UnitOfMeasure);
        Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures);
        Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
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
            StatusNotExisted,
            UnitOfMeasureInUsed
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
            if (string.IsNullOrWhiteSpace(UnitOfMeasure.Code))
            {
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Code), ErrorCode.CodeEmpty);
                return false;
            }
            else
            {
                var Code = UnitOfMeasure.Code;
                if ( !FilterExtension.ChangeToEnglishChar(Code).Equals(UnitOfMeasure.Code))
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
            }
            return UnitOfMeasure.IsValidated;
        }
        public async Task<bool> ValidateName(UnitOfMeasure UnitOfMeasure)
        {
            if (string.IsNullOrWhiteSpace(UnitOfMeasure.Name))
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

        private async Task<bool> ValidateUnitOfMeasureInUsed(UnitOfMeasure UnitOfMeasure)
        {
            ProductFilter ProductFilter = new ProductFilter
            {
                UnitOfMeasureId = new IdFilter { Equal = UnitOfMeasure.Id },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
            };
            int count = await UOW.ProductRepository.Count(ProductFilter);
            if (count > 0)
                UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Id), ErrorCode.UnitOfMeasureInUsed);

            return UnitOfMeasure.IsValidated;
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
                await ValidateUnitOfMeasureInUsed(UnitOfMeasure);
            }
            return UnitOfMeasure.IsValidated;
        }

        public async Task<bool> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            foreach (var UnitOfMeasure in UnitOfMeasures)
            {
                await Delete(UnitOfMeasure);
            }
            return UnitOfMeasures.All(st => st.IsValidated);

        }
        public async Task<bool> BulkMerge(List<UnitOfMeasure> UnitOfMeasures)
        {
            var listCodeInDB = (await UOW.UnitOfMeasureRepository.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.Code
            })).Select(e => e.Code);

            foreach (var UnitOfMeasure in UnitOfMeasures)
            {
                if (listCodeInDB.Contains(UnitOfMeasure.Code))
                {
                    UnitOfMeasure.AddError(nameof(UnitOfMeasureValidator), nameof(UnitOfMeasure.Code), ErrorCode.CodeExisted);
                    return false;
                }

                if (!await(ValidateName(UnitOfMeasure))) return false;
                if (!await(ValidateStatus(UnitOfMeasure))) return false;
            }
            return true;
        }
    }
}
