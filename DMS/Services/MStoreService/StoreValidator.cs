using Common;
using DMS.Entities;
using DMS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MStore
{
    public interface IStoreValidator : IServiceScoped
    {
        Task<bool> Create(Store Store);
        Task<bool> Update(Store Store);
        Task<bool> Delete(Store Store);
        Task<bool> BulkDelete(List<Store> Stores);
        Task<bool> Import(List<Store> Stores);
    }

    public class StoreValidator : IStoreValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Store Store)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = StoreSelect.Id
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if (count == 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> Create(Store Store)
        {
            return Store.IsValidated;
        }

        public async Task<bool> Update(Store Store)
        {
            if (await ValidateId(Store))
            {
            }
            return Store.IsValidated;
        }

        public async Task<bool> Delete(Store Store)
        {
            if (await ValidateId(Store))
            {
            }
            return Store.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Store> Stores)
        {
            return true;
        }

        public async Task<bool> Import(List<Store> Stores)
        {
            return true;
        }
    }
}
