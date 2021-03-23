using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MShowingItem
{
    public interface IShowingItemValidator : IServiceScoped
    {
        Task<bool> Create(ShowingItem ShowingItem);
        Task<bool> Update(ShowingItem ShowingItem);
        Task<bool> Delete(ShowingItem ShowingItem);
        Task<bool> BulkDelete(List<ShowingItem> ShowingItems);
        Task<bool> Import(List<ShowingItem> ShowingItems);
    }

    public class ShowingItemValidator : IShowingItemValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ShowingItemValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ShowingItem ShowingItem)
        {
            ShowingItemFilter ShowingItemFilter = new ShowingItemFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ShowingItem.Id },
                Selects = ShowingItemSelect.Id
            };

            int count = await UOW.ShowingItemRepository.Count(ShowingItemFilter);
            if (count == 0)
                ShowingItem.AddError(nameof(ShowingItemValidator), nameof(ShowingItem.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ShowingItem ShowingItem)
        {
            return ShowingItem.IsValidated;
        }

        public async Task<bool> Update(ShowingItem ShowingItem)
        {
            if (await ValidateId(ShowingItem))
            {
            }
            return ShowingItem.IsValidated;
        }

        public async Task<bool> Delete(ShowingItem ShowingItem)
        {
            if (await ValidateId(ShowingItem))
            {
            }
            return ShowingItem.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ShowingItem> ShowingItems)
        {
            foreach (ShowingItem ShowingItem in ShowingItems)
            {
                await Delete(ShowingItem);
            }
            return ShowingItems.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ShowingItem> ShowingItems)
        {
            return true;
        }
    }
}
