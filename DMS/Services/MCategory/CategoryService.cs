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

namespace DMS.Services.MCategory
{
    public interface ICategoryService :  IServiceScoped
    {
        Task<int> Count(CategoryFilter CategoryFilter);
        Task<List<Category>> List(CategoryFilter CategoryFilter);
        Task<Category> Get(long Id);
        Task<CategoryFilter> ToFilter(CategoryFilter CategoryFilter);
    }

    public class CategoryService : BaseService, ICategoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ICategoryValidator CategoryValidator;

        public CategoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ICategoryValidator CategoryValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.CategoryValidator = CategoryValidator;
        }
        public async Task<int> Count(CategoryFilter CategoryFilter)
        {
            try
            {
                int result = await UOW.CategoryRepository.Count(CategoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Category>> List(CategoryFilter CategoryFilter)
        {
            try
            {
                List<Category> Categorys = await UOW.CategoryRepository.List(CategoryFilter);
                return Categorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(CategoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(CategoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Category> Get(long Id)
        {
            Category Category = await UOW.CategoryRepository.Get(Id);
            if (Category == null)
                return null;
            return Category;
        }
        
        public async Task<CategoryFilter> ToFilter(CategoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<CategoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                CategoryFilter subFilter = new CategoryFilter();
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
