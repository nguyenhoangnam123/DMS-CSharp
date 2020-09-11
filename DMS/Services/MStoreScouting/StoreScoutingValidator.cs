using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Enums;

namespace DMS.Services.MStoreScouting
{
    public interface IStoreScoutingValidator : IServiceScoped
    {
        Task<bool> Create(StoreScouting StoreScouting);
        Task<bool> Update(StoreScouting StoreScouting);
        Task<bool> Delete(StoreScouting StoreScouting);
        Task<bool> Reject(StoreScouting StoreScouting);
        Task<bool> BulkDelete(List<StoreScouting> StoreScoutings);
        Task<bool> Import(List<StoreScouting> StoreScoutings);
    }

    public class StoreScoutingValidator : IStoreScoutingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeOverLength,
            NameEmpty,
            NameOverLength,
            OwnerPhoneOverLength,
            OwnerPhoneInUsed,
            OwnerPhoneEmpty,
            AddressEmpty,
            AddressOverLength,
            ProvinceNotExisted,
            DistrictNotExisted,
            WardNotExisted,
            StoreScoutingHasOpened,
            StoreScoutingHasRejected
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

        private async Task<bool> ValidateOwnerPhone(StoreScouting StoreScouting)
        {
            if (string.IsNullOrWhiteSpace(StoreScouting.OwnerPhone))
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.OwnerPhone), ErrorCode.OwnerPhoneEmpty);
            }
            else
            {
                if (StoreScouting.OwnerPhone.Length > 255)
                    StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.OwnerPhone), ErrorCode.OwnerPhoneOverLength);
                else
                {
                    StoreScoutingFilter StoreScoutingFilter = new StoreScoutingFilter
                    {
                        OwnerPhone = new StringFilter { Equal = StoreScouting.OwnerPhone },
                        Id = new IdFilter { NotEqual = StoreScouting.Id }
                    };

                    int count = await UOW.StoreScoutingRepository.Count(StoreScoutingFilter);
                    if (count != 0)
                        StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.OwnerPhone), ErrorCode.OwnerPhoneInUsed);
                }
            }
            return StoreScouting.IsValidated;
        }

        private async Task<bool> ValidateAddress(StoreScouting StoreScouting)
        {
            if (string.IsNullOrWhiteSpace(StoreScouting.Address))
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Address), ErrorCode.AddressEmpty);
            }
            else if (StoreScouting.Address.Length > 3000)
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Address), ErrorCode.AddressOverLength);
            }
            return StoreScouting.IsValidated;
        }

        private async Task<bool> ValidateCode(StoreScouting StoreScouting)
        {
            if (string.IsNullOrWhiteSpace(StoreScouting.Code))
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Code), ErrorCode.CodeEmpty);
            }
            else if (StoreScouting.Code.Length > 500)
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Code), ErrorCode.CodeOverLength);
            }
            return StoreScouting.IsValidated;
        }

        private async Task<bool> ValidateName(StoreScouting StoreScouting)
        {
            if (string.IsNullOrWhiteSpace(StoreScouting.Name))
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Name), ErrorCode.NameEmpty);
            }
            else if (StoreScouting.Name.Length > 500)
            {
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.Name), ErrorCode.NameOverLength);
            }
            return StoreScouting.IsValidated;
        }

        private async Task<bool> ValidateProvinceId(StoreScouting StoreScouting)
        {
            if (StoreScouting.ProvinceId != 0)
            {
                ProvinceFilter ProvinceFilter = new ProvinceFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = StoreScouting.ProvinceId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ProvinceSelect.Id
                };

                int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
                if (count == 0)
                    StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.ProvinceId), ErrorCode.ProvinceNotExisted);
            }
            return StoreScouting.IsValidated;
        }
        private async Task<bool> ValidateDistrictId(StoreScouting StoreScouting)
        {
            if (StoreScouting.DistrictId != 0)
            {
                DistrictFilter DistrictFilter = new DistrictFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = StoreScouting.DistrictId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = DistrictSelect.Id
                };

                int count = await UOW.DistrictRepository.Count(DistrictFilter);
                if (count == 0)
                    StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.DistrictId), ErrorCode.DistrictNotExisted);
            }
            return StoreScouting.IsValidated;
        }
        private async Task<bool> ValidateWardId(StoreScouting StoreScouting)
        {
            if (StoreScouting.WardId != 0)
            {
                WardFilter WardFilter = new WardFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = StoreScouting.WardId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = WardSelect.Id
                };

                int count = await UOW.WardRepository.Count(WardFilter);
                if (count == 0)
                    StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.WardId), ErrorCode.WardNotExisted);
            }
            return StoreScouting.IsValidated;
        }

        private async Task<bool> ValidateStatus(StoreScouting StoreScouting)
        {
            if (StoreScouting.StoreScoutingStatusId == Enums.StoreScoutingStatusEnum.OPENED.Id)
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.StoreScoutingStatus), ErrorCode.StoreScoutingHasOpened);
            else if (StoreScouting.StoreScoutingStatusId == Enums.StoreScoutingStatusEnum.REJECTED.Id)
                StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.StoreScoutingStatus), ErrorCode.StoreScoutingHasRejected);
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Create(StoreScouting StoreScouting)
        {
            await ValidateOwnerPhone(StoreScouting);
            await ValidateCode(StoreScouting);
            await ValidateName(StoreScouting);
            await ValidateAddress(StoreScouting);
            await ValidateProvinceId(StoreScouting);
            await ValidateDistrictId(StoreScouting);
            await ValidateWardId(StoreScouting);
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Update(StoreScouting StoreScouting)
        {
            if (await ValidateId(StoreScouting))
            {
                await ValidateOwnerPhone(StoreScouting);
                await ValidateCode(StoreScouting);
                await ValidateName(StoreScouting);
                await ValidateAddress(StoreScouting);
                await ValidateProvinceId(StoreScouting);
                await ValidateDistrictId(StoreScouting);
                await ValidateWardId(StoreScouting);
                await ValidateStatus(StoreScouting);
            }
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Delete(StoreScouting StoreScouting)
        {
            if (await ValidateId(StoreScouting))
            {
                StoreScouting = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
                if (StoreScouting.StoreScoutingStatusId == StoreScoutingStatusEnum.OPENED.Id)
                    StoreScouting.AddError(nameof(StoreScoutingValidator), nameof(StoreScouting.StoreScoutingStatus), ErrorCode.StoreScoutingHasOpened);
            }
            return StoreScouting.IsValidated;
        }

        public async Task<bool> Reject(StoreScouting StoreScouting)
        {
            if (await ValidateId(StoreScouting))
            {
                await ValidateStatus(StoreScouting);
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
