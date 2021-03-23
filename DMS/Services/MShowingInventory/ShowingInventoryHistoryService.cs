using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MShowingShowingInventory
{
    public interface IShowingInventoryHistoryService : IServiceScoped
    {
        Task<int> Count(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter);
        Task<List<ShowingInventoryHistory>> List(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter);
        Task<ShowingInventoryHistory> Get(long Id);
        ShowingInventoryHistoryFilter ToFilter(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter);
    }
    public class ShowingInventoryHistoryService : BaseService, IShowingInventoryHistoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public ShowingInventoryHistoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter)
        {
            try
            {
                int result = await UOW.ShowingInventoryHistoryRepository.Count(ShowingInventoryHistoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ShowingInventoryHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ShowingInventoryHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<ShowingInventoryHistory>> List(ShowingInventoryHistoryFilter ShowingInventoryHistoryFilter)
        {
            try
            {
                List<ShowingInventoryHistory> ShowingInventoryHistorys = await UOW.ShowingInventoryHistoryRepository.List(ShowingInventoryHistoryFilter);
                return ShowingInventoryHistorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ShowingInventoryHistoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ShowingInventoryHistoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<ShowingInventoryHistory> Get(long Id)
        {
            ShowingInventoryHistory ShowingInventoryHistory = await UOW.ShowingInventoryHistoryRepository.Get(Id);
            if (ShowingInventoryHistory == null)
                return null;
            return ShowingInventoryHistory;
        }

        public ShowingInventoryHistoryFilter ToFilter(ShowingInventoryHistoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ShowingInventoryHistoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ShowingInventoryHistoryFilter subFilter = new ShowingInventoryHistoryFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
