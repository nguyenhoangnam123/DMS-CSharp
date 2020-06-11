using Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Services.MStore;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MERoute
{
    public interface IERouteService : IServiceScoped
    {
        Task<int> Count(ERouteFilter ERouteFilter);
        Task<List<ERoute>> List(ERouteFilter ERouteFilter);
        Task<ERoute> Get(long Id);
        Task<ERoute> Create(ERoute ERoute);
        Task<ERoute> Update(ERoute ERoute);
        Task<ERoute> Delete(ERoute ERoute);
        Task<List<ERoute>> BulkDelete(List<ERoute> ERoutes);
        Task<List<ERoute>> Import(List<ERoute> ERoutes);
        ERouteFilter ToFilter(ERouteFilter ERouteFilter);

        Task<List<Store>> ListStore(StoreFilter StoreFilter);
    }

    public class ERouteService : BaseService, IERouteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IERouteValidator ERouteValidator;
        private IStoreService StoreService;

        public ERouteService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreService StoreService,
            IERouteValidator ERouteValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreService = StoreService;
            this.ERouteValidator = ERouteValidator;
        }
        public async Task<int> Count(ERouteFilter ERouteFilter)
        {
            try
            {
                int result = await UOW.ERouteRepository.Count(ERouteFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERoute>> List(ERouteFilter ERouteFilter)
        {
            try
            {
                List<ERoute> ERoutes = await UOW.ERouteRepository.List(ERouteFilter);
                return ERoutes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ERoute> Get(long Id)
        {
            ERoute ERoute = await UOW.ERouteRepository.Get(Id);
            if (ERoute == null)
                return null;
            return ERoute;
        }

        public async Task<ERoute> Create(ERoute ERoute)
        {
            if (!await ERouteValidator.Create(ERoute))
                return ERoute;

            try
            {
                int diff = (7 + (ERoute.StartDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                ERoute.RealStartDate = ERoute.StartDate.AddDays(-1 * diff);
                await UOW.Begin();
                await UOW.ERouteRepository.Create(ERoute);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERoute, new { }, nameof(ERouteService));
                return await UOW.ERouteRepository.Get(ERoute.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERoute> Update(ERoute ERoute)
        {
            if (!await ERouteValidator.Update(ERoute))
                return ERoute;
            try
            {
                var oldData = await UOW.ERouteRepository.Get(ERoute.Id);

                await UOW.Begin();
                await UOW.ERouteRepository.Update(ERoute);
                await UOW.Commit();

                var newData = await UOW.ERouteRepository.Get(ERoute.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ERouteService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ERoute> Delete(ERoute ERoute)
        {
            if (!await ERouteValidator.Delete(ERoute))
                return ERoute;

            try
            {
                await UOW.Begin();
                await UOW.ERouteRepository.Delete(ERoute);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERoute, nameof(ERouteService));
                return ERoute;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERoute>> BulkDelete(List<ERoute> ERoutes)
        {
            if (!await ERouteValidator.BulkDelete(ERoutes))
                return ERoutes;

            try
            {
                await UOW.Begin();
                await UOW.ERouteRepository.BulkDelete(ERoutes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ERoutes, nameof(ERouteService));
                return ERoutes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ERoute>> Import(List<ERoute> ERoutes)
        {
            if (!await ERouteValidator.Import(ERoutes))
                return ERoutes;
            try
            {
                await UOW.Begin();
                await UOW.ERouteRepository.BulkMerge(ERoutes);
                await UOW.Commit();

                await Logging.CreateAuditLog(ERoutes, new { }, nameof(ERouteService));
                return ERoutes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public ERouteFilter ToFilter(ERouteFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ERouteFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ERouteFilter subFilter = new ERouteFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleEmployeeId))
                        subFilter.SaleEmployeeId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreId))
                        subFilter.StoreId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.ERouteTypeId))
                        subFilter.ERouteTypeId = FilterPermissionDefinition.IdFilter;

                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestStateId))
                        subFilter.RequestStateId = FilterPermissionDefinition.IdFilter;

                }
            }
            return filter;
        }

        public async Task<List<Store>> ListStore(StoreFilter StoreFilter)
        {
            List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
            List<long> StoreIds = Stores.Select(s => s.Id).ToList();
            ERouteContentFilter ERouteContentFilter = new ERouteContentFilter
            {
                StoreId = new IdFilter { In = StoreIds },
                Skip = 0,
                Take = int.MaxValue,
                Selects = ERouteContentSelect.Id | ERouteContentSelect.Store,
            };

            List<ERouteContent> ERouteContents = await UOW.ERouteContentRepository.List(ERouteContentFilter);
            foreach (Store Store in Stores)
            {
                Store.HasEroute = ERouteContents.Where(e => e.StoreId == Store.Id).Count() > 0;
            }
            return Stores;
        }
    }
}
