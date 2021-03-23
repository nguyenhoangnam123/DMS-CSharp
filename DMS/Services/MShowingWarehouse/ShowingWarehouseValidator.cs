using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

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

        public async Task<bool>Create(ShowingWarehouse ShowingWarehouse)
        {
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> Update(ShowingWarehouse ShowingWarehouse)
        {
            if (await ValidateId(ShowingWarehouse))
            {
            }
            return ShowingWarehouse.IsValidated;
        }

        public async Task<bool> Delete(ShowingWarehouse ShowingWarehouse)
        {
            if (await ValidateId(ShowingWarehouse))
            {
            }
            return ShowingWarehouse.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ShowingWarehouse> ShowingWarehouses)
        {
            foreach (ShowingWarehouse ShowingWarehouse in ShowingWarehouses)
            {
                await Delete(ShowingWarehouse);
            }
            return ShowingWarehouses.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ShowingWarehouse> ShowingWarehouses)
        {
            return true;
        }
    }
}
