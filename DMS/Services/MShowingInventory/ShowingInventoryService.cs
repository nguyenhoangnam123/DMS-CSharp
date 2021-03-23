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

namespace DMS.Services.MShowingInventory
{
    public interface IShowingInventoryService :  IServiceScoped
    {
        Task<int> Count(ShowingInventoryFilter ShowingInventoryFilter);
        Task<List<ShowingInventory>> List(ShowingInventoryFilter ShowingInventoryFilter);
        Task<ShowingInventory> Get(long Id);
        Task<ShowingInventoryFilter> ToFilter(ShowingInventoryFilter ShowingInventoryFilter);
    }

    public class ShowingInventoryService : BaseService, IShowingInventoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ShowingInventoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ShowingInventoryFilter ShowingInventoryFilter)
        {
            try
            {
                int result = await UOW.ShowingInventoryRepository.Count(ShowingInventoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return 0;
        }

        public async Task<List<ShowingInventory>> List(ShowingInventoryFilter ShowingInventoryFilter)
        {
            try
            {
                List<ShowingInventory> ShowingInventories = await UOW.ShowingInventoryRepository.List(ShowingInventoryFilter);
                return ShowingInventories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;
        }
        
        public async Task<ShowingInventory> Get(long Id)
        {
            ShowingInventory ShowingInventory = await UOW.ShowingInventoryRepository.Get(Id);
            if (ShowingInventory == null)
                return null;
            return ShowingInventory;
        }
        
        public async Task<ShowingInventoryFilter> ToFilter(ShowingInventoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingInventoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingInventoryFilter subFilter = new ShowingInventoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ShowingWarehouseId))
                        subFilter.ShowingWarehouseId = FilterBuilder.Merge(subFilter.ShowingWarehouseId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ShowingItemId))
                        subFilter.ShowingItemId = FilterBuilder.Merge(subFilter.ShowingItemId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleStock))
                        subFilter.SaleStock = FilterBuilder.Merge(subFilter.SaleStock, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AccountingStock))
                        subFilter.AccountingStock = FilterBuilder.Merge(subFilter.AccountingStock, FilterPermissionDefinition.LongFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
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
