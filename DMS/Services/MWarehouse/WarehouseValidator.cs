using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;
using DMS.Enums;

namespace DMS.Services.MWarehouse
{
    public interface IWarehouseValidator : IServiceScoped
    {
        Task<bool> Create(Warehouse Warehouse);
        Task<bool> Update(Warehouse Warehouse);
        Task<bool> Delete(Warehouse Warehouse);
        Task<bool> BulkDelete(List<Warehouse> Warehouses);
        Task<bool> Import(List<Warehouse> Warehouses);
    }

    public class WarehouseValidator : IWarehouseValidator
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
            WarehouseHasInventory
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public WarehouseValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Warehouse Warehouse)
        {
            WarehouseFilter WarehouseFilter = new WarehouseFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Warehouse.Id },
                Selects = WarehouseSelect.Id
            };

            int count = await UOW.WarehouseRepository.Count(WarehouseFilter);
            if (count == 0)
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        #region Code
        private async Task<bool> ValidateCode(Warehouse Warehouse)
        {
            if (string.IsNullOrWhiteSpace(Warehouse.Code))
            {
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Code), ErrorCode.CodeEmpty);
            }
            else
            {
                var Code = Warehouse.Code;
                if (Warehouse.Code.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(Warehouse.Code))
                {
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Code), ErrorCode.CodeHasSpecialCharacter);
                }

                WarehouseFilter WarehouseFilter = new WarehouseFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = Warehouse.Id },
                    Code = new StringFilter { Equal = Warehouse.Code },
                    Selects = WarehouseSelect.Code
                };

                int count = await UOW.WarehouseRepository.Count(WarehouseFilter);
                if (count != 0)
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Code), ErrorCode.CodeExisted);
            }

            return Warehouse.IsValidated;
        }
        #endregion

        #region Name
        private async Task<bool> ValidateName(Warehouse Warehouse)
        {
            if (string.IsNullOrWhiteSpace(Warehouse.Name))
            {
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Name), ErrorCode.NameEmpty);
            }
            else if (Warehouse.Name.Length > 255)
            {
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Name), ErrorCode.NameOverLength);
            }
            return Warehouse.IsValidated;
        }
        #endregion

        private async Task<bool> ValidateAddress(Warehouse Warehouse)
        {
            if (!string.IsNullOrWhiteSpace(Warehouse.Address) && Warehouse.Address.Length > 255)
            {
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Address), ErrorCode.AddressOverLength);
            }
            return Warehouse.IsValidated;
        }

        private async Task<bool> ValidateOrganizationId(Warehouse Warehouse)
        {
            if (Warehouse.OrganizationId == 0)
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.OrganizationId), ErrorCode.OrganizationEmpty);
            else
            {
                OrganizationFilter OrganizationFilter = new OrganizationFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Warehouse.OrganizationId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = OrganizationSelect.Id
                };

                int count = await UOW.OrganizationRepository.Count(OrganizationFilter);
                if (count == 0)
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.OrganizationId), ErrorCode.OrganizationNotExisted);
            }
            return Warehouse.IsValidated;
        }

        #region Province + District + Ward
        private async Task<bool> ValidateProvinceId(Warehouse Warehouse)
        {
            if (Warehouse.ProvinceId != 0)
            {
                ProvinceFilter ProvinceFilter = new ProvinceFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Warehouse.ProvinceId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = ProvinceSelect.Id
                };

                int count = await UOW.ProvinceRepository.Count(ProvinceFilter);
                if (count == 0)
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.ProvinceId), ErrorCode.ProvinceNotExisted);
            }
            return Warehouse.IsValidated;
        }
        private async Task<bool> ValidateDistrictId(Warehouse Warehouse)
        {
            if (Warehouse.DistrictId != 0)
            {
                DistrictFilter DistrictFilter = new DistrictFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Warehouse.DistrictId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = DistrictSelect.Id
                };

                int count = await UOW.DistrictRepository.Count(DistrictFilter);
                if (count == 0)
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.DistrictId), ErrorCode.DistrictNotExisted);
            }
            return Warehouse.IsValidated;
        }
        private async Task<bool> ValidateWardId(Warehouse Warehouse)
        {
            if (Warehouse.WardId != 0)
            {
                WardFilter WardFilter = new WardFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { Equal = Warehouse.WardId },
                    StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id },
                    Selects = WardSelect.Id
                };

                int count = await UOW.WardRepository.Count(WardFilter);
                if (count == 0)
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.WardId), ErrorCode.WardNotExisted);
            }
            return Warehouse.IsValidated;
        }
        #endregion

        private async Task<bool> ValidateStatusId(Warehouse Warehouse)
        {
            if (StatusEnum.ACTIVE.Id != Warehouse.StatusId && StatusEnum.INACTIVE.Id != Warehouse.StatusId)
                Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Status), ErrorCode.StatusNotExisted);
            return Warehouse.IsValidated;
        }

        public async Task<bool> Create(Warehouse Warehouse)
        {
            await ValidateCode(Warehouse);
            await ValidateName(Warehouse);
            await ValidateAddress(Warehouse);
            await ValidateOrganizationId(Warehouse);
            await ValidateProvinceId(Warehouse);
            await ValidateDistrictId(Warehouse);
            await ValidateWardId(Warehouse);
            await ValidateStatusId(Warehouse);
            return Warehouse.IsValidated;
        }

        public async Task<bool> Update(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
                await ValidateCode(Warehouse);
                await ValidateName(Warehouse);
                await ValidateAddress(Warehouse);
                await ValidateOrganizationId(Warehouse);
                await ValidateProvinceId(Warehouse);
                await ValidateDistrictId(Warehouse);
                await ValidateWardId(Warehouse);
                await ValidateStatusId(Warehouse);
            }
            return Warehouse.IsValidated;
        }

        public async Task<bool> Delete(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
                InventoryFilter filter = new InventoryFilter
                {
                    WarehouseId = new IdFilter { Equal = Warehouse.Id },
                    SaleStock = new LongFilter { Greater = 0 }
                };

                int count = await UOW.InventoryRepository.Count(filter);
                if (count != 0)
                {
                    Warehouse.AddError(nameof(WarehouseValidator), nameof(Warehouse.Status), ErrorCode.WarehouseHasInventory);
                }
            }
            return Warehouse.IsValidated;
        }

        public async Task<bool> BulkDelete(List<Warehouse> Warehouses)
        {
            foreach (var Warehouse in Warehouses)
            {
                await Delete(Warehouse);
            }
            return Warehouses.All(st => st.IsValidated);
        }

        public async Task<bool> Import(List<Warehouse> Warehouses)
        {
            return true;
        }
    }
}
