using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Common;
using DMS.Entities;
using DMS;
using DMS.Repositories;

namespace DMS.Services.MShowingCategory
{
    public interface IShowingCategoryValidator : IServiceScoped
    {
        Task<bool> Create(ShowingCategory ShowingCategory);
        Task<bool> Update(ShowingCategory ShowingCategory);
        Task<bool> Delete(ShowingCategory ShowingCategory);
        Task<bool> BulkDelete(List<ShowingCategory> ShowingCategories);
        Task<bool> Import(List<ShowingCategory> ShowingCategories);
    }

    public class ShowingCategoryValidator : IShowingCategoryValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public ShowingCategoryValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(ShowingCategory ShowingCategory)
        {
            ShowingCategoryFilter ShowingCategoryFilter = new ShowingCategoryFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = ShowingCategory.Id },
                Selects = ShowingCategorySelect.Id
            };

            int count = await UOW.ShowingCategoryRepository.Count(ShowingCategoryFilter);
            if (count == 0)
                ShowingCategory.AddError(nameof(ShowingCategoryValidator), nameof(ShowingCategory.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(ShowingCategory ShowingCategory)
        {
            return ShowingCategory.IsValidated;
        }

        public async Task<bool> Update(ShowingCategory ShowingCategory)
        {
            if (await ValidateId(ShowingCategory))
            {
            }
            return ShowingCategory.IsValidated;
        }

        public async Task<bool> Delete(ShowingCategory ShowingCategory)
        {
            if (await ValidateId(ShowingCategory))
            {
            }
            return ShowingCategory.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<ShowingCategory> ShowingCategories)
        {
            foreach (ShowingCategory ShowingCategory in ShowingCategories)
            {
                await Delete(ShowingCategory);
            }
            return ShowingCategories.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<ShowingCategory> ShowingCategories)
        {
            return true;
        }
    }
}
