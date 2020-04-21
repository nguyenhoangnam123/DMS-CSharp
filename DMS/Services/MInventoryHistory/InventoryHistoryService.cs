using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MInventoryHistoryHistory
{
    public interface IInventoryHistoryService : IServiceScoped
    {
        Task<int> Count(InventoryHistoryFilter InventoryHistoryFilter);
        Task<List<InventoryHistory>> List(InventoryHistoryFilter InventoryHistoryFilter);
        Task<InventoryHistory> Get(long Id);
        InventoryHistoryFilter ToFilter(InventoryHistoryFilter InventoryHistoryFilter);
    }
    public class InventoryHistoryService : BaseService, IInventoryHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public InventoryHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(InventoryHistoryFilter InventoryHistoryFilter)
        {
            try
            {
                int result = await UOW.InventoryHistoryRepository.Count(InventoryHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(InventoryHistoryService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<InventoryHistory>> List(InventoryHistoryFilter InventoryHistoryFilter)
        {
            try
            {
                List<InventoryHistory> InventoryHistorys = await UOW.InventoryHistoryRepository.List(InventoryHistoryFilter);
                return InventoryHistorys;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(InventoryHistoryService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<InventoryHistory> Get(long Id)
        {
            InventoryHistory InventoryHistory = await UOW.InventoryHistoryRepository.Get(Id);
            if (InventoryHistory == null)
                return null;
            return InventoryHistory;
        }

        public InventoryHistoryFilter ToFilter(InventoryHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<InventoryHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                InventoryHistoryFilter subFilter = new InventoryHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.InventoryId))
                        subFilter.InventoryId = Map(subFilter.InventoryId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OldSaleStock))
                        subFilter.OldSaleStock = Map(subFilter.OldSaleStock, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleStock))
                        subFilter.SaleStock = Map(subFilter.SaleStock, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OldAccountingStock))
                        subFilter.OldAccountingStock = Map(subFilter.OldAccountingStock, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AccountingStock))
                        subFilter.AccountingStock = Map(subFilter.AccountingStock, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = Map(subFilter.AppUserId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
