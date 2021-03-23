using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MShowingInventory
{
    public interface IShowingInventoryValidator : IServiceScoped
    {
        Task<bool> Create(ShowingInventory ShowingInventory);
        Task<bool> Update(ShowingInventory ShowingInventory);
        Task<bool> Delete(ShowingInventory ShowingInventory);
        Task<bool> BulkDelete(List<ShowingInventory> ShowingInventories);
        Task<bool> Import(List<ShowingInventory> ShowingInventories);
    }

    public class ShowingInventoryValidator : IShowingInventoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ShowingInventoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ShowingInventory ShowingInventory)
        {
            ShowingInventoryFilter ShowingInventoryFilter = new ShowingInventoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ShowingInventory.Id },
                Selects = ShowingInventorySelect.Id
            };

            int count = await UOW.ShowingInventoryRepository.Count(ShowingInventoryFilter);
            if (count == 0)
                ShowingInventory.AddError(nameof(ShowingInventoryValidator), nameof(ShowingInventory.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ShowingInventory ShowingInventory)
        {
            return ShowingInventory.IsValidated;
        }

        public async Task<bool> Update(ShowingInventory ShowingInventory)
        {
            if (await ValidateId(ShowingInventory))
            {
            }
            return ShowingInventory.IsValidated;
        }

        public async Task<bool> Delete(ShowingInventory ShowingInventory)
        {
            if (await ValidateId(ShowingInventory))
            {
            }
            return ShowingInventory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ShowingInventory> ShowingInventories)
        {
            foreach (ShowingInventory ShowingInventory in ShowingInventories)
            {
                await Delete(ShowingInventory);
            }
            return ShowingInventories.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ShowingInventory> ShowingInventories)
        {
            return true;
        }
    }
}
