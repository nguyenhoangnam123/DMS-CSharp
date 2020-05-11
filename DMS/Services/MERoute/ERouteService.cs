using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MERoute
{
    public interface IERouteService :  IServiceScoped
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
    }

    public class ERouteService : BaseService, IERouteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IERouteValidator ERouteValidator;

        public ERouteService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IERouteValidator ERouteValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
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
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.SaleEmployeeId))
                        subFilter.SaleEmployeeId = Map(subFilter.SaleEmployeeId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StartDate))
                        subFilter.StartDate = Map(subFilter.StartDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.EndDate))
                        subFilter.EndDate = Map(subFilter.EndDate, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestStateId))
                        subFilter.RequestStateId = Map(subFilter.RequestStateId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.StatusId))
                        subFilter.StatusId = Map(subFilter.StatusId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.CreatorId))
                        subFilter.CreatorId = Map(subFilter.CreatorId, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}