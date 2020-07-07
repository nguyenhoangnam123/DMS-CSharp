using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Repositories;
using DMS.Rpc.e_route;
using DMS.Services.MAppUser;
using DMS.Services.MNotification;
using DMS.Services.MOrganization;
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
        Task<ERouteFilter> ToFilter(ERouteFilter ERouteFilter);

        Task<List<Store>> ListStore(StoreFilter StoreFilter);
    }

    public class ERouteService : BaseService, IERouteService
    {
        private IUOW UOW;
        private ILogging Logging;
        private INotificationService NotificationService;
        private ICurrentContext CurrentContext;
        private IERouteValidator ERouteValidator;
        private IStoreService StoreService;
        private IOrganizationService OrganizationService;
        private IAppUserService AppUserService;

        public ERouteService(
            IUOW UOW,
            ILogging Logging,
            INotificationService NotificationService,
            ICurrentContext CurrentContext,
            IStoreService StoreService,
            IOrganizationService OrganizationService,
            IAppUserService AppUserService,
            IERouteValidator ERouteValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.NotificationService = NotificationService;
            this.CurrentContext = CurrentContext;
            this.StoreService = StoreService;
            this.OrganizationService = OrganizationService;
            this.AppUserService = AppUserService;
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
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
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                int diff = (7 + (ERoute.StartDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                ERoute.RealStartDate = ERoute.StartDate.AddDays(-1 * diff);
                ERoute.CreatorId = CurrentContext.UserId;
                ERoute.RequestStateId = Enums.RequestStateEnum.NEW.Id;
                await UOW.Begin();
                await UOW.ERouteRepository.Create(ERoute);
                await UOW.Commit();

                DateTime Now = StaticParams.DateTimeNow;
                UserNotification NotificationUtils = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"Tuyên {ERoute.Code} - {ERoute.Name} đã được thêm mới cho anh/chị bởi {CurrentUser.DisplayName} vào lúc {Now}",
                    LinkWebsite = $"{ERouteRoute.Master}/{ERoute.Id}",
                    Time = Now,
                    Unread = false,
                    SenderId = CurrentContext.UserId,
                    RecipientId = ERoute.SaleEmployeeId
                };
                await NotificationService.BulkSend(new List<UserNotification> { NotificationUtils });

                await Logging.CreateAuditLog(ERoute, new { }, nameof(ERouteService));
                return await UOW.ERouteRepository.Get(ERoute.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ERoute> Update(ERoute ERoute)
        {
            if (!await ERouteValidator.Update(ERoute))
                return ERoute;
            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var oldData = await UOW.ERouteRepository.Get(ERoute.Id);
                int diff = (7 + (ERoute.StartDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                ERoute.RealStartDate = ERoute.StartDate.AddDays(-1 * diff);
                await UOW.Begin();
                await UOW.ERouteRepository.Update(ERoute);
                await UOW.Commit();

                DateTime Now = StaticParams.DateTimeNow;
                UserNotification NotificationUtils = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"Tuyên {ERoute.Code} - {ERoute.Name} đã được cập nhật cho anh/chị bởi {CurrentUser.DisplayName} vào lúc {Now}",
                    LinkWebsite = $"{ERouteRoute.Master}/{ERoute.Id}",
                    Time = Now,
                    Unread = false,
                    SenderId = CurrentContext.UserId,
                    RecipientId = ERoute.SaleEmployeeId
                };
                await NotificationService.BulkSend(new List<UserNotification> { NotificationUtils });

                var newData = await UOW.ERouteRepository.Get(ERoute.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ERouteService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
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

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                DateTime Now = StaticParams.DateTimeNow;
                UserNotification NotificationUtils = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"Tuyên {ERoute.Code} - {ERoute.Name} đã được xoá khỏi hệ thống bởi {CurrentUser.DisplayName} vào lúc {Now}",
                    Time = Now,
                    Unread = false,
                    SenderId = CurrentContext.UserId,
                    RecipientId = ERoute.SaleEmployeeId
                };
                await NotificationService.BulkSend(new List<UserNotification> { NotificationUtils });

                await Logging.CreateAuditLog(new { }, ERoute, nameof(ERouteService));
                return ERoute;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
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
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ERouteService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ERouteService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ERouteFilter> ToFilter(ERouteFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ERouteFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            List<Organization> Organizations = await OrganizationService.List(new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            foreach (var currentFilter in CurrentContext.Filters)
            {
                ERouteFilter subFilter = new ERouteFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                    {
                        var organizationIds = FilterOrganization(Organizations, FilterPermissionDefinition.IdFilter);
                        IdFilter IdFilter = new IdFilter { In = organizationIds };
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, IdFilter);
                    }
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreId))
                        subFilter.StoreId = FilterBuilder.Merge(subFilter.StoreId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.ERouteTypeId))
                        subFilter.ERouteTypeId = FilterBuilder.Merge(subFilter.ERouteTypeId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(subFilter.RequestStateId))
                        subFilter.RequestStateId = FilterBuilder.Merge(subFilter.RequestStateId, FilterPermissionDefinition.IdFilter);

                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.Equal = CurrentContext.UserId;
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                            if (subFilter.AppUserId == null) subFilter.AppUserId = new IdFilter { };
                            subFilter.AppUserId.NotEqual = CurrentContext.UserId;
                        }
                    }
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
