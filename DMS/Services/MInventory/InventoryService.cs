using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MInventory
{
    public interface IInventoryService : IServiceScoped
    {
        Task<int> Count(InventoryFilter InventoryFilter);
        Task<List<Inventory>> List(InventoryFilter InventoryFilter);
        Task<Inventory> Get(long Id);
        InventoryFilter ToFilter(InventoryFilter InventoryFilter);
    }
    public class InventoryService : BaseService, IInventoryService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public InventoryService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
        public async Task<int> Count(InventoryFilter InventoryFilter)
        {
            try
            {
                int result = await UOW.InventoryRepository.Count(InventoryFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(InventoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(InventoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Inventory>> List(InventoryFilter InventoryFilter)
        {
            try
            {
                List<Inventory> Inventorys = await UOW.InventoryRepository.List(InventoryFilter);
                return Inventorys;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(InventoryService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(InventoryService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Inventory> Get(long Id)
        {
            Inventory Inventory = await UOW.InventoryRepository.Get(Id);
            if (Inventory == null)
                return null;
            return Inventory;
        }

        public InventoryFilter ToFilter(InventoryFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<InventoryFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                InventoryFilter subFilter = new InventoryFilter();
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
