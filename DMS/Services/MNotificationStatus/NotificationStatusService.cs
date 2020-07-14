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

namespace DMS.Services.MNotificationStatus
{
    public interface INotificationStatusService :  IServiceScoped
    {
        Task<int> Count(NotificationStatusFilter NotificationStatusFilter);
        Task<List<NotificationStatus>> List(NotificationStatusFilter NotificationStatusFilter);
        Task<NotificationStatus> Get(long Id);
    }

    public class NotificationStatusService : BaseService, INotificationStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificationStatusValidator NotificationStatusValidator;

        public NotificationStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificationStatusValidator NotificationStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificationStatusValidator = NotificationStatusValidator;
        }
        public async Task<int> Count(NotificationStatusFilter NotificationStatusFilter)
        {
            try
            {
                int result = await UOW.NotificationStatusRepository.Count(NotificationStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(NotificationStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<NotificationStatus>> List(NotificationStatusFilter NotificationStatusFilter)
        {
            try
            {
                List<NotificationStatus> NotificationStatuss = await UOW.NotificationStatusRepository.List(NotificationStatusFilter);
                return NotificationStatuss;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(NotificationStatusService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<NotificationStatus> Get(long Id)
        {
            NotificationStatus NotificationStatus = await UOW.NotificationStatusRepository.Get(Id);
            if (NotificationStatus == null)
                return null;
            return NotificationStatus;
        }
        
        public NotificationStatusFilter ToFilter(NotificationStatusFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<NotificationStatusFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                NotificationStatusFilter subFilter = new NotificationStatusFilter();
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
