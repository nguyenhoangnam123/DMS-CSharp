using Common;
using DMS.Entities;
using DMS.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MStoreChecking
{
    public interface IStoreCheckingValidator : IServiceScoped
    {
        Task<bool> Create(StoreChecking StoreChecking);
        Task<bool> Update(StoreChecking StoreChecking);
        Task<bool> Delete(StoreChecking StoreChecking);
        Task<bool> BulkDelete(List<StoreChecking> StoreCheckings);
        Task<bool> Import(List<StoreChecking> StoreCheckings);
    }

    public class StoreCheckingValidator : IStoreCheckingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            StoreNotExisted,
            DistanceOutOfRange
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public StoreCheckingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(StoreChecking StoreChecking)
        {
            StoreCheckingFilter StoreCheckingFilter = new StoreCheckingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = StoreChecking.Id },
                Selects = StoreCheckingSelect.Id
            };

            int count = await UOW.StoreCheckingRepository.Count(StoreCheckingFilter);
            if (count == 0)
                StoreChecking.AddError(nameof(StoreCheckingValidator), nameof(StoreChecking.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateGPS(StoreChecking StoreChecking)
        {
            var Store = await UOW.StoreRepository.Get(StoreChecking.StoreId);
            if (Store == null)
            {
                StoreChecking.AddError(nameof(StoreCheckingValidator), nameof(StoreChecking.Store), ErrorCode.StoreNotExisted);
            }
            else
            {
                var Distance = Math.Sqrt(Math.Pow(Convert.ToDouble(Store.Longitude - StoreChecking.Longtitude), 2)
                           + Math.Pow(Convert.ToDouble(Store.Latitude - StoreChecking.Latitude), 2));

                if (Distance > 100)
                {
                    StoreChecking.AddError(nameof(StoreCheckingValidator), nameof(StoreChecking.Store), ErrorCode.DistanceOutOfRange);
                }
            }

            return StoreChecking.IsValidated;
        }

        public async Task<bool> Create(StoreChecking StoreChecking)
        {
            await ValidateGPS(StoreChecking);
            return StoreChecking.IsValidated;
        }

        public async Task<bool> Update(StoreChecking StoreChecking)
        {
            if (await ValidateId(StoreChecking))
            {
            }
            return StoreChecking.IsValidated;
        }

        public async Task<bool> Delete(StoreChecking StoreChecking)
        {
            if (await ValidateId(StoreChecking))
            {
            }
            return StoreChecking.IsValidated;
        }

        public async Task<bool> BulkDelete(List<StoreChecking> StoreCheckings)
        {
            return true;
        }

        public async Task<bool> Import(List<StoreChecking> StoreCheckings)
        {
            return true;
        }
    }
}
