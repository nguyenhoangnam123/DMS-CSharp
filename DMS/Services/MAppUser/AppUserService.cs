using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Services.MSex;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MAppUser
{
    public interface IAppUserService : IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<AppUser> Get(long Id);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> UpdateRole(AppUser AppUser);
        Task<AppUser> UpdateGPS(AppUser AppUser);
        Task<bool> ImportERouteScope(List<AppUser> AppUsers);
        AppUserFilter ToFilter(AppUserFilter AppUserFilter);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        private IRabbitManager RabbitManager;
        private ISexService SexService;

        public AppUserService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            IRabbitManager RabbitManager,
            ISexService SexService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
            this.RabbitManager = RabbitManager;
            this.SexService = SexService;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            return AppUser;
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                await UOW.Begin();
                await UOW.AppUserRepository.Update(AppUser);
                await UOW.Commit();

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> UpdateRole(AppUser AppUser)
        {
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.AppUserRoleMappings = AppUser.AppUserRoleMappings;
                await UOW.Begin();
                await UOW.AppUserRepository.Update(oldData);
                await UOW.Commit();

                var newData = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> UpdateGPS(AppUser AppUser)
        {
            try
            {
                if(AppUser.Latitude.HasValue && AppUser.Longitude.HasValue)
                {
                    EventMessage<AppUser> EventMessageAppUser = new EventMessage<AppUser>(AppUser, AppUser.RowId);
                    RabbitManager.PublishSingle(EventMessageAppUser, RoutingKeyEnum.UpdateGPS);
                }
                
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                return AppUser;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<bool> ImportERouteScope(List<AppUser> AppUsers)
        {
            try
            {
                List<AppUserStoreMapping> AppUserStoreMappings = new List<AppUserStoreMapping>();
                foreach (var AppUser in AppUsers)
                {
                    AppUserStoreMappings.AddRange(AppUser.AppUserStoreMappings);
                }
                var appUserIds = AppUsers.Select(x => x.Id).ToList();
                AppUserStoreMappings = AppUserStoreMappings.Distinct().ToList();
                return await UOW.AppUserRepository.BulkMergeERouteScope(AppUserStoreMappings, appUserIds);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public AppUserFilter ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = FilterPermissionDefinition.IdFilter;
                }
            }
            return filter;
        }
    }
}
