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

namespace DMS.Services.MResellerStatus
{
    public interface IResellerStatusService :  IServiceScoped
    {
        Task<int> Count(ResellerStatusFilter ResellerStatusFilter);
        Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter);
        Task<ResellerStatus> Get(long Id);
        Task<ResellerStatus> Create(ResellerStatus ResellerStatus);
        Task<ResellerStatus> Update(ResellerStatus ResellerStatus);
        Task<ResellerStatus> Delete(ResellerStatus ResellerStatus);
        Task<List<ResellerStatus>> BulkDelete(List<ResellerStatus> ResellerStatuses);
        Task<List<ResellerStatus>> Import(List<ResellerStatus> ResellerStatuses);
        ResellerStatusFilter ToFilter(ResellerStatusFilter ResellerStatusFilter);
    }

    public class ResellerStatusService : BaseService, IResellerStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IResellerStatusValidator ResellerStatusValidator;

        public ResellerStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IResellerStatusValidator ResellerStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ResellerStatusValidator = ResellerStatusValidator;
        }
        public async Task<int> Count(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                int result = await UOW.ResellerStatusRepository.Count(ResellerStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ResellerStatus>> List(ResellerStatusFilter ResellerStatusFilter)
        {
            try
            {
                List<ResellerStatus> ResellerStatuss = await UOW.ResellerStatusRepository.List(ResellerStatusFilter);
                return ResellerStatuss;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ResellerStatus> Get(long Id)
        {
            ResellerStatus ResellerStatus = await UOW.ResellerStatusRepository.Get(Id);
            if (ResellerStatus == null)
                return null;
            return ResellerStatus;
        }
       
        public async Task<ResellerStatus> Create(ResellerStatus ResellerStatus)
        {
            if (!await ResellerStatusValidator.Create(ResellerStatus))
                return ResellerStatus;

            try
            {
                await UOW.Begin();
                await UOW.ResellerStatusRepository.Create(ResellerStatus);
                await UOW.Commit();

                await Logging.CreateAuditLog(ResellerStatus, new { }, nameof(ResellerStatusService));
                return await UOW.ResellerStatusRepository.Get(ResellerStatus.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ResellerStatus> Update(ResellerStatus ResellerStatus)
        {
            if (!await ResellerStatusValidator.Update(ResellerStatus))
                return ResellerStatus;
            try
            {
                var oldData = await UOW.ResellerStatusRepository.Get(ResellerStatus.Id);

                await UOW.Begin();
                await UOW.ResellerStatusRepository.Update(ResellerStatus);
                await UOW.Commit();

                var newData = await UOW.ResellerStatusRepository.Get(ResellerStatus.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ResellerStatusService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ResellerStatus> Delete(ResellerStatus ResellerStatus)
        {
            if (!await ResellerStatusValidator.Delete(ResellerStatus))
                return ResellerStatus;

            try
            {
                await UOW.Begin();
                await UOW.ResellerStatusRepository.Delete(ResellerStatus);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ResellerStatus, nameof(ResellerStatusService));
                return ResellerStatus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ResellerStatus>> BulkDelete(List<ResellerStatus> ResellerStatuses)
        {
            if (!await ResellerStatusValidator.BulkDelete(ResellerStatuses))
                return ResellerStatuses;

            try
            {
                await UOW.Begin();
                await UOW.ResellerStatusRepository.BulkDelete(ResellerStatuses);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ResellerStatuses, nameof(ResellerStatusService));
                return ResellerStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ResellerStatus>> Import(List<ResellerStatus> ResellerStatuses)
        {
            if (!await ResellerStatusValidator.Import(ResellerStatuses))
                return ResellerStatuses;
            try
            {
                await UOW.Begin();
                await UOW.ResellerStatusRepository.BulkMerge(ResellerStatuses);
                await UOW.Commit();

                await Logging.CreateAuditLog(ResellerStatuses, new { }, nameof(ResellerStatusService));
                return ResellerStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ResellerStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ResellerStatusFilter ToFilter(ResellerStatusFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ResellerStatusFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ResellerStatusFilter subFilter = new ResellerStatusFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
            }
            return filter;
        }
    }
}
