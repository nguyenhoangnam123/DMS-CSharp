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
            ResellerNotExisted,
            ProvinceNotExisted,
            DistrictNotExisted,
            WardNotExisted,
            AddressEmpty,
            AddressOverLength,
            DeliveryAddressOverLength,
            LocationEmpty,
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
        #region Id
        private async Task<bool> ValidateId(Store Store)
        {
            StoreFilter StoreFilter = new StoreFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.Id },
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                Selects = StoreSelect.Id
            };

            int count = await UOW.StoreRepository.Count(StoreFilter);
            if (count == 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }
        #endregion

        #region Code
        private async Task<bool> ValidateCode(Store Store)
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
        #endregion

        #region Name
        private async Task<bool> ValidateName(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Name))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Name), ErrorCode.NameEmpty);
                return false;
            }
            if (Store.Name.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Name), ErrorCode.NameOverLength);
                return false;
            }
            return true;
        }
        #endregion

        #region Organization
        private async Task<bool> ValidateOrganizationId(Store Store)
        {
            if(Store.OrganizationId != 0)
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.OrganizationId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = OrganizationSelect.Id
                };

                int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.OrganizationId), ErrorCode.OrganizationNotExisted);
                return count != 0;
            }
            return true;
        }
        #endregion

        #region Parent Store
        private async Task<bool> ValidateParentStoreId(Store Store)
        {
            if(Store.ParentStoreId != 0)
            {
                StoreFilter StoreFilter = new StoreFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.ParentStoreId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = StoreSelect.Id
                };

                int count = await UOW.StoreRepository.Count(StoreFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.ParentStoreId), ErrorCode.ParentStoreNotExisted);
                return count != 0;
            }
            return true;
        }
        #endregion

        #region Store Type
        private async Task<bool> ValidateStoreTypeId(Store Store)
        {
            if(Store.StoreTypeId == 0)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.StoreTypeId), ErrorCode.StoreTypeEmpty);
            }
            StoreTypeFilter StoreTypeFilter = new StoreTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Store.StoreStatusId },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Selects = StoreTypeSelect.Id
            };

            int count = await UOW.StoreTypeRepository.Count(StoreTypeFilter);
            if (count == 0)
                Store.AddError(nameof(StoreValidator), nameof(Store.StoreTypeId), ErrorCode.StoreTypeNotExisted);
            return count != 0;
        }
        #endregion

        #region Store Grouping
        private async Task<bool> ValidateStoreGroupingId(Store Store)
        {
            if(Store.StoreGroupingId != 0)
            {
                StoreGroupingFilter StoreGroupingFilter = new StoreGroupingFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.StoreGroupingId },
                    Selects = StoreGroupingSelect.Id
                };

                int count = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.StoreGroupingId), ErrorCode.StoreGroupingNotExisted);
                return count != 0;
            }
            return true;
        }
        #endregion

        #region Phone
        private async Task<bool> ValidatePhone(Store Store)
        {
            if (!string.IsNullOrEmpty(Store.Telephone) && Store.Telephone.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Telephone), ErrorCode.TelephoneOverLength);
                return false;
            }
            return true;
        }
        #endregion
        #region Reseller
        private async Task<bool> ValidateResellerId(Store Store)
        {
            if (Store.ResellerId != 0)
            {
                ResellerFilter ResellerFilter = new ResellerFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.ResellerId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ResellerSelect.Id
                };

                int count = await UOW.ResellerRepository.Count(ResellerFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.ResellerId), ErrorCode.ResellerNotExisted);
                return count != 0;
            }
            return true;
        }
        #endregion
        #region Province + District + Ward
        private async Task<bool> ValidateProvinceId(Store Store)
        {
            if(Store.ProvinceId != 0)
            {
                ProvinceFilter ProvinceFilter = new ProvinceFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.ProvinceId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ProvinceSelect.Id
                };

                int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.ProvinceId), ErrorCode.ProvinceNotExisted);
                return count != 0;
            }
            return true;
        }
        private async Task<bool> ValidateDistrictId(Store Store)
        {
            if(Store.DistrictId != 0)
            {
                DistrictFilter DistrictFilter = new DistrictFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.DistrictId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = DistrictSelect.Id
                };

                int count = await UOW.DistrictRepository.Count(DistrictFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.DistrictId), ErrorCode.DistrictNotExisted);
                return count != 0;
            }
            return true;
        }
        private async Task<bool> ValidateWardId(Store Store)
        {
            if(Store.WardId != 0)
            {
                WardFilter WardFilter = new WardFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Store.WardId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = WardSelect.Id
                };

                int count = await UOW.WardRepository.Count(WardFilter);
                if (count == 0)
                    Store.AddError(nameof(StoreValidator), nameof(Store.WardId), ErrorCode.WardNotExisted);
                return count != 0;
            }
            return true;
        }
        #endregion

        #region Address + Location
        private async Task<bool> ValidateAddress(Store Store)
        {
            if (string.IsNullOrEmpty(Store.Address))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Address), ErrorCode.AddressEmpty);
                return false;
            }
            if (Store.Address.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.Address), ErrorCode.AddressOverLength);
                return false;
            }
            return true;
        }
        private async Task<bool> ValidateDeliveryAddress(Store Store)
        {
            if (!string.IsNullOrEmpty(Store.DeliveryAddress) && Store.DeliveryAddress.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.DeliveryAddress), ErrorCode.DeliveryAddressOverLength);
                return false;
            }
            return true;
        }
        private async Task<bool> ValidateLocation(Store Store)
        {
            if (Store.Latitude == null || Store.Longitude == null)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerName), ErrorCode.LocationEmpty);
                return false;
            }
            return true;
        }
        #endregion

        #region Owner
        private async Task<bool> ValidateOwnerName(Store Store)
        {
            if (string.IsNullOrEmpty(Store.OwnerName))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerName), ErrorCode.OwnerNameEmpty);
                return false;
            }
            if (Store.OwnerName.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerName), ErrorCode.OwnerNameOverLength);
                return false;
            }
            return true;
        }
        private async Task<bool> ValidateOwnerPhone(Store Store)
        {
            if (string.IsNullOrEmpty(Store.OwnerPhone))
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerPhone), ErrorCode.OwnerPhoneEmpty);
                return false;
            }
            if (Store.OwnerPhone.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerPhone), ErrorCode.OwnerPhoneOverLength);
                return false;
            }
            return true;
        }
        private async Task<bool> ValidateOwnerEmail(Store Store)
        {
            if (!string.IsNullOrEmpty(Store.OwnerEmail) && Store.OwnerEmail.Length > 255)
            {
                Store.AddError(nameof(StoreValidator), nameof(Store.OwnerEmail), ErrorCode.OwnerEmailOverLength);
                return false;
            }
            return true;
        }
        #endregion

        #region Status
        private async Task<bool> ValidateStatusId(Store Store)
        {
            if (StatusEnum.ACTIVE.Id != Store.StatusId && StatusEnum.INACTIVE.Id != Store.StatusId)
                Store.AddError(nameof(StoreValidator), nameof(Store.Status), ErrorCode.StatusNotExisted);
            return true;
        }
        #endregion
        public async Task<bool> Create(Store Store)
        {
            await ValidateCode(Store);
            await ValidateName(Store);
            await ValidateOrganizationId(Store);
            await ValidateParentStoreId(Store);
            await ValidateStoreTypeId(Store);
            await ValidateStoreGroupingId(Store);
            await ValidatePhone(Store);
            await ValidateResellerId(Store);
            await ValidateProvinceId(Store);
            await ValidateDistrictId(Store);
            await ValidateWardId(Store);
            await ValidateAddress(Store);
            await ValidateDeliveryAddress(Store);
            await ValidateLocation(Store);
            await ValidateOwnerName(Store);
            await ValidateOwnerPhone(Store);
            await ValidateOwnerEmail(Store);
            await ValidateStatusId(Store);
            return Store.IsValidated;
        }

        public async Task<bool> Update(Store Store)
        {
            if (await ValidateId(Store))
            {
                await ValidateCode(Store);
                await ValidateName(Store);
                await ValidateOrganizationId(Store);
                await ValidateParentStoreId(Store);
                await ValidateStoreTypeId(Store);
                await ValidateStoreGroupingId(Store);
                await ValidatePhone(Store);
                await ValidateResellerId(Store);
                await ValidateProvinceId(Store);
                await ValidateDistrictId(Store);
                await ValidateWardId(Store);
                await ValidateAddress(Store);
                await ValidateDeliveryAddress(Store);
                await ValidateLocation(Store);
                await ValidateOwnerName(Store);
                await ValidateOwnerPhone(Store);
                await ValidateOwnerEmail(Store);
                await ValidateStatusId(Store);
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
            var listCodeInDB = (await UOW.StoreRepository.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreSelect.Code
            })).Select(e => e.Code);

            var listOrganizationCodeInDB = (await UOW.OrganizationRepository.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.Code
            })).Select(e => e.Code);
            var listStoreTypeCodeInDB = (await UOW.StoreTypeRepository.List(new StoreTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreTypeSelect.Code
            })).Select(e => e.Code);
            var listStoreGroupingCodeInDB = (await UOW.StoreGroupingRepository.List(new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = StoreGroupingSelect.Code
            })).Select(e => e.Code);
            var listProvinceCodeInDB = (await UOW.ProvinceRepository.List(new ProvinceFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProvinceSelect.Code
            })).Select(e => e.Code);
            var listDistrictCodeInDB = (await UOW.DistrictRepository.List(new DistrictFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = DistrictSelect.Code
            })).Select(e => e.Code);
            var listWardCodeInDB = (await UOW.WardRepository.List(new WardFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = WardSelect.Code
            })).Select(e => e.Code);
            foreach (var Store in Stores)
            {
                if (listCodeInDB.Contains(Store.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.Code), ErrorCode.CodeExisted);
                    return false;
                }
                if (listOrganizationCodeInDB.Contains(Store.Organization.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.Organization), ErrorCode.OrganizationNotExisted);
                    return false;
                }
                if (listProvinceCodeInDB.Contains(Store.Province.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.Province), ErrorCode.ProvinceNotExisted);
                    return false;
                }
                if (listDistrictCodeInDB.Contains(Store.District.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.District), ErrorCode.DistrictNotExisted);
                    return false;
                }
                if (listWardCodeInDB.Contains(Store.Ward.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.Ward), ErrorCode.WardNotExisted);
                    return false;
                }
                if (listStoreGroupingCodeInDB.Contains(Store.StoreGrouping.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.StoreGrouping), ErrorCode.StoreGroupingNotExisted);
                    return false;
                }
                if (listStoreTypeCodeInDB.Contains(Store.StoreType.Code))
                {
                    Store.AddError(nameof(StoreValidator), nameof(Store.StoreType), ErrorCode.StoreTypeNotExisted);
                    return false;
                }

                if (!await (ValidateName(Store))) return false;
                if (!await (ValidateAddress(Store))) return false;
                if (!await (ValidatePhone(Store))) return false;
                if (!await (ValidateDeliveryAddress(Store))) return false;
                if (!await (ValidateLocation(Store))) return false;
                if (!await (ValidateOwnerName(Store))) return false;
                if (!await (ValidateOwnerPhone(Store))) return false;
                if (!await (ValidateOwnerEmail(Store))) return false;
                
            }
            return true;
        }
    }
}
