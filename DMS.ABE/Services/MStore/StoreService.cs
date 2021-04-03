using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Enums;
using DMS.ABE.Handlers;
using DMS.ABE.Repositories;
using DMS.ABE.Services.MImage;
using DMS.ABE.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.ABE.Services.MNotification;

namespace DMS.ABE.Services.MStore
{
    public interface IStoreService : IServiceScoped
    {
        Task<int> Count(StoreFilter StoreFilter);
        Task<List<Store>> List(StoreFilter StoreFilter);
        Task<Store> Get(long Id);
        StoreFilter ToFilter(StoreFilter StoreFilter);
    }

    public class StoreService : BaseService, IStoreService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreValidator StoreValidator;
        private IImageService ImageService;
        private INotificationService NotificationService;
        private IRabbitManager RabbitManager;
        public StoreService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IStoreValidator StoreValidator,
            INotificationService NotificationService,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreValidator = StoreValidator;
            this.ImageService = ImageService;
            this.NotificationService = NotificationService;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(StoreFilter StoreFilter)
        {
            try
            {
                int result = await UOW.StoreRepository.Count(StoreFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Store>> List(StoreFilter StoreFilter)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);

                var StoreIds = Stores.Select(x => x.Id).ToList();
                StoreUserFilter StoreUserFilter = new StoreUserFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    StoreId = new IdFilter { In = StoreIds },
                    Selects = StoreUserSelect.ALL
                };
                List<StoreUser> StoreUsers = await UOW.StoreUserRepository.List(StoreUserFilter);
                foreach (var Store in Stores)
                {
                    Store.StoreUser = StoreUsers.Where(x => x.StoreId == Store.Id).FirstOrDefault();
                    if (Store.StoreUser != null)
                    {
                        Store.StoreUserId = Store.StoreUser.Id;
                    }
                }
                return Stores;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Store> Get(long Id)
        {
            Store Store = await UOW.StoreRepository.Get(Id);
            if (Store == null)
                return null;

            return Store;
        }

        public StoreFilter ToFilter(StoreFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreFilter subFilter = new StoreFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreGroupingId))
                        subFilter.StoreGroupingId = FilterPermissionDefinition.IdFilter;
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreTypeId))
                        subFilter.StoreTypeId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }
    }
}
