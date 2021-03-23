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

namespace DMS.Services.MShowingItem
{
    public interface IShowingItemService :  IServiceScoped
    {
        Task<int> Count(ShowingItemFilter ShowingItemFilter);
        Task<List<ShowingItem>> List(ShowingItemFilter ShowingItemFilter);
        Task<ShowingItem> Get(long Id);
        Task<ShowingItem> Create(ShowingItem ShowingItem);
        Task<ShowingItem> Update(ShowingItem ShowingItem);
        Task<ShowingItem> Delete(ShowingItem ShowingItem);
        Task<List<ShowingItem>> BulkDelete(List<ShowingItem> ShowingItems);
        Task<List<ShowingItem>> Import(List<ShowingItem> ShowingItems);
        Task<ShowingItemFilter> ToFilter(ShowingItemFilter ShowingItemFilter);
    }

    public class ShowingItemService : BaseService, IShowingItemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingItemValidator ShowingItemValidator;

        public ShowingItemService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingItemValidator ShowingItemValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingItemValidator = ShowingItemValidator;
        }
        public async Task<int> Count(ShowingItemFilter ShowingItemFilter)
        {
            try
            {
                int result = await UOW.ShowingItemRepository.Count(ShowingItemFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return 0;
        }

        public async Task<List<ShowingItem>> List(ShowingItemFilter ShowingItemFilter)
        {
            try
            {
                List<ShowingItem> ShowingItems = await UOW.ShowingItemRepository.List(ShowingItemFilter);
                return ShowingItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;
        }
        
        public async Task<ShowingItem> Get(long Id)
        {
            ShowingItem ShowingItem = await UOW.ShowingItemRepository.Get(Id);
            if (ShowingItem == null)
                return null;
            return ShowingItem;
        }
        public async Task<ShowingItem> Create(ShowingItem ShowingItem)
        {
            if (!await ShowingItemValidator.Create(ShowingItem))
                return ShowingItem;

            try
            {
                await UOW.ShowingItemRepository.Create(ShowingItem);
                ShowingItem = await UOW.ShowingItemRepository.Get(ShowingItem.Id);
                await Logging.CreateAuditLog(ShowingItem, new { }, nameof(ShowingItemService));
                return ShowingItem;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;
        }

        public async Task<ShowingItem> Update(ShowingItem ShowingItem)
        {
            if (!await ShowingItemValidator.Update(ShowingItem))
                return ShowingItem;
            try
            {
                var oldData = await UOW.ShowingItemRepository.Get(ShowingItem.Id);

                await UOW.ShowingItemRepository.Update(ShowingItem);

                ShowingItem = await UOW.ShowingItemRepository.Get(ShowingItem.Id);
                await Logging.CreateAuditLog(ShowingItem, oldData, nameof(ShowingItemService));
                return ShowingItem;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;
        }

        public async Task<ShowingItem> Delete(ShowingItem ShowingItem)
        {
            if (!await ShowingItemValidator.Delete(ShowingItem))
                return ShowingItem;

            try
            {
                await UOW.ShowingItemRepository.Delete(ShowingItem);
                await Logging.CreateAuditLog(new { }, ShowingItem, nameof(ShowingItemService));
                return ShowingItem;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;
        }

        public async Task<List<ShowingItem>> BulkDelete(List<ShowingItem> ShowingItems)
        {
            if (!await ShowingItemValidator.BulkDelete(ShowingItems))
                return ShowingItems;

            try
            {
                await UOW.ShowingItemRepository.BulkDelete(ShowingItems);
                await Logging.CreateAuditLog(new { }, ShowingItems, nameof(ShowingItemService));
                return ShowingItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;

        }
        
        public async Task<List<ShowingItem>> Import(List<ShowingItem> ShowingItems)
        {
            if (!await ShowingItemValidator.Import(ShowingItems))
                return ShowingItems;
            try
            {
                await UOW.ShowingItemRepository.BulkMerge(ShowingItems);

                await Logging.CreateAuditLog(ShowingItems, new { }, nameof(ShowingItemService));
                return ShowingItems;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingItemService));
            }
            return null;
        }     
        
        public async Task<ShowingItemFilter> ToFilter(ShowingItemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingItemFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingItemFilter subFilter = new ShowingItemFilter();
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CategoryId))
                        subFilter.CategoryId = FilterBuilder.Merge(subFilter.CategoryId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.UnitOfMeasureId))
                        subFilter.UnitOfMeasureId = FilterBuilder.Merge(subFilter.UnitOfMeasureId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SalePrice))
                        subFilter.SalePrice = FilterBuilder.Merge(subFilter.SalePrice, FilterPermissionDefinition.DecimalFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Desception))
                        subFilter.Desception = FilterBuilder.Merge(subFilter.Desception, FilterPermissionDefinition.StringFilter);
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
