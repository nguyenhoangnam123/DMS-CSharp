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
        Task<NotificationStatus> Create(NotificationStatus NotificationStatus);
        Task<NotificationStatus> Update(NotificationStatus NotificationStatus);
        Task<NotificationStatus> Delete(NotificationStatus NotificationStatus);
        Task<List<NotificationStatus>> BulkDelete(List<NotificationStatus> NotificationStatuses);
        Task<List<NotificationStatus>> Import(List<NotificationStatus> NotificationStatuses);
        NotificationStatusFilter ToFilter(NotificationStatusFilter NotificationStatusFilter);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
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
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<NotificationStatus> Get(long Id)
        {
            NotificationStatus NotificationStatus = await UOW.NotificationStatusRepository.Get(Id);
            if (NotificationStatus == null)
                return null;
            return NotificationStatus;
        }
       
        public async Task<NotificationStatus> Create(NotificationStatus NotificationStatus)
        {
            if (!await NotificationStatusValidator.Create(NotificationStatus))
                return NotificationStatus;

            try
            {
                await UOW.Begin();
                await UOW.NotificationStatusRepository.Create(NotificationStatus);
                await UOW.Commit();

                await Logging.CreateAuditLog(NotificationStatus, new { }, nameof(NotificationStatusService));
                return await UOW.NotificationStatusRepository.Get(NotificationStatus.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<NotificationStatus> Update(NotificationStatus NotificationStatus)
        {
            if (!await NotificationStatusValidator.Update(NotificationStatus))
                return NotificationStatus;
            try
            {
                var oldData = await UOW.NotificationStatusRepository.Get(NotificationStatus.Id);

                await UOW.Begin();
                await UOW.NotificationStatusRepository.Update(NotificationStatus);
                await UOW.Commit();

                var newData = await UOW.NotificationStatusRepository.Get(NotificationStatus.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(NotificationStatusService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<NotificationStatus> Delete(NotificationStatus NotificationStatus)
        {
            if (!await NotificationStatusValidator.Delete(NotificationStatus))
                return NotificationStatus;

            try
            {
                await UOW.Begin();
                await UOW.NotificationStatusRepository.Delete(NotificationStatus);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, NotificationStatus, nameof(NotificationStatusService));
                return NotificationStatus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<NotificationStatus>> BulkDelete(List<NotificationStatus> NotificationStatuses)
        {
            if (!await NotificationStatusValidator.BulkDelete(NotificationStatuses))
                return NotificationStatuses;

            try
            {
                await UOW.Begin();
                await UOW.NotificationStatusRepository.BulkDelete(NotificationStatuses);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, NotificationStatuses, nameof(NotificationStatusService));
                return NotificationStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<NotificationStatus>> Import(List<NotificationStatus> NotificationStatuses)
        {
            if (!await NotificationStatusValidator.Import(NotificationStatuses))
                return NotificationStatuses;
            try
            {
                await UOW.Begin();
                await UOW.NotificationStatusRepository.BulkMerge(NotificationStatuses);
                await UOW.Commit();

                await Logging.CreateAuditLog(NotificationStatuses, new { }, nameof(NotificationStatusService));
                return NotificationStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificationStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
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
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
