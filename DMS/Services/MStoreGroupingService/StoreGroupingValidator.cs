using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MStoreGrouping
{
    public interface IStoreGroupingValidator : IServiceScoped
    {
        Task<bool> Create(StoreGrouping StoreGrouping);
        Task<bool> Update(StoreGrouping StoreGrouping);
        Task<bool> Delete(StoreGrouping StoreGrouping);
        Task<bool> BulkDelete(List<StoreGrouping> StoreGroupings);
        Task<bool> Import(List<StoreGrouping> StoreGroupings);
    }

    public class StoreGroupingValidator : IStoreGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            StatusNotExisted,
            ParentNotExisted
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(StoreGrouping StoreGrouping)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreGrouping.Id },
                Selects = StoreGroupingSelect.Id
            };

            int count = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
            if (count == 0)
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        private async Task<bool> ValidateCode(StoreGrouping StoreGrouping)
        {
            if (string.IsNullOrEmpty(StoreGrouping.Code))
            {
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Code), ErrorCode.CodeEmpty);
                return false;
            }
            var Code = StoreGrouping.Code;
            if (StoreGrouping.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(StoreGrouping.Code))
            {
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Code), ErrorCode.CodeHasSpecialCharacter);
                return false;
            }
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = StoreGrouping.Id },
                Code = new StringFilter { Equal = StoreGrouping.Code },
                Selects = StoreGroupingSelect.Code
            };

            int count = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
            if (count != 0)
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Code), ErrorCode.CodeExisted);
            return count == 0;
        }

        private async Task<bool> ValidateName(StoreGrouping StoreGrouping)
        {
            if (string.IsNullOrEmpty(StoreGrouping.Name))
            {
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Name), ErrorCode.NameEmpty);
                return false;
            }
            if (StoreGrouping.Name.Length > 255)
            {
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Name), ErrorCode.NameOverLength);
                return false;
            }
            return true;
        }

        private async Task<bool> ValidateParent(StoreGrouping StoreGrouping)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreGrouping.ParentId },
                Selects = StoreGroupingSelect.Id
            };

            int count = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
            if (count == 0)
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.ParentId), ErrorCode.ParentNotExisted);
            return count != 0;
        }

        public async Task<bool> Create(StoreGrouping StoreGrouping)
        {
            await ValidateCode(StoreGrouping);
            await ValidateName(StoreGrouping);
            await ValidateParent(StoreGrouping);
            return StoreGrouping.IsValidated;
        }

        public async Task<bool> Update(StoreGrouping StoreGrouping)
        {
            if (await ValidateId(StoreGrouping))
            {
                await ValidateCode(StoreGrouping);
                await ValidateName(StoreGrouping);
                await ValidateParent(StoreGrouping);
            }
            return StoreGrouping.IsValidated;
        }

        public async Task<bool> Delete(StoreGrouping StoreGrouping)
        {
            if (await ValidateId(StoreGrouping))
            {
            }
            return StoreGrouping.IsValidated;
        }

        public async Task<bool> BulkDelete(List<StoreGrouping> StoreGroupings)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = StoreGroupings.Select(a => a.Id).ToList() },
                Selects = StoreGroupingSelect.Id
            };

            var listInDB = await UOW.StoreGroupingRepository.List(StoreGroupingFilter);
            var listExcept = StoreGroupings.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var StoreGrouping in listExcept)
            {
                StoreGrouping.AddError(nameof(StoreGroupingValidator), nameof(StoreGrouping.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> Import(List<StoreGrouping> StoreGroupings)
        {
            return true;
        }
    }
}
