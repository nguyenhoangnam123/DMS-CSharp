using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Repositories;
using DMS.Rpc.monitor_store_problems;
using DMS.Services.MImage;
using DMS.Services.MNotification;
using DMS.Services.MOrganization;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MProblem
{
    public interface IProblemService : IServiceScoped
    {
        Task<int> Count(ProblemFilter ProblemFilter);
        Task<List<Problem>> List(ProblemFilter ProblemFilter);
        Task<Problem> Get(long Id);
        Task<Problem> Create(Problem Problem);
        Task<Problem> Update(Problem Problem);
        Task<Problem> Delete(Problem Problem);
        Task<List<Problem>> BulkDelete(List<Problem> Problems);
        Task<List<Problem>> Import(List<Problem> Problems);
        Task<Image> SaveImage(Image Image);
        Task<ProblemFilter> ToFilter(ProblemFilter ProblemFilter);
    }

    public class ProblemService : BaseService, IProblemService
    {
        private IUOW UOW;
        private ILogging Logging;
        private INotificationService NotificationService;
        private ICurrentContext CurrentContext;
        private IImageService ImageService;
        private IOrganizationService OrganizationService;
        private IProblemValidator ProblemValidator;
        private IRabbitManager RabbitManager;

        public ProblemService(
            IUOW UOW,
            ILogging Logging,
            INotificationService NotificationService,
            ICurrentContext CurrentContext,
            IImageService ImageService,
            IOrganizationService OrganizationService,
            IProblemValidator ProblemValidator,
            IRabbitManager RabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.NotificationService = NotificationService;
            this.CurrentContext = CurrentContext;
            this.ImageService = ImageService;
            this.OrganizationService = OrganizationService;
            this.ProblemValidator = ProblemValidator;
            this.RabbitManager = RabbitManager;
        }
        public async Task<int> Count(ProblemFilter ProblemFilter)
        {
            try
            {
                int result = await UOW.ProblemRepository.Count(ProblemFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Problem>> List(ProblemFilter ProblemFilter)
        {
            try
            {
                List<Problem> Problems = await UOW.ProblemRepository.List(ProblemFilter);
                return Problems;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Problem> Get(long Id)
        {
            Problem Problem = await UOW.ProblemRepository.Get(Id);
            if (Problem == null)
                return null;
            return Problem;
        }

        public async Task<Problem> Create(Problem Problem)
        {
            if (!await ProblemValidator.Create(Problem))
                return Problem;

            try
            {
                Problem.CreatorId = CurrentContext.UserId;
                Problem.Code = StaticParams.DateTimeNow.ToString();
                await UOW.Begin();
                await UOW.ProblemRepository.Create(Problem);
                Problem = await UOW.ProblemRepository.Get(Problem.Id);
                var Year = StaticParams.DateTimeNow.Year.ToString().Substring(2);
                var Id = (1000000 + Problem.Id).ToString().Substring(1);
                Problem.Code = $"VD{Year}.{Id}";
                Problem.ProblemHistories = new List<ProblemHistory>
                {
                    new ProblemHistory
                    {
                        ProblemId = Problem.Id,
                        ModifierId = CurrentContext.UserId,
                        ProblemStatusId = Enums.ProblemStatusEnum.WAITING.Id,
                        Time = StaticParams.DateTimeNow
                    }
                };
                await UOW.ProblemRepository.Update(Problem);
                await UOW.Commit();

                NotifyUsed(Problem);

                var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                var RecipientIds = await ListReceipientId(CurrentUser, MonitorStoreProblemRoute.Update);

                DateTime Now = StaticParams.DateTimeNow;
                List<UserNotification> UserNotifications = new List<UserNotification>();
                foreach (var id in RecipientIds)
                {
                    UserNotification UserNotification = new UserNotification
                    {
                        TitleWeb = $"Thông báo từ DMS",
                        ContentWeb = $"Vấn đề {Problem.Code} của đại lý {Problem.Store.Code} - {Problem.Store.Name} đã được thêm mới lên hệ thống bởi {CurrentUser.DisplayName}",
                        LinkWebsite = $"{MonitorStoreProblemRoute.Master}#*".Replace("*", Problem.Id.ToString()),
                        LinkMobile = $"{MonitorStoreProblemRoute.Mobile}".Replace("*", Problem.Id.ToString()),
                        Time = Now,
                        Unread = true,
                        SenderId = CurrentContext.UserId,
                        RecipientId = id
                    };
                    UserNotifications.Add(UserNotification);
                }

                await NotificationService.BulkSend(UserNotifications);

                await Logging.CreateAuditLog(Problem, new { }, nameof(ProblemService));
                return await UOW.ProblemRepository.Get(Problem.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Problem> Update(Problem Problem)
        {
            if (!await ProblemValidator.Update(Problem))
                return Problem;
            try
            {
                var oldData = await UOW.ProblemRepository.Get(Problem.Id);
                Problem.Code = oldData.Code;
                if (Problem.ProblemStatusId != oldData.ProblemStatusId)
                {
                    Problem.ProblemHistories = oldData.ProblemHistories;
                    ProblemHistory ProblemHistory = new ProblemHistory
                    {
                        ProblemId = Problem.Id,
                        ModifierId = CurrentContext.UserId,
                        Time = StaticParams.DateTimeNow,
                    };
                    string status = "";
                    List<UserNotification> UserNotifications = new List<UserNotification>();
                    DateTime Now = StaticParams.DateTimeNow;
                    var CurrentUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.WAITING.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.WAITING.Id;
                        status = "chờ xử lý";
                    }
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.PROCESSING.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.PROCESSING.Id;
                        status = "đang xử lý";
                        UserNotification UserNotification = new UserNotification
                        {
                            TitleWeb = $"Thông báo từ DMS",
                            ContentWeb = $"Vấn đề {Problem.Code} của đại lý {Problem.Store.Code} - {Problem.Store.Name} đã chuyển trạng thái {status} bởi {CurrentUser.DisplayName}",
                            LinkWebsite = $"{MonitorStoreProblemRoute.Master}#*".Replace("*", Problem.Id.ToString()),
                            LinkMobile = $"{MonitorStoreProblemRoute.Mobile}".Replace("*", Problem.Id.ToString()),
                            Time = Now,
                            Unread = true,
                            SenderId = CurrentContext.UserId,
                            RecipientId = Problem.CreatorId,
                        };
                        UserNotifications.Add(UserNotification);
                    }
                    if (Problem.ProblemStatusId == Enums.ProblemStatusEnum.DONE.Id)
                    {
                        ProblemHistory.ProblemStatusId = Enums.ProblemStatusEnum.DONE.Id;
                        status = "hoàn thành";
                        Problem.CompletedAt = StaticParams.DateTimeNow;
                        UserNotification UserNotification = new UserNotification
                        {
                            TitleWeb = $"Thông báo từ DMS",
                            ContentWeb = $"Vấn đề {Problem.Code} của đại lý {Problem.Store.Code} - {Problem.Store.Name} đã chuyển trạng thái {status} bởi {CurrentUser.DisplayName}",
                            LinkWebsite = $"{MonitorStoreProblemRoute.Master}#*".Replace("*", Problem.Id.ToString()),
                            LinkMobile = $"{MonitorStoreProblemRoute.Mobile}".Replace("*", Problem.Id.ToString()),
                            Time = Now,
                            Unread = true,
                            SenderId = CurrentContext.UserId,
                            RecipientId = Problem.CreatorId,
                        };
                        UserNotifications.Add(UserNotification);
                    }
                    if (Problem.ProblemHistories == null)
                        Problem.ProblemHistories = new List<ProblemHistory>();
                    Problem.ProblemHistories.Add(ProblemHistory);

                    await NotificationService.BulkSend(UserNotifications);
                }
                await UOW.Begin();
                await UOW.ProblemRepository.Update(Problem);
                await UOW.Commit();

                NotifyUsed(Problem);

                var newData = await UOW.ProblemRepository.Get(Problem.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProblemService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Problem> Delete(Problem Problem)
        {
            if (!await ProblemValidator.Delete(Problem))
                return Problem;

            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.Delete(Problem);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Problem, nameof(ProblemService));
                return Problem;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();

                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Problem>> BulkDelete(List<Problem> Problems)
        {
            if (!await ProblemValidator.BulkDelete(Problems))
                return Problems;

            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.BulkDelete(Problems);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Problems, nameof(ProblemService));
                return Problems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Problem>> Import(List<Problem> Problems)
        {
            if (!await ProblemValidator.Import(Problems))
                return Problems;
            try
            {
                await UOW.Begin();
                await UOW.ProblemRepository.BulkMerge(Problems);
                await UOW.Commit();

                await Logging.CreateAuditLog(Problems, new { }, nameof(ProblemService));
                return Problems;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(ProblemService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<ProblemFilter> ToFilter(ProblemFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProblemFilter>();
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
                ProblemFilter subFilter = new ProblemFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.AppUserId))
                        subFilter.Id = FilterBuilder.Merge(subFilter.AppUserId, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                    {
                        var organizationIds = FilterOrganization(Organizations, FilterPermissionDefinition.IdFilter);
                        IdFilter IdFilter = new IdFilter { In = organizationIds };
                        subFilter.OrganizationId = FilterBuilder.Merge(subFilter.OrganizationId, IdFilter);
                    }
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

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/problem/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            Image = await ImageService.Create(Image, path);
            return Image;
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

        private void NotifyUsed(Problem Problem)
        {
            {
                EventMessage<ProblemType> ProblemTypeMessage = new EventMessage<ProblemType>
                {
                    Content = new ProblemType { Id = Problem.ProblemTypeId },
                    EntityName = nameof(Item),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(ProblemTypeMessage, RoutingKeyEnum.ProblemTypeUsed);
            }

            {
                EventMessage<Store> StoreMessage = new EventMessage<Store>
                {
                    Content = new Store { Id = Problem.StoreId },
                    EntityName = nameof(Store),
                    RowId = Guid.NewGuid(),
                    Time = StaticParams.DateTimeNow,
                };
                RabbitManager.PublishSingle(StoreMessage, RoutingKeyEnum.StoreUsed);
            }
        }
       
    }
}
