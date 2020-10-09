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
using RestSharp;
using DMS.Helpers;
using DMS.Handlers;
using DMS.Enums;
using DMS.Services.MNotification;
using DMS.Rpc.store_scouting;
using DMS.Services.MOrganization;

namespace DMS.Services.MStoreScouting
{
    public interface IStoreScoutingService : IServiceScoped
    {
        Task<int> Count(StoreScoutingFilter StoreScoutingFilter);
        Task<List<StoreScouting>> List(StoreScoutingFilter StoreScoutingFilter);
        Task<StoreScouting> Get(long Id);
        Task<StoreScouting> Create(StoreScouting StoreScouting);
        Task<StoreScouting> Update(StoreScouting StoreScouting);
        Task<StoreScouting> Delete(StoreScouting StoreScouting);
        Task<StoreScouting> Reject(StoreScouting StoreScouting);
        Task<StoreScoutingFilter> ToFilter(StoreScoutingFilter StoreScoutingFilter);
    }

    public class StoreScoutingService : BaseService, IStoreScoutingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationService NotificationService;
        private IOrganizationService OrganizationService;
        private IStoreScoutingValidator StoreScoutingValidator;
        private IRabbitManager RabbitManager;

        public StoreScoutingService(
            IUOW UOW,
            ILogging Logging,
            INotificationService NotificationService,
            IOrganizationService OrganizationService,
            ICurrentContext CurrentContext,
            IStoreScoutingValidator StoreScoutingValidator,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.NotificationService = NotificationService;
            this.OrganizationService = OrganizationService;
            this.CurrentContext = CurrentContext;
            this.StoreScoutingValidator = StoreScoutingValidator;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(StoreScoutingFilter StoreScoutingFilter)
        {
            try
            {
                int result = await UOW.StoreScoutingRepository.Count(StoreScoutingFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<StoreScouting>> List(StoreScoutingFilter StoreScoutingFilter)
        {
            try
            {
                List<StoreScouting> StoreScoutings = await UOW.StoreScoutingRepository.List(StoreScoutingFilter);
                return StoreScoutings;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<StoreScouting> Get(long Id)
        {
            StoreScouting StoreScouting = await UOW.StoreScoutingRepository.Get(Id);
            if (StoreScouting == null)
                return null;
            return StoreScouting;
        }

        public async Task<StoreScouting> Create(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Create(StoreScouting))
                return StoreScouting;

            try
            {
                var User = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                StoreScouting.CreatorId = User.Id;
                StoreScouting.OrganizationId = User.OrganizationId;
                StoreScouting.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.NOTOPEN.Id;
                await UOW.Begin();
                await UOW.StoreScoutingRepository.Create(StoreScouting);
                await UOW.StoreScoutingRepository.Update(StoreScouting);
                await UOW.Commit();

                var RecipientIds = await ListReceipientId(User, StoreScoutingRoute.Reject);

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var Id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Đại lý cắm cờ {StoreScouting.Code} - {StoreScouting.Name} vừa được thêm mới vào hệ thống bởi {User.DisplayName}",
                        LinkWebsite = $"{StoreScoutingRoute.Master}/id=*".Replace("*", StoreScouting.Id.ToString()),
                        LinkMobile = $"{StoreScoutingRoute.Mobile}".Replace("*", StoreScouting.Id.ToString()),
                        Time = Now,
                        Unread = false,
                        SenderId = CurrentContext.UserId,
                        RecipientId = Id,
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(StoreScouting, new { }, nameof(StoreScoutingService));
                return await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreScouting> Update(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Update(StoreScouting))
                return StoreScouting;
            try
            {
                var oldData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);

                await UOW.Begin();
                await UOW.StoreScoutingRepository.Update(StoreScouting);
                await UOW.Commit();

                var newData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreScoutingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreScouting> Delete(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Delete(StoreScouting))
                return StoreScouting;
            try
            {
                var oldData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);

                await UOW.Begin();
                await UOW.StoreScoutingRepository.Delete(StoreScouting);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreScouting, oldData, nameof(StoreScoutingService));
                return StoreScouting;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreScouting> Reject(StoreScouting StoreScouting)
        {
            if (!await StoreScoutingValidator.Reject(StoreScouting))
                return StoreScouting;

            try
            {
                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var oldData = await UOW.StoreScoutingRepository.Get(StoreScouting.Id);
                var Creator = await UOW.AppUserRepository.Get(oldData.CreatorId);
                oldData.StoreScoutingStatusId = Enums.StoreScoutingStatusEnum.REJECTED.Id;
                await UOW.Begin();
                await UOW.StoreScoutingRepository.Update(oldData);
                await UOW.Commit();

                DateTime Now = StaticParams.DateTimeNow;
                UserNotification UserNotification = new UserNotification
                {
                    TitleWeb = $"Thông báo từ DMS",
                    ContentWeb = $"{oldData.Name} đã bị từ chối bởi {CurrentUser.DisplayName}.",
                    LinkWebsite = $"{StoreScoutingRoute.Master}/id=*".Replace("*", StoreScouting.Id.ToString()),
                    LinkMobile = $"{StoreScoutingRoute.Mobile}".Replace("*", StoreScouting.Id.ToString()),
                    Time = Now,
                    Unread = false,
                    SenderId = CurrentContext.UserId,
                    RecipientId = Creator.Id
                };
                await NotificationService.BulkSend(new List<UserNotification> { UserNotification });

                Mail mail = new Mail
                {
                    Subject = "Từ chối tạo đại lý",
                    Body = $"{oldData.Name} đã bị từ chối bởi {CurrentUser.DisplayName}. Chi tiết xem tại {StoreScouting.Link}",
                    Recipients = new List<string> { Creator.Email },
                    RowId = Guid.NewGuid()
                };
                RabbitManager.PublishSingle(new EventMessage<Mail>(mail, mail.RowId), RoutingKeyEnum.MailSend);
                await Logging.CreateAuditLog(StoreScouting, oldData, nameof(StoreScoutingService));
                return StoreScouting;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreScoutingService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        private async Task<List<long>> ListReceipientId(AppUser CurrentUser, string Path)
        {
            var Ids = await UOW.PermissionRepository.ListAppUser(Path);
            OrganizationFilter OrganizationFilter = new OrganizationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            };

            var Organizations = await UOW.OrganizationRepository.List(OrganizationFilter);
            var OrganizationIds = Organizations
                .Where(x => x.Path.StartsWith(CurrentUser.Organization.Path) || CurrentUser.Organization.Path.StartsWith(x.Path))
                .Select(x => x.Id)
                .ToList();

            var AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = AppUserSelect.Id | AppUserSelect.Organization,
                OrganizationId = new IdFilter { In = OrganizationIds },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }
            });
            var AppUserIds = AppUsers.Where(x => OrganizationIds.Contains(x.OrganizationId)).Select(x => x.Id).ToList();
            AppUserIds = AppUserIds.Intersect(Ids).ToList();
            return AppUserIds;
        }

        public async Task<StoreScoutingFilter> ToFilter(StoreScoutingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreScoutingFilter>();
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
                StoreScoutingFilter subFilter = new StoreScoutingFilter();
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

                    if (FilterPermissionDefinition.Name == nameof(subFilter.StoreScoutingStatusId))
                        subFilter.AppUserId = FilterBuilder.Merge(subFilter.StoreScoutingStatusId, FilterPermissionDefinition.IdFilter);

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
    }
}
