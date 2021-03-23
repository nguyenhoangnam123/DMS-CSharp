using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Enums;

namespace DMS.Services.MShowingWarehouse
{
    public interface IShowingWarehouseValidator : IServiceScoped
    {
        Task<bool> Create(ShowingWarehouse ShowingWarehouse);
        Task<bool> Update(ShowingWarehouse ShowingWarehouse);
        Task<bool> Delete(ShowingWarehouse ShowingWarehouse);
        Task<bool> BulkDelete(List<ShowingWarehouse> ShowingWarehouses);
        Task<bool> Import(List<ShowingWarehouse> ShowingWarehouses);
    }

    public class ShowingWarehouseValidator : IShowingWarehouseValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            CodeEmpty,
            CodeHasSpecialCharacter,
            CodeExisted,
            NameEmpty,
            NameOverLength,
            AddressOverLength,
            OrganizationNotExisted,
            OrganizationEmpty,
            DistrictNotExisted,
            ProvinceNotExisted,
            WardNotExisted,
            StatusNotExisted,
            ShowingWarehouseHasShowingInventory
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ShowingWarehouseValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ShowingWarehouse ShowingWarehouse)
        {
            ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ShowingWarehouse.Id },
                Selects = ShowingWarehouseSelect.Id
            };

            int count = await UOW.ShowingWarehouseRepository.Count(ShowingWarehouseFilter);
            if (count == 0)
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        #region Code
        private async Task<bool> ValidateCode(ShowingWarehouse ShowingWarehouse)
        {
            if (string.IsNullOrWhiteSpace(ShowingWarehouse.Code))
            {
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = ShowingWarehouse.Code;
                if (ShowingWarehouse.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(ShowingWarehouse.Code))
                {
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                ShowingWarehouseFilter ShowingWarehouseFilter = new ShowingWarehouseFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = ShowingWarehouse.Id },
                    Code = new StringFilter { Equal = ShowingWarehouse.Code },
                    Selects = ShowingWarehouseSelect.Code
                };

                int count = await UOW.ShowingWarehouseRepository.Count(ShowingWarehouseFilter);
                if (count != 0)
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Code), ErrorCode.CodeExisted);
            }

            return ShowingWarehouse.IsValidated;
        }
        #endregion

        #region Name
        private async Task<bool> ValidateName(ShowingWarehouse ShowingWarehouse)
        {
            if (string.IsNullOrWhiteSpace(ShowingWarehouse.Name))
            {
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Name), ErrorCode.NameEmpty);
            }
            else if (ShowingWarehouse.Name.Length > 255)
            {
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Name), ErrorCode.NameOverLength);
            }
            return ShowingWarehouse.IsValidated;
        }
        #endregion

        private async Task<bool> ValidateAddress(ShowingWarehouse ShowingWarehouse)
        {
            if (!string.IsNullOrWhiteSpace(ShowingWarehouse.Address) && ShowingWarehouse.Address.Length > 255)
            {
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Address), ErrorCode.AddressOverLength);
            }
            return ShowingWarehouse.IsValidated;
        }

        private async Task<bool> ValidateOrganization(ShowingWarehouse ShowingWarehouse)
        {
            if (ShowingWarehouse.OrganizationId == 0)
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Organization), ErrorCode.OrganizationEmpty);
            else
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = ShowingWarehouse.OrganizationId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = OrganizationSelect.Id
                };

                int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.OrganizationId), ErrorCode.OrganizationNotExisted);
            }
            return ShowingWarehouse.IsValidated;
        }

        #region Province + District + Ward
        private async Task<bool> ValidateProvince(ShowingWarehouse ShowingWarehouse)
        {
            if (ShowingWarehouse.ProvinceId != 0)
            {
                ProvinceFilter ProvinceFilter = new ProvinceFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = ShowingWarehouse.ProvinceId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ProvinceSelect.Id
                };

                int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
                if (count == 0)
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Province), ErrorCode.ProvinceNotExisted);
            }
            return ShowingWarehouse.IsValidated;
        }
        private async Task<bool> ValidateDistrict(ShowingWarehouse ShowingWarehouse)
        {
            if (ShowingWarehouse.DistrictId != 0)
            {
                DistrictFilter DistrictFilter = new DistrictFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = ShowingWarehouse.DistrictId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = DistrictSelect.Id
                };

                int count = await UOW.DistrictRepository.Count(DistrictFilter);
                if (count == 0)
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.District), ErrorCode.DistrictNotExisted);
            }
            return ShowingWarehouse.IsValidated;
        }
        private async Task<bool> ValidateWard(ShowingWarehouse ShowingWarehouse)
        {
            if (ShowingWarehouse.WardId != 0)
            {
                WardFilter WardFilter = new WardFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = ShowingWarehouse.WardId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = WardSelect.Id
                };

                int count = await UOW.WardRepository.Count(WardFilter);
                if (count == 0)
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Ward), ErrorCode.WardNotExisted);
            }
            return ShowingWarehouse.IsValidated;
        }
        #endregion

        private async Task<bool> ValidateStatusId(ShowingWarehouse ShowingWarehouse)
        {
            if (StatusEnum.ACTIVE.Id != ShowingWarehouse.StatusId && StatusEnum.INACTIVE.Id != ShowingWarehouse.StatusId)
                ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Status), ErrorCode.StatusNotExisted);
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> Create(ShowingWarehouse ShowingWarehouse)
        {
            await ValidateCode(ShowingWarehouse);
            await ValidateName(ShowingWarehouse);
            await ValidateAddress(ShowingWarehouse);
            await ValidateOrganization(ShowingWarehouse);
            await ValidateProvince(ShowingWarehouse);
            await ValidateDistrict(ShowingWarehouse);
            await ValidateWard(ShowingWarehouse);
            await ValidateStatusId(ShowingWarehouse);
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> Update(ShowingWarehouse ShowingWarehouse)
        {
            if (await ValidateId(ShowingWarehouse))
            {
                await ValidateCode(ShowingWarehouse);
                await ValidateName(ShowingWarehouse);
                await ValidateAddress(ShowingWarehouse);
                await ValidateOrganization(ShowingWarehouse);
                await ValidateProvince(ShowingWarehouse);
                await ValidateDistrict(ShowingWarehouse);
                await ValidateWard(ShowingWarehouse);
                await ValidateStatusId(ShowingWarehouse);
            }
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> Delete(ShowingWarehouse ShowingWarehouse)
        {
            if (await ValidateId(ShowingWarehouse))
            {
                ShowingInventoryFilter filter = new ShowingInventoryFilter
                {
                    ShowingWarehouseId = new IdFilter { Equal = ShowingWarehouse.Id },
                    SaleStock = new LongFilter { Greater = 0 }
                };

                int count = await UOW.ShowingInventoryRepository.Count(filter);
                if (count != 0)
                {
                    ShowingWarehouse.AddError(nameof(ShowingWarehouseValidator), nameof(ShowingWarehouse.Id), ErrorCode.ShowingWarehouseHasShowingInventory);
                }
            }
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> BulkDelete(List<ShowingWarehouse> ShowingWarehouses)
        {
            foreach (var ShowingWarehouse in ShowingWarehouses)
            {
                await Delete(ShowingWarehouse);
            }
            return ShowingWarehouses.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<ShowingWarehouse> ShowingWarehouses)
        {
            return true;
        }
    }
}
