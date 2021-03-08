using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MStoreUser
{
    public interface IStoreUserValidator : IServiceScoped
    {
        Task<bool> Create(StoreUser StoreUser);
        Task<bool> Update(StoreUser StoreUser);
        Task<bool> Delete(StoreUser StoreUser);
        Task<bool> BulkDelete(List<StoreUser> StoreUsers);
        Task<bool> Import(List<StoreUser> StoreUsers);
    }

    public class StoreUserValidator : IStoreUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(StoreUser StoreUser)
        {
            StoreUserFilter StoreUserFilter = new StoreUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreUser.Id },
                Selects = StoreUserSelect.Id
            };

            int count = await UOW.StoreUserRepository.Count(StoreUserFilter);
            if (count == 0)
                StoreUser.AddError(nameof(StoreUserValidator), nameof(StoreUser.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(StoreUser StoreUser)
        {
            return StoreUser.IsValidated;
        }

        public async Task<bool> Update(StoreUser StoreUser)
        {
            if (await ValidateId(StoreUser))
            {
            }
            return StoreUser.IsValidated;
        }

        public async Task<bool> Delete(StoreUser StoreUser)
        {
            if (await ValidateId(StoreUser))
            {
            }
            return StoreUser.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<StoreUser> StoreUsers)
        {
            foreach (StoreUser StoreUser in StoreUsers)
            {
                await Delete(StoreUser);
            }
            return StoreUsers.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<StoreUser> StoreUsers)
        {
            return true;
        }
    }
}
