using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

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

        public async Task<bool>Create(Warehouse Warehouse)
        {
            return Warehouse.IsValidated;
        }

        public async Task<bool> Update(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
            }
            return Warehouse.IsValidated;
        }

        public async Task<bool> Delete(Warehouse Warehouse)
        {
            if (await ValidateId(Warehouse))
            {
            }
            return Warehouse.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Warehouse> Warehouses)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Warehouse> Warehouses)
        {
            return true;
        }
    }
}
