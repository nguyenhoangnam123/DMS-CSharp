using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using System.Collections.Generic;
using System.Linq;
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
            CodeExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            NameEmpty,
            NameOverLength,
            OrganizationNotExisted,
            ParentStoreNotExisted,
            StoreTypeNotExisted,
            StoreTypeEmpty,
            StoreGroupingNotExisted,
            TelephoneOverLength,
            ProvinceNotExisted,
            DistrictNotExisted,
            WardNotExisted,
            AddressEmpty,
            AddressOverLength,
            DeliveryAddressOverLength,
            OwnerNameEmpty,
            OwnerNameOverLength,
            OwnerPhoneEmpty,
            OwnerPhoneOverLength,
            OwnerEmailOverLength,
            StatusNotExisted
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
        public async Task<bool> ValidateCode(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Code))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Code), ErrorCode.CodeEmpty);
                return false;
            }
            var Code = Store.Code;
            if (Store.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Store.Code))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Code), ErrorCode.CodeHasSpecialCharacter);
                return false;
            }
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { NotEqual = Store.Id },
                Code = new StringFilter { Equal = Store.Code },
                Selects = StoreSelect.Code
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.Code), ErrorCode.CodeExisted);
            return count == 0;
        }
        public async Task<bool> ValidateName(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Name))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Name), ErrorCode.NameEmpty);
            }
            if (Store.Name.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Name), ErrorCode.NameOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateOrganization(Store Store)
        {
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = OrganizationSelect.Id
            };

            int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.OrganizationId), ErrorCode.OrganizationNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidateParentStore(Store Store)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                ParentStoreId = new IdFilter { Equal = Store.ParentStoreId },
                Selects = StoreSelect.ParentStore
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if (count == 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.ParentStoreId), ErrorCode.ParentStoreNotExisted);
            return count == 1;
        }
        public async Task<bool> ValidateStoreType(Store Store)
        {
            if(Store.StoreTypeId == 0)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.StoreTypeId), ErrorCode.StoreTypeEmpty);
            }
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = StoreTypeSelect.Id
            };

            int count = await UOW.StoreTypeRepository.Count(StoreTypeFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.StoreTypeId), ErrorCode.StoreTypeNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidateStoreGrouping(Store Store)
        {
            StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = StoreGroupingSelect.Id
            };

            int count = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.StoreGroupingId), ErrorCode.StoreGroupingNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidatePhone(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Telephone) && Store.Telephone.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Telephone), ErrorCode.TelephoneOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateProvince(Store Store)
        {
            ProvinceFilter ProvinceFilter = new ProvinceFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = ProvinceSelect.Id
            };

            int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.ProvinceId), ErrorCode.ProvinceNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidateDistrict(Store Store)
        {
            DistrictFilter DistrictFilter = new DistrictFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = DistrictSelect.Id
            };

            int count = await UOW.DistrictRepository.Count(DistrictFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.DistrictId), ErrorCode.DistrictNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidateWard(Store Store)
        {
            WardFilter WardFilter = new WardFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                Selects = WardSelect.Id
            };

            int count = await UOW.WardRepository.Count(WardFilter);
            if (count != 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.WardId), ErrorCode.WardNotExisted);
            return count == 0;
        }
        public async Task<bool> ValidateAddress(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Address))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Address), ErrorCode.AddressEmpty);
            }
            if (Store.Address.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Address), ErrorCode.AddressOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateDeliveryAddress(Store Store)
        {
            if (string.IsNullOrEmpty(Store.DeliveryAddress) && Store.DeliveryAddress.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.DeliveryAddress), ErrorCode.DeliveryAddressOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateOwnerName(Store Store)
        {
            if (string.IsNullOrEmpty(Store.OwnerName))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerName), ErrorCode.OwnerNameEmpty);
            }
            if (Store.OwnerName.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerName), ErrorCode.OwnerNameOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateOwnerPhone(Store Store)
        {
            if (string.IsNullOrEmpty(Store.OwnerPhone))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerPhone), ErrorCode.OwnerPhoneEmpty);
            }
            if (Store.OwnerPhone.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerPhone), ErrorCode.OwnerPhoneOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateOwnerEmail(Store Store)
        {
            if (string.IsNullOrEmpty(Store.OwnerEmail) && Store.OwnerEmail.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerEmail), ErrorCode.OwnerEmailOverLength);
            }
            return true;
        }
        public async Task<bool> ValidateStatus(Store Store)
        {
            if (StatusEnum.ACTIVE.Id != Store.StatusId && StatusEnum.INACTIVE.Id != Store.StatusId)
                Store.AddError(nameof(StoreValidator), nameof(Store.Status), ErrorCode.StatusNotExisted);
            return true;
        }
        public async Task<bool> Create(Store Store)
        {
            await ValidateCode(Store);
            await ValidateName(Store);
            await ValidateOrganization(Store);
            await ValidateParentStore(Store);
            await ValidateStoreType(Store);
            await ValidateStoreGrouping(Store);
            await ValidatePhone(Store);
            await ValidateProvince(Store);
            await ValidateDistrict(Store);
            await ValidateWard(Store);
            await ValidateAddress(Store);
            await ValidateDeliveryAddress(Store);
            await ValidateOwnerName(Store);
            await ValidateOwnerPhone(Store);
            await ValidateOwnerEmail(Store);
            await ValidateStatus(Store);
            return Store.IsValidated;
        }

        public async Task<bool> Update(Store Store)
        {
            if (await ValidateId(Store))
            {
                await ValidateCode(Store);
                await ValidateName(Store);
                await ValidateOrganization(Store);
                await ValidateParentStore(Store);
                await ValidateStoreType(Store);
                await ValidateStoreGrouping(Store);
                await ValidatePhone(Store);
                await ValidateProvince(Store);
                await ValidateDistrict(Store);
                await ValidateWard(Store);
                await ValidateAddress(Store);
                await ValidateDeliveryAddress(Store);
                await ValidateOwnerName(Store);
                await ValidateOwnerPhone(Store);
                await ValidateOwnerEmail(Store);
                await ValidateStatus(Store);
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
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Id = new IdFilter { In = Stores.Select(a => a.Id).ToList() },
                Selects = StoreSelect.Id
            };

            var listInDB = await UOW.StoreRepository.List(StoreFilter);
            var listExcept = Stores.Except(listInDB);
            if (listExcept == null || listExcept.Count() == 0) return true;
            foreach (var Store in listExcept)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Id), ErrorCode.IdNotExisted);
            }
            return false;
        }

        public async Task<bool> Import(List<Store> Stores)
        {
            return true;
        }
    }
}
