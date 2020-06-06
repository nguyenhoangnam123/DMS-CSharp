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

namespace DMS.Services.MProblemStatus
{
    public interface IProblemStatusService :  IServiceScoped
    {
        Task<int> Count(ProblemStatusFilter ProblemStatusFilter);
        Task<List<ProblemStatus>> List(ProblemStatusFilter ProblemStatusFilter);
        Task<ProblemStatus> Get(long Id);
        Task<ProblemStatus> Create(ProblemStatus ProblemStatus);
        Task<ProblemStatus> Update(ProblemStatus ProblemStatus);
        Task<ProblemStatus> Delete(ProblemStatus ProblemStatus);
        Task<List<ProblemStatus>> BulkDelete(List<ProblemStatus> ProblemStatuses);
        Task<List<ProblemStatus>> Import(List<ProblemStatus> ProblemStatuses);
        ProblemStatusFilter ToFilter(ProblemStatusFilter ProblemStatusFilter);
    }

    public class ProblemStatusService : BaseService, IProblemStatusService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProblemStatusValidator ProblemStatusValidator;

        public ProblemStatusService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProblemStatusValidator ProblemStatusValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProblemStatusValidator = ProblemStatusValidator;
        }
        public async Task<int> Count(ProblemStatusFilter ProblemStatusFilter)
        {
            try
            {
                int result = await UOW.ProblemStatusRepository.Count(ProblemStatusFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProblemStatus>> List(ProblemStatusFilter ProblemStatusFilter)
        {
            try
            {
                List<ProblemStatus> ProblemStatuss = await UOW.ProblemStatusRepository.List(ProblemStatusFilter);
                return ProblemStatuss;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ProblemStatus> Get(long Id)
        {
            ProblemStatus ProblemStatus = await UOW.ProblemStatusRepository.Get(Id);
            if (ProblemStatus == null)
                return null;
            return ProblemStatus;
        }
       
        public async Task<ProblemStatus> Create(ProblemStatus ProblemStatus)
        {
            if (!await ProblemStatusValidator.Create(ProblemStatus))
                return ProblemStatus;

            try
            {
                await UOW.Begin();
                await UOW.ProblemStatusRepository.Create(ProblemStatus);
                await UOW.Commit();

                await Logging.CreateAuditLog(ProblemStatus, new { }, nameof(ProblemStatusService));
                return await UOW.ProblemStatusRepository.Get(ProblemStatus.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ProblemStatus> Update(ProblemStatus ProblemStatus)
        {
            if (!await ProblemStatusValidator.Update(ProblemStatus))
                return ProblemStatus;
            try
            {
                var oldData = await UOW.ProblemStatusRepository.Get(ProblemStatus.Id);

                await UOW.Begin();
                await UOW.ProblemStatusRepository.Update(ProblemStatus);
                await UOW.Commit();

                var newData = await UOW.ProblemStatusRepository.Get(ProblemStatus.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProblemStatusService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ProblemStatus> Delete(ProblemStatus ProblemStatus)
        {
            if (!await ProblemStatusValidator.Delete(ProblemStatus))
                return ProblemStatus;

            try
            {
                await UOW.Begin();
                await UOW.ProblemStatusRepository.Delete(ProblemStatus);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ProblemStatus, nameof(ProblemStatusService));
                return ProblemStatus;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProblemStatus>> BulkDelete(List<ProblemStatus> ProblemStatuses)
        {
            if (!await ProblemStatusValidator.BulkDelete(ProblemStatuses))
                return ProblemStatuses;

            try
            {
                await UOW.Begin();
                await UOW.ProblemStatusRepository.BulkDelete(ProblemStatuses);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ProblemStatuses, nameof(ProblemStatusService));
                return ProblemStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<ProblemStatus>> Import(List<ProblemStatus> ProblemStatuses)
        {
            if (!await ProblemStatusValidator.Import(ProblemStatuses))
                return ProblemStatuses;
            try
            {
                await UOW.Begin();
                await UOW.ProblemStatusRepository.BulkMerge(ProblemStatuses);
                await UOW.Commit();

                await Logging.CreateAuditLog(ProblemStatuses, new { }, nameof(ProblemStatusService));
                return ProblemStatuses;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProblemStatusService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
        public ProblemStatusFilter ToFilter(ProblemStatusFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProblemStatusFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProblemStatusFilter subFilter = new ProblemStatusFilter();
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
