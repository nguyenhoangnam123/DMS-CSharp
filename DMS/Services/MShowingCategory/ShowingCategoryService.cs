using DMS.Common;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;
using DMS.Enums;

namespace DMS.Services.MShowingCategory
{
    public interface IShowingCategoryService :  IServiceScoped
    {
        Task<int> Count(ShowingCategoryFilter ShowingCategoryFilter);
        Task<List<ShowingCategory>> List(ShowingCategoryFilter ShowingCategoryFilter);
        Task<ShowingCategory> Get(long Id);
        Task<ShowingCategory> Create(ShowingCategory ShowingCategory);
        Task<ShowingCategory> Update(ShowingCategory ShowingCategory);
        Task<ShowingCategory> Delete(ShowingCategory ShowingCategory);
        Task<List<ShowingCategory>> BulkDelete(List<ShowingCategory> ShowingCategories);
        Task<List<ShowingCategory>> Import(List<ShowingCategory> ShowingCategories);
        Task<ShowingCategoryFilter> ToFilter(ShowingCategoryFilter ShowingCategoryFilter);
    }

    public class ShowingCategoryService : BaseService, IShowingCategoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingCategoryValidator ShowingCategoryValidator;

        public ShowingCategoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingCategoryValidator ShowingCategoryValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingCategoryValidator = ShowingCategoryValidator;
        }
        public async Task<int> Count(ShowingCategoryFilter ShowingCategoryFilter)
        {
            try
            {
                int result = await UOW.ShowingCategoryRepository.Count(ShowingCategoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return 0;
        }

        public async Task<List<ShowingCategory>> List(ShowingCategoryFilter ShowingCategoryFilter)
        {
            try
            {
                List<ShowingCategory> ShowingCategories = await UOW.ShowingCategoryRepository.List(ShowingCategoryFilter);
                return ShowingCategories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;
        }
        
        public async Task<ShowingCategory> Get(long Id)
        {
            ShowingCategory ShowingCategory = await UOW.ShowingCategoryRepository.Get(Id);
            if (ShowingCategory == null)
                return null;
            return ShowingCategory;
        }
        public async Task<ShowingCategory> Create(ShowingCategory ShowingCategory)
        {
            if (!await ShowingCategoryValidator.Create(ShowingCategory))
                return ShowingCategory;

            try
            {
                await UOW.ShowingCategoryRepository.Create(ShowingCategory);
                ShowingCategory = await UOW.ShowingCategoryRepository.Get(ShowingCategory.Id);
                await Logging.CreateAuditLog(ShowingCategory, new { }, nameof(ShowingCategoryService));
                return ShowingCategory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;
        }

        public async Task<ShowingCategory> Update(ShowingCategory ShowingCategory)
        {
            if (!await ShowingCategoryValidator.Update(ShowingCategory))
                return ShowingCategory;
            try
            {
                var oldData = await UOW.ShowingCategoryRepository.Get(ShowingCategory.Id);

                await UOW.ShowingCategoryRepository.Update(ShowingCategory);

                ShowingCategory = await UOW.ShowingCategoryRepository.Get(ShowingCategory.Id);
                await Logging.CreateAuditLog(ShowingCategory, oldData, nameof(ShowingCategoryService));
                return ShowingCategory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;
        }

        public async Task<ShowingCategory> Delete(ShowingCategory ShowingCategory)
        {
            if (!await ShowingCategoryValidator.Delete(ShowingCategory))
                return ShowingCategory;

            try
            {
                await UOW.ShowingCategoryRepository.Delete(ShowingCategory);
                await Logging.CreateAuditLog(new { }, ShowingCategory, nameof(ShowingCategoryService));
                return ShowingCategory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;
        }

        public async Task<List<ShowingCategory>> BulkDelete(List<ShowingCategory> ShowingCategories)
        {
            if (!await ShowingCategoryValidator.BulkDelete(ShowingCategories))
                return ShowingCategories;

            try
            {
                await UOW.ShowingCategoryRepository.BulkDelete(ShowingCategories);
                await Logging.CreateAuditLog(new { }, ShowingCategories, nameof(ShowingCategoryService));
                return ShowingCategories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;

        }
        
        public async Task<List<ShowingCategory>> Import(List<ShowingCategory> ShowingCategories)
        {
            if (!await ShowingCategoryValidator.Import(ShowingCategories))
                return ShowingCategories;
            try
            {
                await UOW.ShowingCategoryRepository.BulkMerge(ShowingCategories);

                await Logging.CreateAuditLog(ShowingCategories, new { }, nameof(ShowingCategoryService));
                return ShowingCategories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingCategoryService));
            }
            return null;
        }     
        
        public async Task<ShowingCategoryFilter> ToFilter(ShowingCategoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingCategoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingCategoryFilter subFilter = new ShowingCategoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = FilterBuilder.Merge(subFilter.Code, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = FilterBuilder.Merge(subFilter.Name, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = FilterBuilder.Merge(subFilter.ParentId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Path))
                        subFilter.Path = FilterBuilder.Merge(subFilter.Path, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Level))
                        subFilter.Level = FilterBuilder.Merge(subFilter.Level, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = FilterBuilder.Merge(subFilter.StatusId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ImageId))
                        subFilter.ImageId = FilterBuilder.Merge(subFilter.ImageId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }
    }
}
