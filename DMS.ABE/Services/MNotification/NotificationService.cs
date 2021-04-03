using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Helpers;
using DMS.ABE.Repositories;
using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Services.MNotification
{
    public interface INotificationService : IServiceScoped
    {
       
        Task<List<UserNotification>> BulkSend(List<UserNotification> UserNotifications);
    }

    public class NotificationService : BaseService, INotificationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;

        public NotificationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
        }
     
   
        public async Task<List<UserNotification>> BulkSend(List<UserNotification> UserNotifications)
        {
            if (StaticParams.EnableExternalService)
            {
                RestClient restClient = new RestClient(InternalServices.UTILS);
                RestRequest restRequest = new RestRequest("/rpc/utils/user-notification/bulk-create");
                restRequest.RequestFormat = DataFormat.Json;
                restRequest.Method = Method.POST;
                restRequest.AddCookie("Token", CurrentContext.Token);
                restRequest.AddCookie("X-Language", CurrentContext.Language);
                restRequest.AddJsonBody(UserNotifications);
                try
                {
                    var response = await restClient.ExecuteAsync<List<UserNotification>>(restRequest);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return UserNotifications;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
