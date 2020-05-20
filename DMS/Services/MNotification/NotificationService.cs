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

namespace DMS.Services.MNotification
{
    public interface INotificationService :  IServiceScoped
    {
        Task<int> Count(NotificationFilter NotificationFilter);
        Task<List<Notification>> List(NotificationFilter NotificationFilter);
        Task<Notification> Get(long Id);
        Task<Notification> Create(Notification Notification);
        Task<Notification> Update(Notification Notification);
        Task<Notification> Delete(Notification Notification);
        Task<List<Notification>> BulkDelete(List<Notification> Notifications);
        Task<List<Notification>> Import(List<Notification> Notifications);
        NotificationFilter ToFilter(NotificationFilter NotificationFilter);
    }

    public class NotificationService : BaseService, INotificationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationValidator NotificationValidator;

        public NotificationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificationValidator NotificationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationValidator = NotificationValidator;
        }
        public async Task<int> Count(NotificationFilter NotificationFilter)
        {
            try
            {
                int result = await UOW.NotificationRepository.Count(NotificationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Notification>> List(NotificationFilter NotificationFilter)
        {
            try
            {
                List<Notification> Notifications = await UOW.NotificationRepository.List(NotificationFilter);
                return Notifications;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Notification> Get(long Id)
        {
            Notification Notification = await UOW.NotificationRepository.Get(Id);
            if (Notification == null)
                return null;
            return Notification;
        }
       
        public async Task<Notification> Create(Notification Notification)
        {
            if (!await NotificationValidator.Create(Notification))
                return Notification;

            try
            {
                
                await UOW.Begin();
                await UOW.NotificationRepository.Create(Notification);
                await UOW.Commit();

                if (Notification.OrganizationId.HasValue)
                {
                    AppUserFilter AppUserFilter = new AppUserFilter
                    {
                        Skip = 0,
                        Take = int.MaxValue,
                        OrganizationId = new IdFilter { Equal = Notification.OrganizationId.Value },
                        Selects = AppUserSelect.Id
                    };

                    var AppUserIds = (await UOW.AppUserRepository.List(AppUserFilter));

                    List<NotificationUtils> NotificationUtilss = AppUserIds.Select(x => new NotificationUtils
                    {
                        Content = Notification.Content,
                        Time = StaticParams.DateTimeNow,
                        Unread = false,
                        AppUserId = x.Id
                    }).ToList();

                    await SendNotification(NotificationUtilss);
                }

                await Logging.CreateAuditLog(Notification, new { }, nameof(NotificationService));
                return await UOW.NotificationRepository.Get(Notification.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Notification> Update(Notification Notification)
        {
            if (!await NotificationValidator.Update(Notification))
                return Notification;
            try
            {
                var oldData = await UOW.NotificationRepository.Get(Notification.Id);

                await UOW.Begin();
                await UOW.NotificationRepository.Update(Notification);
                await UOW.Commit();

                var newData = await UOW.NotificationRepository.Get(Notification.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(NotificationService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Notification> Delete(Notification Notification)
        {
            if (!await NotificationValidator.Delete(Notification))
                return Notification;

            try
            {
                await UOW.Begin();
                await UOW.NotificationRepository.Delete(Notification);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Notification, nameof(NotificationService));
                return Notification;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Notification>> BulkDelete(List<Notification> Notifications)
        {
            if (!await NotificationValidator.BulkDelete(Notifications))
                return Notifications;

            try
            {
                await UOW.Begin();
                await UOW.NotificationRepository.BulkDelete(Notifications);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Notifications, nameof(NotificationService));
                return Notifications;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Notification>> Import(List<Notification> Notifications)
        {
            if (!await NotificationValidator.Import(Notifications))
                return Notifications;
            try
            {
                await UOW.Begin();
                await UOW.NotificationRepository.BulkMerge(Notifications);
                await UOW.Commit();

                await Logging.CreateAuditLog(Notifications, new { }, nameof(NotificationService));
                return Notifications;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public NotificationFilter ToFilter(NotificationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<NotificationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                NotificationFilter subFilter = new NotificationFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Title))
                        subFilter.Title = Map(subFilter.Title, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Content))
                        subFilter.Content = Map(subFilter.Content, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.OrganizationId))
                        subFilter.OrganizationId = Map(subFilter.OrganizationId, FilterPermissionDefinition);
                }
            }
            return filter;
        }

        private async Task<List<NotificationUtils>> SendNotification(List<NotificationUtils> NotificationUtilss)
        {
            RestClient restClient = new RestClient($"http://localhost:{Modules.Utils}");
            RestRequest restRequest = new RestRequest("/rpc/utils/notification/bulk-create");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddBody(NotificationUtilss);
            try
            {
                var response = restClient.Execute<List<NotificationUtils>>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return NotificationUtilss;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public class NotificationUtils
        {
            public long Id { get; set; }
            public string Content { get; set; }
            public long AppUserId { get; set; }
            public bool Unread { get; set; }
            public DateTime Time { get; set; }
        }
    }

}