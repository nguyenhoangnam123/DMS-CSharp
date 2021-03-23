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
        Task<ShowingInventory> Create(ShowingInventory ShowingInventory);
        Task<ShowingInventory> Update(ShowingInventory ShowingInventory);
        Task<ShowingInventory> Delete(ShowingInventory ShowingInventory);
        Task<List<ShowingInventory>> BulkDelete(List<ShowingInventory> ShowingInventories);
        Task<List<ShowingInventory>> Import(List<ShowingInventory> ShowingInventories);
        Task<ShowingInventoryFilter> ToFilter(ShowingInventoryFilter ShowingInventoryFilter);
    }

    public class ShowingInventoryService : BaseService, IShowingInventoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IShowingInventoryValidator ShowingInventoryValidator;

        public ShowingInventoryService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IShowingInventoryValidator ShowingInventoryValidator,
            ILogging Logging
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ShowingInventoryValidator = ShowingInventoryValidator;
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
        public async Task<ShowingInventory> Create(ShowingInventory ShowingInventory)
        {
            if (!await ShowingInventoryValidator.Create(ShowingInventory))
                return ShowingInventory;

            try
            {
                await UOW.ShowingInventoryRepository.Create(ShowingInventory);
                ShowingInventory = await UOW.ShowingInventoryRepository.Get(ShowingInventory.Id);
                await Logging.CreateAuditLog(ShowingInventory, new { }, nameof(ShowingInventoryService));
                return ShowingInventory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;
        }

        public async Task<ShowingInventory> Update(ShowingInventory ShowingInventory)
        {
            if (!await ShowingInventoryValidator.Update(ShowingInventory))
                return ShowingInventory;
            try
            {
                var oldData = await UOW.ShowingInventoryRepository.Get(ShowingInventory.Id);

                await UOW.ShowingInventoryRepository.Update(ShowingInventory);

                ShowingInventory = await UOW.ShowingInventoryRepository.Get(ShowingInventory.Id);
                await Logging.CreateAuditLog(ShowingInventory, oldData, nameof(ShowingInventoryService));
                return ShowingInventory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;
        }

        public async Task<ShowingInventory> Delete(ShowingInventory ShowingInventory)
        {
            if (!await ShowingInventoryValidator.Delete(ShowingInventory))
                return ShowingInventory;

            try
            {
                await UOW.ShowingInventoryRepository.Delete(ShowingInventory);
                await Logging.CreateAuditLog(new { }, ShowingInventory, nameof(ShowingInventoryService));
                return ShowingInventory;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;
        }

        public async Task<List<ShowingInventory>> BulkDelete(List<ShowingInventory> ShowingInventories)
        {
            if (!await ShowingInventoryValidator.BulkDelete(ShowingInventories))
                return ShowingInventories;

            try
            {
                await UOW.ShowingInventoryRepository.BulkDelete(ShowingInventories);
                await Logging.CreateAuditLog(new { }, ShowingInventories, nameof(ShowingInventoryService));
                return ShowingInventories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;

        }
        
        public async Task<List<ShowingInventory>> Import(List<ShowingInventory> ShowingInventories)
        {
            if (!await ShowingInventoryValidator.Import(ShowingInventories))
                return ShowingInventories;
            try
            {
                await UOW.ShowingInventoryRepository.BulkMerge(ShowingInventories);

                await Logging.CreateAuditLog(ShowingInventories, new { }, nameof(ShowingInventoryService));
                return ShowingInventories;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(ShowingInventoryService));
            }
            return null;
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
