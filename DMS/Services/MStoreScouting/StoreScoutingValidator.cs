using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MStoreScouting
{
    public interface IStoreScoutingValidator : IServiceScoped
    {
        Task<bool> Create(StoreScouting StoreScouting);
        Task<bool> Update(StoreScouting StoreScouting);
        Task<bool> Delete(StoreScouting StoreScouting);
        Task<bool> BulkDelete(List<StoreScouting> StoreScoutings);
        Task<bool> Import(List<StoreScouting> StoreScoutings);
    }

    public class StoreScoutingValidator : IStoreScoutingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreScoutingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(StoreScouting StoreScouting)
        {
            StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreScouting.Id },
                Selects = StoreScoutingSelect.Id
            };

            int count = await UOW.StoreScoutingRepository.Count(StoreScoutingFilter);
            if (count == 0)
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(StoreScouting StoreScouting)
        {
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Update(StoreScouting StoreScouting)
        {
            if (await ValidateId(StoreScouting))
            {
            }
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Delete(StoreScouting StoreScouting)
        {
            if (await ValidateId(StoreScouting))
            {
            }
            return StoreScouting.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<StoreScouting> StoreScoutings)
        {
            return true;
        }
        
        public async Task<bool> Import(List<StoreScouting> StoreScoutings)
        {
            return true;
        }
    }
}
