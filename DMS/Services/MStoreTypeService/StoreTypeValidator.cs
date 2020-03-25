using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MStoreType
{
    public interface IStoreTypeValidator : IServiceScoped
    {
        Task<bool> Create(StoreType StoreType);
        Task<bool> Update(StoreType StoreType);
        Task<bool> Delete(StoreType StoreType);
        Task<bool> BulkDelete(List<StoreType> StoreTypes);
        Task<bool> Import(List<StoreType> StoreTypes);
    }

    public class StoreTypeValidator : IStoreTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(StoreType StoreType)
        {
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreType.Id },
                Selects = StoreTypeSelect.Id
            };

            int count = await UOW.StoreTypeRepository.Count(StoreTypeFilter);
            if (count == 0)
                StoreType.AddError(nameof(StoreTypeValidator), nameof(StoreType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(StoreType StoreType)
        {
            return StoreType.IsValidated;
        }

        public async Task<bool> Update(StoreType StoreType)
        {
            if (await ValidateId(StoreType))
            {
            }
            return StoreType.IsValidated;
        }

        public async Task<bool> Delete(StoreType StoreType)
        {
            if (await ValidateId(StoreType))
            {
            }
            return StoreType.IsValidated;
        }

        public async Task<bool> BulkDelete(List<StoreType> StoreTypes)
        {
            return true;
        }

        public async Task<bool> Import(List<StoreType> StoreTypes)
        {
            return true;
        }
    }
}
